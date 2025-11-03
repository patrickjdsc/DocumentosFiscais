namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.UpdateDocumentoFiscal
{
    public class AtualizarDocumentoFiscalResponse
    {
        public Domain.Entities.DocumentoFiscal? DocumentoFiscal { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}