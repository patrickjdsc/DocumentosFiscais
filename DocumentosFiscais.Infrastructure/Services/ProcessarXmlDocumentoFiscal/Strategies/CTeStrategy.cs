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
        private const string CTE_NAMESPACE = "http://www.portalfiscal.inf.br/cte";
        private const string CTE_PREFIXO = "cte";

        public Task<bool> PodeProcessar(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return Task.FromResult(false);

            var cteTag = "<" + CTE_PREFIXO;
            var contemTagCte = xmlContent.ToLower().Contains(cteTag, StringComparison.OrdinalIgnoreCase);

            return Task.FromResult(contemTagCte);
        }

        public Task<DocumentoFiscal> ProcessarXml(string xmlContent)
        {
            XmlDocument conhecimentoTransporte = CarregarXmlDocument(xmlContent);
            XmlNamespaceManager nsManager = CriarNamespaceManager(conhecimentoTransporte);

            var chave = conhecimentoTransporte.SelectSingleNode("//cte:infCte", nsManager).Attributes["Id"].Value.Replace("CTe", "");
            var emitente = conhecimentoTransporte.SelectSingleNode("//cte:emit/cte:CNPJ", nsManager)?.InnerText;
            var destinatario = conhecimentoTransporte.SelectSingleNode("//cte:dest/cte:CNPJ", nsManager)?.InnerText ?? conhecimentoTransporte.SelectSingleNode("//cte:dest/cte:CPF", nsManager)?.InnerText;
            var dataEmissaoText = conhecimentoTransporte.SelectSingleNode("//cte:ide/cte:dhEmi", nsManager)?.InnerText ?? conhecimentoTransporte.SelectSingleNode("//cte:ide/cte:dEmi", nsManager)?.InnerText;
            var dataEmissao = dataEmissaoText is not null ? DateTime.Parse(dataEmissaoText) : DateTime.Now;
            var numero = conhecimentoTransporte.SelectSingleNode("//cte:ide/cte:nCT", nsManager)?.InnerText;
            var serie = conhecimentoTransporte.SelectSingleNode("//cte:ide/cte:serie", nsManager)?.InnerText;
            var valorTotal = decimal.Parse(conhecimentoTransporte.SelectSingleNode("//cte:vPrest/cte:vTPrest", nsManager)?.InnerText, CultureInfo.InvariantCulture);
            
            var cte = new DocumentoFiscal
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Tipo = "CTe",
                Chave = chave,
                CnpjEmitente = emitente,
                Destinatario = destinatario,
                DataEmissao = dataEmissao,
                ValorTotal = valorTotal,
                Numero = numero,
                Serie = serie,
                Raw = xmlContent
            };

            return Task.FromResult(cte);
        }

        private static XmlNamespaceManager CriarNamespaceManager(XmlDocument conhecimentoTransporte)
        {
            var nsManager = new XmlNamespaceManager(conhecimentoTransporte.NameTable);
            nsManager.AddNamespace(CTE_PREFIXO, CTE_NAMESPACE);
            return nsManager;
        }

        private static XmlDocument CarregarXmlDocument(string xmlContent)
        {
            var conhecimentoTransporte = new XmlDocument();
            conhecimentoTransporte.LoadXml(xmlContent);
            return conhecimentoTransporte;
        }
    }
}
