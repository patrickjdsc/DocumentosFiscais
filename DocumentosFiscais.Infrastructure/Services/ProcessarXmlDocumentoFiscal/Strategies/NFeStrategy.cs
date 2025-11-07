using DocumentosFiscais.Domain.Entities; 
using System.Globalization; 
using System.Xml;

namespace DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies
{
    public class NFeStrategy : IProcessarTipoDocumentoFiscal
    {

        private const string NFE_NAMESPACE = "http://www.portalfiscal.inf.br/nfe";
        private const string NFE_PREFIXO = "nfe"; 

        public Task<bool> PodeProcessar(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return Task.FromResult(false);

            //pode ser <nfe ou <nfeProc
            var nfeTag = "<"+ NFE_PREFIXO;
            var contemTagNfe = xmlContent.ToLower().Contains(nfeTag, StringComparison.OrdinalIgnoreCase);
 
            return Task.FromResult(contemTagNfe);
        }

        public Task<DocumentoFiscal> ProcessarXml(string xmlContent)
        {
            XmlDocument notaFiscal = CarregarXmlDocument(xmlContent);
            XmlNamespaceManager nsManager = CriarNamespaceManager(notaFiscal);

            var chave = notaFiscal.SelectSingleNode("//nfe:infNFe", nsManager).Attributes["Id"].Value.Replace("NFe", "");
            var emitente = notaFiscal.SelectSingleNode("//nfe:emit/nfe:CNPJ", nsManager)?.InnerText;
            var destinatario = notaFiscal.SelectSingleNode("//nfe:dest/nfe:CNPJ", nsManager)?.InnerText ?? notaFiscal.SelectSingleNode("//nfe:dest/nfe:CPF", nsManager)?.InnerText;
            var dataEmissaoText = notaFiscal.SelectSingleNode("//nfe:ide/nfe:dhEmi", nsManager)?.InnerText ?? notaFiscal.SelectSingleNode("//nfe:ide/nfe:dEmi", nsManager)?.InnerText;
            var numero = notaFiscal.SelectSingleNode("//nfe:ide/nfe:nNF", nsManager)?.InnerText;
            var serie = notaFiscal.SelectSingleNode("//nfe:ide/nfe:serie", nsManager)?.InnerText;

            var nfe = new DocumentoFiscal
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Tipo = "nfe",
                Chave = chave,
                CnpjEmitente = emitente,
                Destinatario = destinatario,
                DataEmissao = dataEmissaoText is not null ? DateTime.Parse(dataEmissaoText) : DateTime.Now,
                ValorTotal = decimal.Parse(notaFiscal.SelectSingleNode("//nfe:total/nfe:ICMSTot/nfe:vNF", nsManager)?.InnerText, CultureInfo.InvariantCulture),
                Numero = numero,
                Serie = serie,
                Raw = xmlContent
            };

            return Task.FromResult(nfe);
        }

        private static XmlNamespaceManager CriarNamespaceManager(XmlDocument notaFiscal)
        {
            var nsManager = new XmlNamespaceManager(notaFiscal.NameTable);
            nsManager.AddNamespace(NFE_PREFIXO, NFE_NAMESPACE);
            return nsManager;
        }

        private static XmlDocument CarregarXmlDocument(string xmlContent)
        {
            var notaFiscal = new XmlDocument();
            notaFiscal.LoadXml(xmlContent);
            return notaFiscal;
        }
    }
}
