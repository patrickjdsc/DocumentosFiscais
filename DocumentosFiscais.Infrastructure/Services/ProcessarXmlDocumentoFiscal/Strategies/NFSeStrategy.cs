using DocumentosFiscais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies
{
    public class NFSeStrategy : IProcessarTipoDocumentoFiscal
    {
        public Task<bool> PodeProcessar(string xmlContent)
        {
            throw new NotImplementedException();
        }

        public Task<DocumentoFiscal> ProcessarXml(string xmlContent)
        {
            var notaFiscal = new XmlDocument();
            notaFiscal.LoadXml(xmlContent);

            var nfe = new DocumentoFiscal
            {
                Chave = notaFiscal.SelectSingleNode("//infNFe").Attributes["Id"].Value.Replace("NFe", ""),
                Emitente = notaFiscal.SelectSingleNode("//emit/xNome")?.InnerText,
                Destinatario = notaFiscal.SelectSingleNode("//dest/xNome")?.InnerText,
                DataEmissao = DateTime.Parse(notaFiscal.SelectSingleNode("//ide/dhEmi")?.InnerText ?? notaFiscal.SelectSingleNode("//ide/dEmi")?.InnerText),
                ValorTotal = decimal.Parse(notaFiscal.SelectSingleNode("//total/ICMSTot/vNF")?.InnerText, CultureInfo.InvariantCulture),
                Raw = xmlContent
            };

            return Task.FromResult(nfe);
        }
    }
}
