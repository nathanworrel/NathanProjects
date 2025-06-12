using EasyNetQ;
using FishyLibrary;
using FishyLibrary.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Svc.Notifications;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus  _bus;
    private readonly TelegramBotClient bot;

    public Worker(ILogger<Worker> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
        var token = "";
        bot = new TelegramBotClient(token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _bus.PubSub.SubscribeAsync<INotification>("id", msg => SendNotification(msg));
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    public void SendNotification(INotification notification)
    {
        Chat chat = new Chat();
        chat.Id = notification.ChatId;
        bot.SendMessage(chat, notification.Message);
    }
}