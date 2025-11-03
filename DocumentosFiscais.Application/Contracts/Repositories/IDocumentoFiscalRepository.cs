using DocumentosFiscais.Domain.Entities;

namespace DocumentosFiscais.Application.Contracts.Repositories
{
    public interface IDocumentoFiscalRepository
    {
        Task<DocumentoFiscal> Inserir(DocumentoFiscal documentoFiscal);
    }
}
