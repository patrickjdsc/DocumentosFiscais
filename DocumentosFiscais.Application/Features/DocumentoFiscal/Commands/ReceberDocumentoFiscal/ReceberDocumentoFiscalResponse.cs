namespace DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.ReceberDocumentoFiscal
{
    public class ReceberDocumentoFiscalResponse
    {
        public Domain.Entities.DocumentoFiscal DocumentoFiscal { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}
