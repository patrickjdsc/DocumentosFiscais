using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Application.Models;
using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Persistence.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DocumentosFiscais.Persistence.Repositories
{
    public class DocumentoFiscalRepository : IDocumentoFiscalRepository
    {
        private readonly IMongoCollection<DocumentoFiscalPersistence> _documentoFiscalCollection;

        public DocumentoFiscalRepository(IMongoDatabase database)
        {
            _documentoFiscalCollection = database.GetCollection<DocumentoFiscalPersistence>("documentosFiscais");
        }

        public Task<DocumentoFiscal> Inserir(DocumentoFiscal documentoFiscal)
        {
            var documento = MapFromEntity(documentoFiscal);
            _documentoFiscalCollection.InsertOne(documento);

            return Task.FromResult(documentoFiscal);
        }

        public async Task<ResultadoComPaginacao<DocumentoFiscal>> ListarDocumentos(int pageNumber, int pageSize, FiltroBuscaDocumentoFiscal? filters = null)
        {
            var filterBuilder = Builders<DocumentoFiscalPersistence>.Filter;
            var filter = filterBuilder.Empty;

            // Apply filters
            if (filters != null)
            {
                if (filters.DataInicio.HasValue)
                    filter &= filterBuilder.Gte(x => x.DataEmissao, filters.DataInicio.Value);

                if (filters.DataFim.HasValue)
                    filter &= filterBuilder.Lte(x => x.DataEmissao, filters.DataFim.Value);

                if (!string.IsNullOrEmpty(filters.CnpjEmitente))
                    filter &= filterBuilder.Regex(x => x.Emitente, new MongoDB.Bson.BsonRegularExpression(filters.CnpjEmitente, "i"));

                if (!string.IsNullOrEmpty(filters.UfEmitente))
                    filter &= filterBuilder.Regex(x => x.Emitente, new MongoDB.Bson.BsonRegularExpression(filters.UfEmitente, "i"));

                if (!string.IsNullOrEmpty(filters.Tipo))
                    filter &= filterBuilder.Eq(x => x.Tipo, filters.Tipo);
            }

            var totalCount = await _documentoFiscalCollection.CountDocumentsAsync(filter);
            
            var documents = await _documentoFiscalCollection
                .Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .SortByDescending(x => x.DataEmissao)
                .ToListAsync();

            var entities = documents.Select(MapToEntity).ToList();

            return new ResultadoComPaginacao<DocumentoFiscal>
            {
                Documentos = entities,
                Total = (int)totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<DocumentoFiscal?> BuscarPorId(string id)
        {
            var objectId = new ObjectId(id);

            var document = await _documentoFiscalCollection
                .Find(x => x.Id == objectId)
                .FirstOrDefaultAsync();

            return document != null ? MapToEntity(document) : null;
        }

        public async Task<DocumentoFiscal?> Atualizar(string id, DocumentoFiscal documentoFiscal)
        {
            var updateDocument = MapFromEntity(documentoFiscal);

            var objectId = new ObjectId(id);

            updateDocument.Id = objectId;

            var result = await _documentoFiscalCollection
                .ReplaceOneAsync(x => x.Id == objectId, updateDocument);

            return result.ModifiedCount > 0 ? documentoFiscal : null;
        }

        public async Task<bool> Deletar(string id)
        {
            var objectId = new ObjectId(id);

            var result = await _documentoFiscalCollection
                .DeleteOneAsync(x => x.Id == objectId);

            return result.DeletedCount > 0;
        }

        public async Task<bool> ExisteDocumentoFiscal(string id)
        {
            var objectId = new ObjectId(id);
            var count = await _documentoFiscalCollection
                .CountDocumentsAsync(x => x.Id.ToString() == id);

            return count > 0;
        }

        private DocumentoFiscalPersistence MapFromEntity(DocumentoFiscal documentoFiscal)
        {
            return new DocumentoFiscalPersistence
            {
                Id = new ObjectId(documentoFiscal.Id),
                Tipo = documentoFiscal.Tipo,
                Chave = documentoFiscal.Chave,
                Destinatario = documentoFiscal.Destinatario,
                Emitente = documentoFiscal.CnpjEmitente,
                DataEmissao = documentoFiscal.DataEmissao,
                ValorTotal = documentoFiscal.ValorTotal,
                Numero = documentoFiscal.Numero,
                Serie = documentoFiscal.Serie,
                Raw = documentoFiscal.Raw
            };
        }

        private DocumentoFiscal MapToEntity(DocumentoFiscalPersistence document)
        {
            return new DocumentoFiscal
            {
                Id = document.Id.ToString(),
                Tipo = document.Tipo,
                Chave = document.Chave,
                Destinatario = document.Destinatario,
                CnpjEmitente = document.Emitente,
                DataEmissao = document.DataEmissao,
                ValorTotal = document.ValorTotal,
                Numero = document.Numero,
                Serie = document.Serie,
                Raw = document.Raw
            };
        }

        public async Task<DocumentoFiscal?> ListarDocumentoPorChave(string chave)
        {
            var document = await _documentoFiscalCollection
                .Find(x => x.Chave == chave)
                .FirstOrDefaultAsync();

            return document != null ? MapToEntity(document) : null;
        }
    }
}
