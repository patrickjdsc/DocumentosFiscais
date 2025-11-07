using DocumentosFiscais.Domain.Entities;

namespace DocumentosFiscais.Infrastructure.Messaging.Contratos
{
    public interface IDocumentoFiscalPublisher
    {
        Task PublicarAsync(DocumentoFiscal message);
    }
}