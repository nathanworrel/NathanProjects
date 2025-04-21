using System.Diagnostics;
using CommonServices.Retrievers.DbAccess;
using CommonServices.Retrievers.GetData;
using FishyBacktesty.Models;
using FishyLibrary.Backtest;
using FishyLibrary.Enums;
using FishyLibrary.Models;
using FishyLibrary.Helpers;
using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Strategies;
using FishyLibrary.Utils;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using YahooFinanceApi;

namespace FishyBacktesty.Components.Pages;

public class ProductParameter
{
    public string Product { get; set; } = "";
}

public partial class Backtest : ComponentBase
{
    public List<ProductParameter> Products { get; set; }
    public TimeSpan? TimeOfDay {get; set;}
    public DateTime? StartDate {get; set;}
    private IStrategy _selectedStrategy;
    private List<EarningBacktest> Efficiencies { get; set; }
    private List<RangedParameter> RangedParameters { get; set; }
    private List<IStrategy> Strategies { get; set; }
    private List<string> StrategieNames { get; set; }
    private HashSet<EarningBacktest> SelectedBacktests { get; set; }
    private string[] TimeRange { get; set; }
    private ChartOptions ChartOptions { get; set; }
    [Inject]
    private IGetDataRetriever GetDataRetriever { get; set; }
    [Inject]
    private IDbAccessRetriever DbAccessRetriever { get; set; }
    private int DaysReduced { get; set; }
    private int NumSplits { get; set; }
    
    private void OnSelectionChanged(HashSet<EarningBacktest> selected)
    {
        SelectedBacktests = selected;
        StateHasChanged(); 
    }
    
    private List<ChartSeries> BacktestCharts
    {
        get
        {
            var maxReturns = (double)SelectedBacktests.Select(b => b.TotalRtns).Max() * 1.5;
            // Create chart series from the selected backtests
            var backtestCharts = SelectedBacktests.Select(backtest => new ChartSeries
            {
                Name = JsonConvert.SerializeObject(backtest.Parameters.GetParameters().Select(x=>x.Value).ToList()),
                Data = backtest.DailyRtns
                    // .GetRange(daysReduced, backtest.DailyRtns.Count-daysReduced)
                    .Select((_, index) =>
                    {
                        double cumulativeReturn = 1;
                        for (int i = 0; i <= index; i++)
                        {
                            cumulativeReturn *= (double) (backtest.DailyRtns[i].Returns);
                        }

                        return cumulativeReturn;
                    })
                    .ToArray()
            }).ToList();
            if (NumSplits > 1 && SelectedBacktests.Count > 0)
            {
                int increment = SelectedBacktests.First().DailyRtns.Count / NumSplits;
                double[] data = new double[SelectedBacktests.First().DailyRtns.Count];
                for (int i = 1; i <= NumSplits; i++)
                {
                    data[int.Min(i * increment, data.Length - 1)] = maxReturns;
                }
                backtestCharts.Add(
                    new ChartSeries
                    {
                        Name = "Splits",
                        Data = data,
                    }
                );   
            }
            return backtestCharts;
        }
    }
    
    private string _selectedStrategyName;
    
    private string SelectedStrategyName
{
    get => _selectedStrategyName;
    set
    {
        if (_selectedStrategyName != value)
        {
            _selectedStrategyName = value;

            if (Enum.TryParse<StrategyName>(_selectedStrategyName, true, out StrategyName strategyName))
            {
                _selectedStrategy = Strategies[(int)strategyName-1];
            }

            RangedParameters.Clear();
            foreach (var param in _selectedStrategy.Parameters.GetParameters())
            {
                RangedParameters.Add(new RangedParameter(param.Key));
            }
            for (var i = Products.Count; i < _selectedStrategy.NumProducts; i++)
            {
                Products.Add(new ProductParameter());
            }

            for (int i = _selectedStrategy.NumProducts; i < Products.Count; i++)
            {
                Products.RemoveAt(Products.Count - 1);
            }
        }
    }
}
    public Backtest()
    {
        Efficiencies = new List<EarningBacktest>();
        SelectedBacktests = new HashSet<EarningBacktest>();
        RangedParameters = new List<RangedParameter>();
        Strategies = new List<IStrategy>();
        Products = new List<ProductParameter>();
        TimeOfDay = new TimeSpan(0,9,40,0);
        StartDate = new DateTime(2000, 1, 1);
        Strategies.Add(new MacdStrategy());
        Strategies.Add(new ExponentialMAStrategy());
        _selectedStrategy = Strategies.First();
        ChartOptions = new ChartOptions();
        ChartOptions.YAxisTicks = 1;
        StrategieNames = new List<string>();

        TimeRange = new string[1];
        foreach (StrategyName name in Enum.GetValues(typeof(StrategyName)))
        {
            StrategieNames.Add(name.ToString());
        }
    }

    private async void RunBacktest()
    {
        try
        {
            Console.WriteLine("Getting Data");
            List<List<StockData>> stockData = await GetDataRetriever
                .GetBacklogDataInternal(
                    TimeOfDay ?? TimeSpan.Zero, 
                    StartDate ?? DateTime.Now, 
                    Products.Select(x => x.Product).ToList());
            Console.WriteLine($"{stockData[0].Count} rows acquired for primary data");
            BacktestBuilder builder = new BacktestBuilder(_selectedStrategy, stockData);
            RangedParameters.ForEach(r => builder.AddRangedParameter(r));
            
            BacktestRunner test = builder.Build();
            Console.WriteLine("Running backtest...");
            test.RunBacktest(DaysReduced, NumSplits);
            Console.WriteLine("Finished backtest");
            Efficiencies = test.BacktestEfficiencies;
            SelectedBacktests.Clear();
            SelectedBacktests.Add(Efficiencies.First());
            int year = -1;
            TimeRange = stockData[0]
                .GetRange(DaysReduced, stockData[0].Count-DaysReduced)
                .Select(l =>
                {
                    if (l.Time.Year != year)
                    {
                        year = l.Time.Year;
                        return l.Time.Year.ToString();
                    }
                    return "";
                })
                .ToArray();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine(ex.Message);
            Debug.WriteLine(ex);
        }
    }

    private async void AddParamsToDb()
    {
        int strategyTypeId = 0;
        foreach (StrategyName name in Enum.GetValues(typeof(StrategyName)))
        {
            if (name.ToString() == SelectedStrategyName)
            {
                strategyTypeId = (int) name;
            }
        }
        foreach (var selectedBacktest in SelectedBacktests)
        {
            if (!IsMarketParams(selectedBacktest.Parameters))
            {
                selectedBacktest.Parameters.StrategyTypeId = strategyTypeId;
                Console.WriteLine($"Adding Parameters to Db");
                var successful = await DbAccessRetriever.AddParameters(selectedBacktest.Parameters);
                Console.WriteLine($"Success: {successful}");   
            }
        }
    }

    private bool IsMarketParams(Parameters parameters)
    {
        foreach (var paramsValue in parameters.Params.Values)
        {
            if (paramsValue is not null && !AlmostEquals.Equals(paramsValue.Value, -1))
            {
                return false;
            }
        }
        
        return true;
    }
}
