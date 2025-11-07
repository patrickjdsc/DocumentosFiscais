using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.UpdateDocumentoFiscal
{
    public class AtualizarDocumentoFiscalCommand : IRequest<AtualizarDocumentoFiscalResponse>
    {
        public string Id { get; set; } = string.Empty;
        public string ArquivoXml { get; set; } = string.Empty;
    }
}