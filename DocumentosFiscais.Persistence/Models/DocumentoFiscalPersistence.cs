using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes; 

namespace DocumentosFiscais.Persistence.Models
{
    public class DocumentoFiscalPersistence
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Tipo { get; set; }
        public string Chave { get; set; }
        public string Destinatario { get; set; }
        public string Emitente { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }
        public string Raw { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
