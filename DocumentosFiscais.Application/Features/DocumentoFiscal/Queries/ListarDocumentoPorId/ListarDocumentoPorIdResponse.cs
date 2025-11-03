namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.GetDocumentoFiscalById
{
    public class ListarDocumentoPorIdResponse
    {
        public Domain.Entities.DocumentoFiscal? DocumentoFiscal { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}