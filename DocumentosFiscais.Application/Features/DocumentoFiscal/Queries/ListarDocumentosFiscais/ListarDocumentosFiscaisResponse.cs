using DocumentosFiscais.Application.Models;

namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.ListarDocumentosFiscais
{
    public class ListarDocumentosFiscaisResponse
    {
        public ResultadoComPaginacao<Domain.Entities.DocumentoFiscal> DocumentosFiscais { get; set; } = new();
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}