using DocumentosFiscais.Application.Contracts.Repositories;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.GetDocumentoFiscalById
{
    public class ListarDocumentoPorIdHandler : IRequestHandler<ListarDocumentoPorIdQuery, ListarDocumentoPorIdResponse>
    {
        private readonly IDocumentoFiscalRepository _repository;

        public ListarDocumentoPorIdHandler(IDocumentoFiscalRepository repository)
        {
            _repository = repository;
        }

        public async Task<ListarDocumentoPorIdResponse> Handle(ListarDocumentoPorIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var documento = await _repository.BuscarPorId(request.Id);

                if (documento == null)
                {
                    return new ListarDocumentoPorIdResponse
                    {
                        Sucesso = false,
                        Mensagem = "Documento fiscal não encontrado."
                    };
                }

                return new ListarDocumentoPorIdResponse
                {
                    DocumentoFiscal = documento,
                    Sucesso = true,
                    Mensagem = "Sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ListarDocumentoPorIdResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao recuperar documento fiscal: {ex.Message}"
                };
            }
        }
    }
}