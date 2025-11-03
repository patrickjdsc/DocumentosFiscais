using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentosFiscais.Persistence.Models
{
    public class DocumentoFiscalPersistence
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; }

        public string Tipo { get; set; }
        public string Chave { get; set; }
        public string Destinatario { get; set; }
        public string Emitente { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorTotal { get; set; }
        public string Raw { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
