using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Infrastructure.Messaging.Contratos;
using DocumentosFiscais.Infrastructure.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumentosFiscais.Infrastructure.Messaging
{
    public class DocumentoFiscalPublisher : IDocumentoFiscalPublisher
    {
        private readonly IRabbitMqConnection _connection;
        private readonly ConfiguracoesRabbitMQ _settings;

        public DocumentoFiscalPublisher(IRabbitMqConnection connection, IOptions<ConfiguracoesRabbitMQ> options)
        {
            _connection = connection;
            _settings = options.Value;
        }

        public async Task PublicarAsync(DocumentoFiscal message)
        {
            await _connection.InitializeAsync();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var props = new BasicProperties();
             
            await _connection.Channel.BasicPublishAsync(
                _settings.Exchange,
                routingKey: _settings.RoutingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: CancellationToken.None
            );
        }
    }
}
