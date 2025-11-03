using DocumentosFiscais.Application.Models;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.ListarDocumentosFiscais
{
    public class ListarDocumentosFiscaisQuery : IRequest<ListarDocumentosFiscaisResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public FiltroBuscaDocumentoFiscal FiltroBuscaDocumentoFiscal { get; set; }
    }
}