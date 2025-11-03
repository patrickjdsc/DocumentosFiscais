using DocumentosFiscais.Infrastructure.Messaging.Contratos;
using DocumentosFiscais.Infrastructure.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DocumentosFiscais.Infrastructure.Messaging
{
    public class RabbitMqConnection : IAsyncDisposable, IRabbitMqConnection
    {
        private readonly ConfiguracoesRabbitMQ _settings;
        private IConnection? _connection;
        private IChannel? _channel;

        public IChannel Channel => _channel ?? throw new InvalidOperationException("Canal não inicializado.");

        public RabbitMqConnection(IOptions<ConfiguracoesRabbitMQ> options)
        {
            _settings = options.Value;
        }

        public async Task InitializeAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(_settings.Exchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(_settings.Queue, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(_settings.Queue, _settings.Exchange, _settings.RoutingKey);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}
