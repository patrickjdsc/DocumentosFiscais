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
        private const string NFSE_NAMESPACE = "http://www.abrasf.org.br/nfse.xsd";
        private const string NFSE_PREFIXO = "CompNfse";

        public Task<bool> PodeProcessar(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return Task.FromResult(false);

            var nfeTag = "<" + NFSE_PREFIXO;
            var contemTagNfe = xmlContent.ToLower().Contains(nfeTag, StringComparison.OrdinalIgnoreCase);

            return Task.FromResult(contemTagNfe);
        }

        public Task<DocumentoFiscal> ProcessarXml(string xmlContent)
        {
            XmlDocument notaFiscal = CarregarXmlDocument(xmlContent);
            XmlNamespaceManager nsManager = CriarNamespaceManager(notaFiscal);

            var chave = notaFiscal.SelectSingleNode("//nfse:CodigoVerificacao", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//CodigoVerificacao")?.InnerText ??
                notaFiscal.SelectSingleNode("//nfse:IdentificacaoNfse/nfse:CodigoVerificacao", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//IdentificacaoNfse/CodigoVerificacao")?.InnerText;

            var emitente = notaFiscal.SelectSingleNode("//nfse:PrestadorServico/nfse:CpfCnpj", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//PrestadorServico/CpfCnpj")?.InnerText ??
                notaFiscal.SelectSingleNode("//nfse:Prestador/nfse:CpfCnpj", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//Prestador/CpfCnpj")?.InnerText;
            var destinatario = notaFiscal.SelectSingleNode("//nfse:Tomador/nfse:CpfCnpj", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//Tomador/CpfCnpj")?.InnerText ??
                notaFiscal.SelectSingleNode("//nfse:Tomador/nfse:IdentificacaoTomador/nfse:CpfCnpj", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//Tomador/IdentificacaoTomador/CpfCnpj")?.InnerText;
            var dataEmissaoText = notaFiscal.SelectSingleNode("//nfse:DataEmissaoNfse", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//DataEmissaoNfse")?.InnerText ??
                notaFiscal.SelectSingleNode("//nfse:DataEmissao", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//DataEmissao")?.InnerText;
            var numero = notaFiscal.SelectSingleNode("//nfse:NumeroNfse", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//NumeroNfse")?.InnerText ??
                notaFiscal.SelectSingleNode("//nfse:Numero", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//Numero")?.InnerText;
            var serie = notaFiscal.SelectSingleNode("//nfse:Serie", nsManager)?.InnerText ??
                notaFiscal.SelectSingleNode("//Serie")?.InnerText;

            var nfse = new DocumentoFiscal
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Tipo = "nfse",
                Chave = chave,
                CnpjEmitente = emitente,
                Destinatario = destinatario,
                DataEmissao = dataEmissaoText is not null ? DateTime.Parse(dataEmissaoText) : DateTime.Now,
                ValorTotal = decimal.Parse(notaFiscal.SelectSingleNode("//ValorServicos")?.InnerText ?? notaFiscal.SelectSingleNode("//ValoresNfse/ValorServicos")?.InnerText ?? "0", CultureInfo.InvariantCulture),
                Numero = numero,
                Serie = serie,
                Raw = xmlContent
            };

            return Task.FromResult(nfse);
        }

        private static XmlNamespaceManager CriarNamespaceManager(XmlDocument notaFiscal)
        {
            var nsManager = new XmlNamespaceManager(notaFiscal.NameTable);
            nsManager.AddNamespace("nfse", NFSE_NAMESPACE);
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
