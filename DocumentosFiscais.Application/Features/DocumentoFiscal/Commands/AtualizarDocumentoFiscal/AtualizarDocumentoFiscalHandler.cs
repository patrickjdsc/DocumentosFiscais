using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Application.Contracts.Services;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.UpdateDocumentoFiscal
{
    public class AtualizarDocumentoFiscalHandler : IRequestHandler<AtualizarDocumentoFiscalCommand, AtualizarDocumentoFiscalResponse>
    {
        private readonly IDocumentoFiscalRepository _repository;
        private readonly IProcessarXmlDocumentoFiscal _processarXmlService;

        public AtualizarDocumentoFiscalHandler(
            IDocumentoFiscalRepository repository,
            IProcessarXmlDocumentoFiscal processarXmlService)
        {
            _repository = repository;
            _processarXmlService = processarXmlService;
        }

        public async Task<AtualizarDocumentoFiscalResponse> Handle(AtualizarDocumentoFiscalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var documentoExistente = await _repository.BuscarPorId(request.Id);
                if (documentoExistente == null)
                {
                    return new AtualizarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = "Documento fiscal não encontrado."
                    };
                }

                var documentoNovo = await _processarXmlService.Processar(request.ArquivoXml);

                if (!string.Equals(documentoExistente.Tipo, documentoNovo.Tipo, StringComparison.OrdinalIgnoreCase))
                {
                    return new AtualizarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = $"O tipo do documento não pode ser alterado. Tipo original: {documentoExistente.Tipo}, Novo tipo: {documentoNovo.Tipo}."
                    };
                }

                if (!string.Equals(documentoExistente.Chave, documentoNovo.Chave, StringComparison.OrdinalIgnoreCase))
                {
                    return new AtualizarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = $"A chave do documento não pode ser alterada. Chave original: {documentoExistente.Chave}, Nova chave: {documentoNovo.Chave}."
                    };
                }

                documentoNovo.Id = documentoExistente.Id;

                var documentoAtualizado = await _repository.Atualizar(request.Id, documentoNovo);

                if (documentoAtualizado == null)
                {
                    return new AtualizarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = "Falha ao atualizar documento fiscal."
                    };
                }

                return new AtualizarDocumentoFiscalResponse
                {
                    DocumentoFiscal = documentoAtualizado,
                    Sucesso = true,
                    Mensagem = "Documento fiscal atualizado com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new AtualizarDocumentoFiscalResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao atualizar documento fiscal: {ex.Message}"
                };
            }
        }
    }
}