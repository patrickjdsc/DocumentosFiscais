using DocumentosFiscais.Application.Contracts.Repositories;
using MediatR;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.ListarDocumentosFiscais
{
    public class ListarDocumentosFiscaisQueryHandler : IRequestHandler<ListarDocumentosFiscaisQuery, ListarDocumentosFiscaisResponse>
    {
        private readonly IDocumentoFiscalRepository _repository;

        public ListarDocumentosFiscaisQueryHandler(IDocumentoFiscalRepository repository)
        {
            _repository = repository;
        }

        public async Task<ListarDocumentosFiscaisResponse> Handle(ListarDocumentosFiscaisQuery request, CancellationToken cancellationToken)
        {
            try
            { 
                var result = await _repository.ListarDocumentos(request.PageNumber, request.PageSize, request.FiltroBuscaDocumentoFiscal);

                return new ListarDocumentosFiscaisResponse
                {
                    DocumentosFiscais = result,
                    Sucesso = true,
                 };
            }
            catch (Exception ex)
            {
                return new ListarDocumentosFiscaisResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao recuperar documentos fiscais: {ex.Message}"
                };
            }
        }
    }
}