using RabbitMQ.Client;

namespace DocumentosFiscais.Infrastructure.Messaging.Contratos
{
    public interface IRabbitMqConnection
    {
        IChannel Channel { get; } 
        ValueTask DisposeAsync();
        Task InitializeAsync();
    }
}