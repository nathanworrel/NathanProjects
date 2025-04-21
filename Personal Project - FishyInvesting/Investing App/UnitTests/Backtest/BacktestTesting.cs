using FishyLibrary.Backtest;
using FishyLibrary.Models;
using FishyLibrary.Helpers;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Strategies;

namespace UnitTests;

public class BacktestTesting
{
    [Fact]
    public void GenerateParameterCombinations_OneRangedParameter()
    {
        BacktestRunner backtestRunner = new BacktestRunner();
        backtestRunner.RangedParameters.Add("Param",
            new RangedParameter("Param", 1, 5, 1));
        List<Parameters> received = backtestRunner.GenerateParameterCombinations();
        Assert.Equal(1, received[0].GetParameter("Param"));
        Assert.Equal(2, received[1].GetParameter("Param"));
        Assert.Equal(3, received[2].GetParameter("Param"));
        Assert.Equal(4, received[3].GetParameter("Param"));
        Assert.Equal(5, received[4].GetParameter("Param"));
    }

    [Fact]
    public void GenerateParameterCombinations_MultipleRangedParameters()
    {
        BacktestRunner backtestRunner = new BacktestRunner();
        backtestRunner.RangedParameters.Add("Param 1",
            new RangedParameter("Param 1", 1, 2, 1));
        backtestRunner.RangedParameters.Add("Param 2",
            new RangedParameter("Param 2", -10, 5, 10));
        backtestRunner.RangedParameters.Add("Param 3",
            new RangedParameter("Param 3", 10, 12, 2));
        List<Parameters> received = backtestRunner.GenerateParameterCombinations();
        Assert.Equal(8, received.Count);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 1 &&
                                          item.GetParameter("Param 2") == -10 && item.GetParameter("Param 3") == 10);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 1 &&
                                          item.GetParameter("Param 2") == -10 && item.GetParameter("Param 3") == 12);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 1 &&
                                          item.GetParameter("Param 2") == 0 && item.GetParameter("Param 3") == 10);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 1 &&
                                          item.GetParameter("Param 2") == 0 && item.GetParameter("Param 3") == 12);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 2 &&
                                          item.GetParameter("Param 2") == -10 && item.GetParameter("Param 3") == 10);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 2 &&
                                          item.GetParameter("Param 2") == -10 && item.GetParameter("Param 3") == 12);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 2 &&
                                          item.GetParameter("Param 2") == 0 && item.GetParameter("Param 3") == 10);
        Assert.Contains(received, item => item.GetParameter("Param 1") == 2 &&
                                          item.GetParameter("Param 2") == 0 && item.GetParameter("Param 3") == 12);
    }

    [Fact]
    public void GenerateParameterCombinations_OneSetParameter()
    {
        BacktestRunner backtestRunner = new BacktestRunner();
        backtestRunner.RangedParameters.Add("Param 1",
            new RangedParameter("Param 1", 1, 1, 1));
        List<Parameters> received = backtestRunner.GenerateParameterCombinations();
        Assert.Equal(1, received[0].GetParameter("Param 1"));
    }

    [Fact]
    public void GenerateParameterCombinations_MultipleSetParameters()
    {
        BacktestRunner backtestRunner = new BacktestRunner();
        backtestRunner.RangedParameters.Add("Param 1",
            new RangedParameter("Param 1", 1, 1, 1));
        backtestRunner.RangedParameters.Add("Param 2",
            new RangedParameter("Param 2", 2, 2, 1));
        backtestRunner.RangedParameters.Add("Param 3",
            new RangedParameter("Param 3", -5, -5, 1));
        List<Parameters> received = backtestRunner.GenerateParameterCombinations();
        Assert.Equal(1, received[0].GetParameter("Param 1"));
        Assert.Equal(2, received[0].GetParameter("Param 2"));
        Assert.Equal(-5, received[0].GetParameter("Param 3"));
        Assert.Equal(-5, received[0].GetParameter("Param 3"));
    }

    [Fact]
    public void BacktestMovingAverage()
    {
        List<StockData> stockData = new List<StockData>();
        stockData.Add(new StockData("", new DateTime(), 1));
        stockData.Add(new StockData("", new DateTime(), 1));
        stockData.Add(new StockData( "", new DateTime(), 5));
        stockData.Add(new StockData("", new DateTime(), (decimal)2.5));
        stockData.Add(new StockData("", new DateTime(), 5));
        stockData.Add(new StockData( "", new DateTime(), 1));
        MovingAverageStrategy strategy = new MovingAverageStrategy();
        BacktestRunner backtestRunner = new BacktestRunner(strategy, [stockData]);
        backtestRunner.RangedParameters.Add(nameof(MovingAverageStrategy.Period), new RangedParameter(nameof(MovingAverageStrategy.Period), 1, 3, 1));
        backtestRunner.RangedParameters.Add(nameof(MovingAverageStrategy.Period2), new RangedParameter(nameof(MovingAverageStrategy.Period2), 1, 1, 1));
        backtestRunner.RunBacktest(0, 0);
        Assert.Equal(4, backtestRunner.BacktestEfficiencies.Count);
        Assert.Equal(1, backtestRunner.BacktestEfficiencies[1].TotalRtns);
        Assert.Equal(2, (double)backtestRunner.BacktestEfficiencies[2].TotalRtns, 0.001);
        Assert.Equal(2, (double)backtestRunner.BacktestEfficiencies[3].TotalRtns, 0.001);
    }

    [Fact]
    public void BacktestMACD()
    {
        List<StockData> stockData = new List<StockData>();
        stockData.Add(new StockData("", new DateTime(), 1));
        stockData.Add(new StockData("", new DateTime(), 1));
        stockData.Add(new StockData( "", new DateTime(), 5));
        stockData.Add(new StockData("", new DateTime(), (decimal)2.5));
        stockData.Add(new StockData("", new DateTime(), 5));
        stockData.Add(new StockData( "", new DateTime(), 1));
        MacdStrategy strategy = new MacdStrategy();
        BacktestRunner backtestRunner = new BacktestRunner(strategy, [stockData]);
        backtestRunner.RangedParameters.Add(nameof(MacdStrategy.PeriodShort), new RangedParameter(nameof(MacdStrategy.PeriodShort), 1, 3, 1));
        backtestRunner.RangedParameters.Add(nameof(MacdStrategy.PeriodLong), new RangedParameter(nameof(MacdStrategy.PeriodLong), 1, 3, 1));
        backtestRunner.RangedParameters.Add(nameof(MacdStrategy.PeriodExponential), new RangedParameter(nameof(MacdStrategy.PeriodExponential), 1, 3, 1));
        backtestRunner.RunBacktest(0, 0);
        Assert.Equal(28, backtestRunner.BacktestEfficiencies.Count);
        Assert.Equal(1, backtestRunner.BacktestEfficiencies[1].TotalRtns);
        Assert.Equal(1, (double)backtestRunner.BacktestEfficiencies[2].TotalRtns, 0.001);
        Assert.Equal(1, (double)backtestRunner.BacktestEfficiencies[3].TotalRtns, 0.001);
        Assert.Equal(1, (double)backtestRunner.BacktestEfficiencies[4].TotalRtns, 0.001);
        Assert.Equal(2, (double)backtestRunner.BacktestEfficiencies[5].TotalRtns, 0.001);
        Assert.Equal(2, (double)backtestRunner.BacktestEfficiencies[6].TotalRtns, 0.001);
        Assert.Equal(1, (double)backtestRunner.BacktestEfficiencies[7].TotalRtns, 0.001);
        Assert.Equal(2, (double)backtestRunner.BacktestEfficiencies[8].TotalRtns, 0.001);
        Assert.Equal(2, (double)backtestRunner.BacktestEfficiencies[9].TotalRtns, 0.001);
        Assert.Equal(1, (double)backtestRunner.BacktestEfficiencies[10].TotalRtns, 0.001);
    }
}