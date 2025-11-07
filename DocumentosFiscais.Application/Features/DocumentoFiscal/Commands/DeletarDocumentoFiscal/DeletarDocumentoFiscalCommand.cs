using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.DeleteDocumentoFiscal
{
    public class DeletarDocumentoFiscalCommand : IRequest<DeletarDocumentoFiscalResponse>
    {
        public string Id { get; set; }
    }
}