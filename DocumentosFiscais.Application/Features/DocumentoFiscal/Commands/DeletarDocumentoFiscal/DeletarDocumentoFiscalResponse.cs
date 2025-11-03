namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.DeleteDocumentoFiscal
{
    public class DeletarDocumentoFiscalResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}