using DocumentosFiscais.Application.Contracts.Repositories;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.UpdateDocumentoFiscal
{
    public class AtualizarDocumentoFiscalHandler : IRequestHandler<AtualizarDocumentoFiscalCommand, AtualizarDocumentoFiscalResponse>
    {
        private readonly IDocumentoFiscalRepository _repository;

        public AtualizarDocumentoFiscalHandler(IDocumentoFiscalRepository repository)
        {
            _repository = repository;
        }

        public async Task<AtualizarDocumentoFiscalResponse> Handle(AtualizarDocumentoFiscalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingDocument = await _repository.BuscarPorId(request.Id);
                if (existingDocument == null)
                {
                    return new AtualizarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = "Documento fiscal não encontrado."
                    };
                }

                var result = await _repository.Atualizar(request.Id, request.DocumentoFiscal);

                if (result == null)
                {
                    return new AtualizarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = "Falha ao atualizar documento fiscal."
                    };
                }

                return new AtualizarDocumentoFiscalResponse
                {
                    DocumentoFiscal = result,
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