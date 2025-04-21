using System.Data;
using FishyBacktesty.Models;
using FishyLibrary.Models;
using FishyLibrary.Helpers;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Trade;
using FishyLibrary.Strategies;

namespace FishyLibrary.Backtest;

public class BacktestRunner
{
    private List<List<StockData>> PrimaryPricingData { get; set; }
    private IStrategy Strategy { get; set; }
    public Dictionary<string, RangedParameter> RangedParameters { get; set; }
    public List<EarningBacktest> BacktestEfficiencies { get; }
    
    public BacktestRunner(IStrategy strategyIn, List<List<StockData>> primaryPricingData)
    {
        PrimaryPricingData = primaryPricingData;
        RangedParameters = new Dictionary<string, RangedParameter>();
        Strategy = strategyIn;
        BacktestEfficiencies = new List<EarningBacktest>();
    }

    public BacktestRunner()
    {
        PrimaryPricingData = new List<List<StockData>>();
        RangedParameters = new Dictionary<string, RangedParameter>();
        BacktestEfficiencies = new List<EarningBacktest>();
    }
    
    public List<Parameters> GenerateParameterCombinations()
    {
        List<Parameters> masterList = new List<Parameters> {new()};
        foreach (RangedParameter param in RangedParameters.Values)
        {
            List<Parameters> newMasterList = new List<Parameters>();
            foreach (var parameters in masterList)
            {
                for (float i = param.StartingValue; i <= param.EndingValue; i += param.StepSize)
                {
                    Parameters tempParameters = new Parameters(parameters);
                    tempParameters.AddParameter(param.Name, i);
                    newMasterList.Add(tempParameters);
                } 
            }
            masterList = newMasterList;
        }
        
        return masterList;
    }
    
    private Trade? GenerateTrade(double desiredAllocation, double tradePrice, DateTime tradeDateTime, double currentAllocation)
     {
         double diff = desiredAllocation - currentAllocation;
         if (Math.Abs(diff) > 0)
         {
             Side side = diff > 0 ? Side.BUY : Side.SELL;
             return new Trade(tradeDateTime, (int) Math.Abs(diff) * 100, (decimal) tradePrice, (int) side, (decimal) desiredAllocation);
         }
         return null;
     }

    public EarningBacktest GenerateEfficiency(Parameters parameters,int daysReduced)
    {
        Strategy.ResetStrategy(parameters);
        List<Return> dailyReturns = new List<Return>();
        Position backtestPosition = new Position();
        double currentAllocation = 0;
        for (int i = 0; i < PrimaryPricingData[0].Count; i++)
        {
            if (i < daysReduced)
            {
                Strategy.GenerateSignal(PrimaryPricingData.Select(data => (double)data[i].Price).ToList());
            }
            else
            {
                dailyReturns.Add(backtestPosition.IncreaseInterval(PrimaryPricingData[0][i]));
                double signal = Strategy.GenerateSignal(PrimaryPricingData.Select(data => (double)data[i].Price).ToList());
                Trade? trade = GenerateTrade(signal, 
                    (double)PrimaryPricingData[0][i].Price,PrimaryPricingData[0][i].Time, currentAllocation);
                if (trade != null)
                {
                    currentAllocation = (double) trade.DesiredAllocation;
                    backtestPosition.AddTradeInInterval(trade);
                }
            }
        }
        return new (dailyReturns, parameters);
    }
    
    public EarningBacktest GenerateBaseEfficiency(Parameters parameters, int daysReduced)
    {
        List<Return> dailyReturns = new List<Return>();
        Position backtestPosition = new Position();
        double currentAllocation = 0;
        for (int i = 0; i < PrimaryPricingData[0].Count; i++)
        {
            if (i >= daysReduced)
            {
                dailyReturns.Add(backtestPosition.IncreaseInterval(PrimaryPricingData[0][i]));
                double signal = 1;
                var trade = GenerateTrade(signal,
                    (double)PrimaryPricingData[0][i].Price, PrimaryPricingData[0][i].Time, currentAllocation);
                if (trade != null)
                {
                    backtestPosition.AddTradeInInterval(trade);
                }

                currentAllocation = signal;
            }
        }
        return new EarningBacktest(dailyReturns, parameters);
    }

    public void RunBacktest(int daysReduced, int numSplits)
    {
        Console.WriteLine("Getting Combinations");
        List<Parameters> masterList = GenerateParameterCombinations();
        Console.WriteLine($"Generated {masterList.Count} combinations");
        Console.WriteLine("Generating Efficiency Data");
        
        Parameters newParameters = new Parameters();
        foreach (var keyValuePair in masterList.First().GetParameters())
        {
            newParameters.AddParameter(keyValuePair.Key, -1);
        }
        var generateBaseEfficiency = GenerateBaseEfficiency(newParameters, daysReduced);
        BacktestEfficiencies.Add(generateBaseEfficiency);
        if (numSplits == 0)
        {
            masterList.ForEach(l => BacktestEfficiencies.Add(GenerateEfficiency(l,daysReduced)));
        }
        else
        {
            Dictionary<Parameters, EarningBacktest> fullEfficienciesList = new Dictionary<Parameters, EarningBacktest>();
            masterList.ForEach(l => fullEfficienciesList.Add(l, GenerateEfficiency(l,daysReduced)));
            fullEfficienciesList.Add(newParameters,generateBaseEfficiency);
            var ammountData = fullEfficienciesList.First().Value.DailyRtns.Count ;
            int increment = ammountData / numSplits;
            for (int i = 0; i < ammountData-increment; i+=increment)
            {
                var maxTotalReturns = fullEfficienciesList.Values
                    .Select(e => new EarningBacktest(e.DailyRtns.GetRange(i, increment), e.Parameters))
                    .MaxBy(e => e.TotalRtns);
                if (maxTotalReturns != null)
                {
                    var returns = fullEfficienciesList[maxTotalReturns.Parameters];
                    returns.MostRecentSplit = (returns.MostRecentSplit == "" ? "" : returns.MostRecentSplit+",") + (i / increment + 1);
                    if (!BacktestEfficiencies.Contains(returns))
                    {
                        BacktestEfficiencies.Add(returns);
                    }
                }
            }   
        }
    }
}