using EasyNetQ;
using FishyLibrary.Extensions;
using Svc.Notifications;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.CustomAddEasyNetQ();

var host = builder.Build();
host.Run();