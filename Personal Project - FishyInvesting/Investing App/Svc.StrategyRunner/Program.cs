using CommonServices.Retrievers.CharlesSchwab;
using CommonServices.Retrievers.GetData;
using CommonServices.Retrievers.Scheduler;
using CommonServices.Retrievers.UpdateTrades;
using FishyLibrary.Extensions;
using Serilog;
using SerilogTracing;
using Svc.StrategyRunner;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<StrategyRunner>();
builder.Services.AddSerilogWithSeq();
_ = new ActivityListenerConfiguration()
    .TraceToSharedLogger();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<ICharlesSchwabRetriever>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    ILogger<CharlesSchwabRetriever> logger = provider.GetRequiredService<ILogger<CharlesSchwabRetriever>>();
    return new CharlesSchwabRetriever(httpClientFactory, builder.Environment.EnvironmentName, logger);
});
builder.Services.AddSingleton<IGetDataRetriever>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    ILogger<GetDataRetriever> logger = provider.GetRequiredService<ILogger<GetDataRetriever>>();
    return new GetDataRetriever(httpClientFactory, builder.Environment.EnvironmentName, logger);
});
builder.Services.AddSingleton<IUpdateTradesRetriever>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    ILogger<UpdateTradesRetriever> logger = provider.GetRequiredService<ILogger<UpdateTradesRetriever>>();
    return new UpdateTradesRetriever(httpClientFactory, builder.Environment.EnvironmentName, logger);
});
builder.Services.AddSingleton<ISchedulerRetriever>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    ILogger<SchedulerRetriever> logger = provider.GetRequiredService<ILogger<SchedulerRetriever>>();
    return new SchedulerRetriever(httpClientFactory, builder.Environment.EnvironmentName, logger);
});

var host = builder.Build();
host.Run();