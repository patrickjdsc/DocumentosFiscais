using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Application.Models;

namespace DocumentosFiscais.Application.Contracts.Repositories
{
    public interface IDocumentoFiscalRepository
    {
        Task<DocumentoFiscal> Inserir(DocumentoFiscal documentoFiscal);
        Task<ResultadoComPaginacao<DocumentoFiscal>> ListarDocumentos(int pageNumber, int pageSize, FiltroBuscaDocumentoFiscal? filters = null);
        Task<DocumentoFiscal?> ListarDocumentoPorChave(string chave);
        Task<DocumentoFiscal?> BuscarPorId(string id);
        Task<DocumentoFiscal?> Atualizar(string id, DocumentoFiscal documentoFiscal);
        Task<bool> Deletar(string id);
        Task<bool> ExisteDocumentoFiscal(string id);
    }
}
