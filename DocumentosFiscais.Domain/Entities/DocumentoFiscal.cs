namespace DocumentosFiscais.Domain.Entities
{
    public class DocumentoFiscal
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } 
        public string Chave { get; set; }
        public string Destinatario { get; set; }
        public string Emitente { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string Raw { get; set; }
    }
}
