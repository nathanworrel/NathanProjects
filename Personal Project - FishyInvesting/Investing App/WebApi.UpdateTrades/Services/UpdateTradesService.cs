using CommonServices.Retrievers.CharlesSchwab;
using FishyLibrary.Enums;
using FishyLibrary.Models;
using FishyLibrary.Models.Order;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.Trade;
using Microsoft.IdentityModel.Tokens;
using WebApi.UpdateTrades.Contexts;

namespace WebApi.UpdateTrades.Services;

public class UpdateTradesService : IUpdateTradesService
{
    private readonly ILogger<UpdateTradesService> _logger;
    private readonly UpdateTradesContext _updateTradesContext;
    private readonly ICharlesSchwabRetriever _charlesSchwabRetriever;

    public UpdateTradesService(ILogger<UpdateTradesService> logger, UpdateTradesContext updateTradesContext,
        ICharlesSchwabRetriever charlesSchwabRetriever)
    {
        _logger = logger;
        _updateTradesContext = updateTradesContext;
        _charlesSchwabRetriever = charlesSchwabRetriever;
    }

    public void UpdateTrades()
    {
        List<Trade> pendingTrades = _updateTradesContext.Trades.Where(t => t.Status <= 8).ToList();

        if (pendingTrades.IsNullOrEmpty())
        {
            return;
        }

        foreach (var trade in pendingTrades)
        {
            _logger.LogDebug("{trade}", trade);
            _logger.LogInformation("Updating trade {orderNumber} for strategy {strategyId}", trade.OrderNumber, trade.StrategyId);
            Strategy? strategy = _updateTradesContext.strategies.FirstOrDefault(s => s.Id == trade.StrategyId);
            _logger.LogDebug("{strategy}", strategy);
            if (strategy == null)
            {
                _logger.LogError("No strategy found with id: {strategyId}", trade.StrategyId);
                continue;
            }
            OrderData? order = _charlesSchwabRetriever.GetOrder(strategy.AccountId, trade.OrderNumber);
            _logger.LogDebug("{order}", order);
            if (order == null)
            {
                _logger.LogError("No order found with id: {orderNumber}", trade.OrderNumber);
                continue;
            }

            int filled = 0;
            decimal price = 0;
            if (order.OrderActivityCollection != null && order.OrderActivityCollection.Count > 0)
            {
                foreach (var orderActivity in order.OrderActivityCollection)
                {
                    if (orderActivity.ExecutionType == "FILL")
                    {
                        foreach (var leg in orderActivity.ExecutionLegs)
                        {
                            price = (decimal)(((double)price * filled + leg.Price * leg.Quantity) /
                                              (filled + leg.Quantity));
                            filled += (int)leg.Quantity;
                        }
                    }
                }
            }
            
            int quantityChanged = filled - trade.QuantityFilled;
            decimal changeInMoney = (price * filled) - (trade.PriceFilled * trade.QuantityFilled);

            if (trade.Side == (int)Side.SELL)
            {
                strategy.NumStocksHolding -= quantityChanged;
                strategy.AmountAllocated += changeInMoney;
                _logger.LogInformation("Made {changeInMoney} from {orderNumber} for strategy {strategyId}", 
                    changeInMoney, trade.OrderNumber, trade.StrategyId);
            }
            else
            {
                strategy.NumStocksHolding += quantityChanged;
                strategy.AmountAllocated -= changeInMoney;
                _logger.LogInformation("Cost {changeInMoney} from {orderNumber} for strategy {strategyId}", 
                    changeInMoney, trade.OrderNumber, trade.StrategyId);
            }

            trade.QuantityFilled = filled;
            trade.PriceFilled = price;

            Enum.TryParse(order.Status, out TradeStatus status);
            trade.Status = (int)status;
            trade.TimeModified = DateTime.Now.ToUniversalTime();
            _logger.LogDebug("{trade}", trade);
        }

        _updateTradesContext.SaveChanges();
        _logger.LogInformation("Updated trades");
    }
}