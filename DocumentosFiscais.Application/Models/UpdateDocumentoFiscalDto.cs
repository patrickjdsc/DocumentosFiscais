using System.ComponentModel.DataAnnotations;

namespace DocumentosFiscais.Application.Models
{
    public class UpdateDocumentoFiscalDto
    {
        [Required]
        public string Tipo { get; set; } = string.Empty;
        
        [Required]
        public string Chave { get; set; } = string.Empty;
        
        public string? Destinatario { get; set; }
        
        public string? Emitente { get; set; }
        
        [Required]
        public DateTime DataEmissao { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal ValorTotal { get; set; }
    }
}