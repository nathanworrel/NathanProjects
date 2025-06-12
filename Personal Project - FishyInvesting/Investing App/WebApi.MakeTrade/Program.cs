using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommonServices;
using EasyNetQ;
using FishyLibrary.Extensions;
using MakeTrade;
using Serilog;
using SerilogTracing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSerilogWithSeq();

_ = new ActivityListenerConfiguration()
    .TraceToSharedLogger();

// Use Autofac as the DI container
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddAutoMapper(typeof(FishyLibrary.Models.AutoMapperProfile));
// Autofac module configuration
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<ProgramRegistry>();
    containerBuilder.RegisterModule(new CommonRegistry(builder.Environment.EnvironmentName));
});

builder.Services.CustomAddEasyNetQ();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();