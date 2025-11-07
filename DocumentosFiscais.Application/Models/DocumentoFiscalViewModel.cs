namespace DocumentosFiscais.Application.Models
{
    public class DocumentoFiscalViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Chave { get; set; } = string.Empty;
        public string Destinatario { get; set; } = string.Empty;
        public string CnpjEmitente { get; set; } = string.Empty;
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
    }
}