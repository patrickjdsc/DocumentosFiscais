using MediatR; 

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.ReceberDocumentoFiscal
{
    public class ReceberDocumentoFiscalCommand : IRequest<ReceberDocumentoFiscalResponse>
    {
        public string ArquivoRecebido { get; set; }
    }
}
