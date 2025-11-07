using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Application.Contracts.Services;
using DocumentosFiscais.Application.Models;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.ReceberDocumentoFiscal
{
    public class ReceberDocumentoFiscalHandler(
        IProcessarXmlDocumentoFiscal processarXmlDocumentoFiscal,
        IDocumentoFiscalRepository documentoFiscalRepository )
        : IRequestHandler<ReceberDocumentoFiscalCommand, ReceberDocumentoFiscalResponse>
    {

        private readonly IProcessarXmlDocumentoFiscal _processarXmlDocumentoFiscal = processarXmlDocumentoFiscal;
        private readonly IDocumentoFiscalRepository _documentoFiscalRepository = documentoFiscalRepository;

        public async Task<ReceberDocumentoFiscalResponse> Handle(ReceberDocumentoFiscalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var documentoProcessado = await _processarXmlDocumentoFiscal.Processar(request.ArquivoRecebido);
                 
                if (documentoProcessado is null)
                {
                    throw new Exception("Documento fiscal inválido ou não suportado.");
                }

                var documentoExistente = await _documentoFiscalRepository.ListarDocumentoPorChave(documentoProcessado.Chave);
                
                if (documentoExistente is not null)
                {
                    return new ReceberDocumentoFiscalResponse
                    {
                        Sucesso = true,
                        DocumentoFiscal = documentoExistente
                    };
                }

                var documentoCriado = await _documentoFiscalRepository.Inserir(documentoProcessado);
                 
                return new ReceberDocumentoFiscalResponse
                {
                    Sucesso = true,
                    DocumentoFiscal = documentoCriado
                };

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
