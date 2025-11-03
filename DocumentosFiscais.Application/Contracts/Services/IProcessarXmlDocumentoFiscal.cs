using DocumentosFiscais.Domain.Entities;

namespace DocumentosFiscais.Application.Contracts.Services
{
    public interface IProcessarXmlDocumentoFiscal
    {
        Task<DocumentoFiscal> Processar(string xml);
    }
}