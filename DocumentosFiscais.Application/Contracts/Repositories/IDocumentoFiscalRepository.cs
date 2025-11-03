using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Application.Models;

namespace DocumentosFiscais.Application.Contracts.Repositories
{
    public interface IDocumentoFiscalRepository
    {
        Task<DocumentoFiscal> Inserir(DocumentoFiscal documentoFiscal);
        Task<ResultadoComPaginacao<DocumentoFiscal>> ListarDocumentos(int pageNumber, int pageSize, FiltroBuscaDocumentoFiscal? filters = null);
        Task<DocumentoFiscal?> BuscarPorId(Guid id);
        Task<DocumentoFiscal?> Atualizar(Guid id, DocumentoFiscal documentoFiscal);
        Task<bool> Deletar(Guid id);
        Task<bool> ExisteDocumentoFiscal(Guid id);
    }
}
