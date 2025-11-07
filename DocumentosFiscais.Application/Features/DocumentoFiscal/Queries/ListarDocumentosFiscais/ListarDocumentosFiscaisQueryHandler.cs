using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Application.Models;
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

                var documentosComLinks = result.Documentos.Select(documento => new DocumentoFiscalViewModel
                {
                    Id = documento.Id,
                    Tipo = documento.Tipo,
                    Chave = documento.Chave,
                    Destinatario = documento.Destinatario,
                    CnpjEmitente = documento.CnpjEmitente,
                    DataEmissao = documento.DataEmissao,
                    ValorTotal = documento.ValorTotal,
                    Numero = documento.Numero,
                    Serie = documento.Serie
                }).ToList();

                var resultadoComLinks = new ResultadoComPaginacao<DocumentoFiscalViewModel>
                {
                    Documentos = documentosComLinks,
                    Total = result.Total,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                };

                return new ListarDocumentosFiscaisResponse
                {
                    DocumentosFiscais = resultadoComLinks,
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