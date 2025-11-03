using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Infrastructure.Messaging.Contratos;
using DocumentosFiscais.Infrastructure.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumentosFiscais.Infrastructure.Messaging
{
    internal class DocumentoFiscalConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _connection;
        private readonly ConfiguracoesRabbitMQ _settings;

        public DocumentoFiscalConsumer(IRabbitMqConnection connection, IOptions<ConfiguracoesRabbitMQ> options)
        {
            _connection = connection;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = _connection.Channel;
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) => 
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<DocumentoFiscal>(json);
                     
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                     await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: _settings.Queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );
        } 
    }
} 
