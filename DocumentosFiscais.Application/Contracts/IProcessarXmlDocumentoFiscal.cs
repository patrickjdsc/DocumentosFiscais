using DocumentosFiscais.Domain.Entities;

namespace DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal
{
    public interface IProcessarXmlDocumentoFiscal
    {
        Task<DocumentoFiscal> Processar(string xml);
    }
}