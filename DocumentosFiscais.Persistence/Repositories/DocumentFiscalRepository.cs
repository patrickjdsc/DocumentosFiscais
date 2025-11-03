using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Domain.Entities;

namespace DocumentosFiscais.Persistence.Repositories
{
    public class DocumentFiscalRepository : IDocumentoFiscalRepository
    {
        public Task<DocumentoFiscal> Inserir(DocumentoFiscal documentoFiscal)
        {
            throw new NotImplementedException();
        }
    }
}
