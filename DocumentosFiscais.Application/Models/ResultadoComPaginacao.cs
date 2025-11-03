namespace DocumentosFiscais.Application.Models
{
    public class ResultadoComPaginacao<T>
    {
        public IEnumerable<T> Documentos { get; set; } = new List<T>();
        public int Total { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}