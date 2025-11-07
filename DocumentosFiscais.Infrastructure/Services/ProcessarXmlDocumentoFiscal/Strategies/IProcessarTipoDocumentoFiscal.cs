using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies
{
    public interface IProcessarTipoDocumentoFiscal
    {
        Task<bool> PodeProcessar(string xmlContent);
        Task<Domain.Entities.DocumentoFiscal> ProcessarXml(string xmlContent); 
    }
}
