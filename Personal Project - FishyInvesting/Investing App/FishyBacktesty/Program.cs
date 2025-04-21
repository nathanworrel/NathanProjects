using CommonServices.Retrievers.DbAccess;
using CommonServices.Retrievers.GetData;
using MudBlazor.Services;
using FishyBacktesty.Components;
using FishyLibrary.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Logging.RouteLogger();
builder.Services.AddSingleton<IGetDataRetriever>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    ILogger<GetDataRetriever> logger = provider.GetRequiredService<ILogger<GetDataRetriever>>();
    return new GetDataRetriever(httpClientFactory, "Staging", logger);
});
builder.Services.AddSingleton<IDbAccessRetriever>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    ILogger<DbAccessRetriever> logger = provider.GetRequiredService<ILogger<DbAccessRetriever>>();
    return new DbAccessRetriever(httpClientFactory, "Staging", logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();