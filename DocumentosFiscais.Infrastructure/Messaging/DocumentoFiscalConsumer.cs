using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Infrastructure.Messaging.Contratos;
using DocumentosFiscais.Infrastructure.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DocumentosFiscais.Infrastructure.Messaging
{
    public class DocumentoFiscalConsumer : BackgroundService
    {
        private readonly IRabbitMqConnection _conn;
        private readonly ConfiguracoesRabbitMQ _settings;

        public DocumentoFiscalConsumer(IRabbitMqConnection conn, IOptions<ConfiguracoesRabbitMQ> opts)
        {
            _conn = conn;
            _settings = opts.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _conn.InitializeAsync();
            var channel = _conn.Channel;
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var bytes = ea.Body.ToArray();
                    var msg = System.Text.Json.JsonSerializer.Deserialize<DocumentoFiscal>(bytes)!;

                    await ProcessMessageAsync(msg);
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };


            await channel.BasicConsumeAsync(queue: _settings.Queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        }

        private Task ProcessMessageAsync(DocumentoFiscal msg) { /* ... */ return Task.CompletedTask; }
    }
}
