namespace DocumentosFiscais.Application.Models
{
    public class FiltroBuscaDocumentoFiscal
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? CnpjEmitente { get; set; }
        public string? UfEmitente { get; set; }
        public string? Tipo { get; set; }
    }
}