using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.DeleteDocumentoFiscal
{
    public class DeletarDocumentoFiscalCommand : IRequest<DeletarDocumentoFiscalResponse>
    {
        public Guid Id { get; set; }
    }
}