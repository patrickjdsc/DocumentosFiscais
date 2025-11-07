using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.GetDocumentoFiscalById
{
    public class ListarDocumentoPorIdQuery : IRequest<ListarDocumentoPorIdResponse>
    {
        public string Id { get; set; }
    }
}