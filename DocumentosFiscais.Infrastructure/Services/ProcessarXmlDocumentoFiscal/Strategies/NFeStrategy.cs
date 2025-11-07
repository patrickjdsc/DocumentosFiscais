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
    public class NFeStrategy : IProcessarTipoDocumentoFiscal
    {
        public Task<bool> PodeProcessar(string xmlContent)
        {
            return Task.FromResult(xmlContent.Contains("<NFe", StringComparison.OrdinalIgnoreCase));
        }

        public Task<DocumentoFiscal> ProcessarXml(string xmlContent)
        {
            var notaFiscal = new XmlDocument();
            notaFiscal.LoadXml(xmlContent);

            var nsManager = new XmlNamespaceManager(notaFiscal.NameTable);
            nsManager.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");

            var chave = notaFiscal.SelectSingleNode("//nfe:infNFe", nsManager).Attributes["Id"].Value.Replace("NFe", "");
            var emitente = notaFiscal.SelectSingleNode("//nfe:emit/nfe:CNPJ", nsManager)?.InnerText;
            var destinatario = notaFiscal.SelectSingleNode("//nfe:dest/nfe:CNPJ", nsManager)?.InnerText ?? notaFiscal.SelectSingleNode("//nfe:dest/nfe:CPF", nsManager)?.InnerText;
            var dataEmissaoText = notaFiscal.SelectSingleNode("//nfe:ide/nfe:dhEmi", nsManager)?.InnerText ?? notaFiscal.SelectSingleNode("//nfe:ide/nfe:dEmi", nsManager)?.InnerText;


            var nfe = new DocumentoFiscal
            {
                Id = Guid.NewGuid(),
                Tipo = "NFe",
                Chave = chave,
                Emitente = emitente,
                Destinatario = destinatario,
                DataEmissao = dataEmissaoText is not null ? DateTime.Parse(dataEmissaoText) : DateTime.Now,
                ValorTotal = decimal.Parse(notaFiscal.SelectSingleNode("//nfe:total/nfe:ICMSTot/nfe:vNF", nsManager)?.InnerText, CultureInfo.InvariantCulture),
                Raw = xmlContent
            };

            return Task.FromResult(nfe);
        }
    }
}
