using CommonServices.Retrievers.CharlesSchwab;
using FishyLibrary.Enums;
using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.Order;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.Trade;
using FishyLibrary.Utils;
using Newtonsoft.Json;
using WebApi.GetData.Context;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MakeTrade.Services;

public class MakeTradeService : IMakeTradeService
{
    private readonly ILogger<MakeTradeService> _logger;
    private MakeTradeContext _makeTradeContext;
    private ICharlesSchwabRetriever _charlesSchwabRetriever;

    public MakeTradeService(
        ILogger<MakeTradeService> logger, MakeTradeContext makeTradeContext,
        ICharlesSchwabRetriever charlesSchwabRetriever)
    {
        _logger = logger;
        _makeTradeContext = makeTradeContext;
        _charlesSchwabRetriever = charlesSchwabRetriever;
    }

    /*
     * Sends the trade to be placed to the Charles Schwab Endpoint
     *  @trade - the trade to be placed
     *  @userId - the user_id for the user
     */


    public Trade? MakeTrade(float desiredPosition, int strategyId, double currentPrice, bool dry)
    {
        Strategy? strategy = _makeTradeContext.strategies.FirstOrDefault(x => x.Id == strategyId);
        if (strategy == null)
        {
            throw new Exception($"Strategy {strategyId} not found.");
        }

        AccountInfo? accountInfo = _charlesSchwabRetriever.GetAccountInfo(strategy.AccountId);
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
        _makeTradeContext.trades.Add(trade);
        _makeTradeContext.SaveChanges();
        
        _logger.LogInformation($"Trade generated for {strategyId} and number {response} saved to database.");
        
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

        _logger.LogInformation($"Desired position: {optimalPosition}. Current position: {currentPosition} for {strategy.Id}.");

        if (AlmostEquals.Equals(optimalPosition, currentPosition))
        {
            _logger.LogInformation($"Desired position is close to optimal position.");
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
}