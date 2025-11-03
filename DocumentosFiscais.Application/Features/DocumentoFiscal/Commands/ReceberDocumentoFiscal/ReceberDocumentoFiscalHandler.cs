using DocumentosFiscais.Application.Contracts.Services;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.ReceberDocumentoFiscal
{
    public class ReceberDocumentoFiscalHandler(IProcessarXmlDocumentoFiscal processarXmlDocumentoFiscal)

        : IRequestHandler<ReceberDocumentoFiscalCommand, ReceberDocumentoFiscalResponse>
    {

        private readonly IProcessarXmlDocumentoFiscal _processarXmlDocumentoFiscal = processarXmlDocumentoFiscal;


        public async Task<ReceberDocumentoFiscalResponse> Handle(ReceberDocumentoFiscalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var resultado = await _processarXmlDocumentoFiscal.Processar(request.ArquivoRecebido);

                if (resultado is null)
                {
                    return new ReceberDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = "Documento fiscal inválido ou não suportado."
                    };

                }
                else
                {
                    return new ReceberDocumentoFiscalResponse
                    {
                        Sucesso = true,
                        DocumentoFiscal = resultado
                    };
                }
            }
            catch (Exception ex)
            {
                return new ReceberDocumentoFiscalResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao processar o documento fiscal: {ex.Message}"
                };
            } 
        }
    }
}
