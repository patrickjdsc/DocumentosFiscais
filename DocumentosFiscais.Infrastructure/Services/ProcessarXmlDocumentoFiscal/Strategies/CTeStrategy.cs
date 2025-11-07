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
    public class CTeStrategy : IProcessarTipoDocumentoFiscal
    {
        public Task<bool> PodeProcessar(string xmlContent)
        {
            return Task.FromResult(xmlContent.Contains("<NFe", StringComparison.OrdinalIgnoreCase));
        }

        public Task<DocumentoFiscal> ProcessarXml(string xml)
        {
            var xmlCte = new XmlDocument();
            xmlCte.LoadXml(xml);

            var cte = new DocumentoFiscal
            {
                Chave = xmlCte.SelectSingleNode("//infCte").Attributes["Id"].Value.Replace("CTe", ""),
                Emitente = xmlCte.SelectSingleNode("//emit/xNome")?.InnerText,
                Destinatario = xmlCte.SelectSingleNode("//dest/xNome")?.InnerText,
                DataEmissao = DateTime.Parse(xmlCte.SelectSingleNode("//ide/dhEmi")?.InnerText ?? xmlCte.SelectSingleNode("//ide/dEmi")?.InnerText),
                ValorTotal = decimal.Parse(xmlCte.SelectSingleNode("//vPrest/vTPrest")?.InnerText, CultureInfo.InvariantCulture),
                Raw = xml
            };

            return Task.FromResult(cte);
        }
    }
}
