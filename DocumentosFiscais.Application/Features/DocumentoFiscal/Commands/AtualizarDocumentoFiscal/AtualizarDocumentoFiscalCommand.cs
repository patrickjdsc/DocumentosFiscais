using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.UpdateDocumentoFiscal
{
    public class AtualizarDocumentoFiscalCommand : IRequest<AtualizarDocumentoFiscalResponse>
    {
        public string Id { get; set; }
        public Domain.Entities.DocumentoFiscal DocumentoFiscal { get; set; } = new();
    }
}