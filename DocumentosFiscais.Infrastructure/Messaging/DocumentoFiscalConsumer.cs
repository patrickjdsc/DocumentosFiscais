using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Infrastructure.Messaging.Contratos;
using DocumentosFiscais.Infrastructure.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DocumentosFiscais.Infrastructure.Messaging
{
    public class DocumentoFiscalConsumer : BackgroundService
    {
        private readonly IRabbitMqConnection _conn;
        private readonly ConfiguracoesRabbitMQ _settings;
        private readonly ILogger<DocumentoFiscalConsumer> _logger;

        public DocumentoFiscalConsumer(
            IRabbitMqConnection conn, 
            IOptions<ConfiguracoesRabbitMQ> opts,
            ILogger<DocumentoFiscalConsumer> logger)
        {
            _conn = conn;
            _settings = opts.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
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

                    await ProcessarDocumentoFiscal(msg);
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };


            await channel.BasicConsumeAsync(queue: _settings.Queue, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
            }
            catch(Exception ex)
            {

            }
        }

        private Task ProcessarDocumentoFiscal(DocumentoFiscal msg) {
            /* ... */
            return Task.CompletedTask; 
        }
    }
}
