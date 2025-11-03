using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Persistence.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection.Metadata;

namespace DocumentosFiscais.Persistence.Repositories
{
    public class DocumentFiscalRepository : IDocumentoFiscalRepository
    {
        private readonly IMongoCollection<DocumentoFiscalPersistence> _documentoFiscalCollection;

        public DocumentFiscalRepository(IMongoDatabase database)
        {
            _documentoFiscalCollection = database.GetCollection<DocumentoFiscalPersistence>("documentosFiscais");
        }

        public Task<DocumentoFiscal> Inserir(DocumentoFiscal documentoFiscal)
        {
            var documento = MapFromEntity(documentoFiscal);
            _documentoFiscalCollection.InsertOne(documento);

            return Task.FromResult(documentoFiscal);
        }


        private DocumentoFiscalPersistence MapFromEntity(DocumentoFiscal documentoFiscal)
        {
            return new DocumentoFiscalPersistence
            {
                Id = documentoFiscal.Id,
                Tipo = documentoFiscal.Tipo,
                Chave = documentoFiscal.Chave,
                Destinatario = documentoFiscal.Destinatario,
                Emitente = documentoFiscal.Emitente,
                DataEmissao = documentoFiscal.DataEmissao,
                ValorTotal = documentoFiscal.ValorTotal,
                Raw = documentoFiscal.Raw
            };
        }
    }
}
