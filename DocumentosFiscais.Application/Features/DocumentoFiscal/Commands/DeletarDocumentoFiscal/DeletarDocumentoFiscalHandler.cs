using DocumentosFiscais.Application.Contracts.Repositories;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.DeleteDocumentoFiscal
{
    public class DeletarDocumentoFiscalHandler : IRequestHandler<DeletarDocumentoFiscalCommand, DeletarDocumentoFiscalResponse>
    {
        private readonly IDocumentoFiscalRepository _repository;

        public DeletarDocumentoFiscalHandler(IDocumentoFiscalRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeletarDocumentoFiscalResponse> Handle(DeletarDocumentoFiscalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existe = await _repository.ExisteDocumentoFiscal(request.Id);

                if (!existe)
                {
                    return new DeletarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = "Documento fiscal não encontrado."
                    };
                }

                var sucessoDeletar = await _repository.Deletar(request.Id);

                if (sucessoDeletar)
                {
                    return new DeletarDocumentoFiscalResponse
                    {
                        Sucesso = true,
                        Mensagem = "Documento fiscal excluído com sucesso."
                    };
                }
                else
                {
                    return new DeletarDocumentoFiscalResponse
                    {
                        Sucesso = false,
                        Mensagem = "Falha ao excluir documento fiscal."
                    };
                }
            }
            catch (Exception ex)
            {
                return new DeletarDocumentoFiscalResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao excluir documento fiscal: {ex.Message}"
                };
            }
        }
    }
}