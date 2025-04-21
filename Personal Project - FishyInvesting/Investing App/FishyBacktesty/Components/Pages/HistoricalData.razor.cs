using CommonServices.Retrievers.GetData;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FishyBacktesty.Components.Pages;

public partial class HistoricalData : ComponentBase
{
    public string Symbol { get; set; }
    private DateRange? DateRanges { get; set; }
    [Inject]
    private IGetDataRetriever GetDataRetriever { get; set; }

    private async void GetDateRanges()
    {
        var dateRanges = await GetDataRetriever.GetDateRangeInternal(Symbol);
        Console.WriteLine(dateRanges);
        DateRanges = new DateRange(dateRanges.Item1, dateRanges.Item2);
        StateHasChanged();
    }

    private async void AddDateRanges()
    {
        bool finished = await GetDataRetriever.LoadDataForProductInternal(Symbol);
        Console.WriteLine(finished);
        GetDateRanges();
    }
}