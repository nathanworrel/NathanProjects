using CommonServices.Retrievers.CharlesSchwab;
using EasyNetQ;
using FishyLibrary;
using FishyLibrary.Enums;
using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.Client;
using FishyLibrary.Models.Order;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.Trade;
using FishyLibrary.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApi.GetData.Context;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MakeTrade.Services;

public class MakeTradeService : IMakeTradeService
{
    private readonly ILogger<MakeTradeService> _logger;
    private MakeTradeContext _makeTradeContext;
    private ICharlesSchwabRetriever _charlesSchwabRetriever;
    private readonly IBus _notificationBus;

    public MakeTradeService(
        ILogger<MakeTradeService> logger, MakeTradeContext makeTradeContext,
        ICharlesSchwabRetriever charlesSchwabRetriever, 
        IBus notificationBus)
    {
        _logger = logger;
        _makeTradeContext = makeTradeContext;
        _charlesSchwabRetriever = charlesSchwabRetriever;
        _notificationBus = notificationBus;
    }

    /*
     * Sends the trade to be placed to the Charles Schwab Endpoint
     *  @trade - the trade to be placed
     *  @userId - the user_id for the user
     */


    public Trade? MakeTrade(float desiredPosition, int strategyId, double currentPrice, bool dry)
    {
        Strategy? strategy = _makeTradeContext.strategies.FirstOrDefault(x => x.Id == strategyId);
        _logger.LogDebug("{strategy}", strategy);
        if (strategy == null)
        {
            throw new Exception($"Strategy {strategyId} not found.");
        }

        AccountInfo? accountInfo = _charlesSchwabRetriever.GetAccountInfo(strategy.AccountId);
        _logger.LogDebug("{accountInfo}", accountInfo);
        if (accountInfo == null)
        {
            throw new Exception($"Account info for {strategy.AccountId} not found.");
        }

        FishyLibrary.Helpers.MakeTrade? makeTrade = GenerateTrade(desiredPosition, strategy, currentPrice, accountInfo);
        if (makeTrade == null)
        {
            _logger.LogInformation("No trade generated for {strategyId}", strategyId);
            return new Trade();
        }
        _logger.LogDebug("{makeTrade}", makeTrade);

        long response = _charlesSchwabRetriever.SendTrade(makeTrade, strategy.AccountId, dry);
        
        if (dry)
        {
            return null;
        }
        
        if (response == 0)
        {
            _logger.LogError("Sending trade failed.");
            return null;
        }
        
        var trade = new Trade(
            DateTime.Now.ToUniversalTime(), 
            makeTrade.Quantity, 
            (decimal)currentPrice,
            response,
            (int) TradeStatus.ACCEPTED,
            (int) makeTrade.Side,
            DateTime.Now.ToUniversalTime(),
            0,
            0,
            (decimal)desiredPosition, 
            strategyId);
        _logger.LogDebug("{trade}", trade);
        _makeTradeContext.trades.Add(trade);
        _makeTradeContext.SaveChanges();
        
        _logger.LogInformation("Trade generated for {strategyId} and number {response} saved to database.", strategyId, response);
        
        return trade;
    }

    /*
     * Uses the given account information and desired position to generate the trade to be placed.
     *  @desiredPosition - range from 0 to 1 of how much we want invested in this product for a given strategy
     *  @strategy - the information about what stocks the strategy trades
     *  @currentPrice - the current price for the given product
     *  @accountInfo - account information like positions and balance etc
     */
    public FishyLibrary.Helpers.MakeTrade? GenerateTrade(float desiredPosition, Strategy strategy, double currentPrice,
        AccountInfo accountInfo)
    {
        int currentPosition = strategy.NumStocksHolding;
        double totalCapital = currentPosition * currentPrice + (double)strategy.AmountAllocated;
        int optimalPosition = (int)Math.Floor((totalCapital / currentPrice) * desiredPosition);

        _logger.LogInformation("Desired position: {optimalPosition}. Current position: {currentPosition} for {strategy.Id}.", optimalPosition, currentPosition, strategy.Id);

        if (AlmostEquals.Equals(optimalPosition, currentPosition))
        {
            _logger.LogInformation("Desired position is close to optimal position.");
            return null;
        }

        FishyLibrary.Helpers.MakeTrade makeTrade;
        if (optimalPosition > currentPosition)
        {
            var quantity = optimalPosition - currentPosition;
            if (quantity * currentPrice > accountInfo.GetAvailableCash())
            {
                _logger.LogError("Can't buy more stocks.");
                return null;
            }

            makeTrade = new FishyLibrary.Helpers.MakeTrade(strategy.Product, quantity, Side.BUY, currentPrice, 0,
                DateTime.Now);
        }
        else
        {
            var quantity = currentPosition - optimalPosition;
            makeTrade = new FishyLibrary.Helpers.MakeTrade(strategy.Product, quantity,
                Side.SELL, currentPrice, 0, DateTime.Now);
        }

        return makeTrade;
    }

    public async void SendNotification(int strategyId, bool dry, Trade? trade)
    {
        Client? client = _makeTradeContext.strategies
            .Include(x => x.Account)
            .ThenInclude(x => x.Client)
            .FirstOrDefault(x => x.Id == strategyId)?
            .Account.Client;
        if (client != null)
        {
            string message = $"Strategy {strategyId} ";
            if (trade != null)
            {
                message += $"Traded. Side: {trade.Side}, PricePlaced: {trade.PricePlaced}.";
            } 
            else if (dry)
            {
                message += $"on dry so no trade made.";
            }
            else
            {
                message += $"did not trade.";
            }

            INotification notification = new Notification() { Message = message, ChatId = Int64.Parse(client.TelegramChatId) };
            await _notificationBus.PubSub.PublishAsync(notification);
            _logger.LogInformation("Sent notification.");   
        }
        else
        {
            _logger.LogError("Unable to send notification for strategy {strategyId} because client is null.", strategyId);
        }
    }
}