using DocumentosFiscais.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal
{
    public class ProcessarXmlDocumentoFiscal: IProcessarXmlDocumentoFiscal
    {
        public async Task<DocumentoFiscal> Processar(string xml)
        {
            DocumentoFiscal documentoFiscal = null;
            if (xml.Contains("<NFe"))
            {
                documentoFiscal = ProcessarNotaFiscalProduto(xml).Result;
            }
            else if (xml.Contains("<CTe"))
            {
                documentoFiscal = ProcessarConhecimentoTransporte(xml).Result;
            }
            else if (xml.Contains("<nfse"))
            {
                documentoFiscal = ProcessarNotaFiscalServico(xml).Result;
            }
            else
            {
                throw new NotSupportedException("Tipo de documento fiscal não suportado.");
            }
            return documentoFiscal;
        }

        private async Task<DocumentoFiscal> ProcessarNotaFiscalProduto(string xml)
        {
            var notaFiscal = new XmlDocument();
            notaFiscal.LoadXml(xml);

            var nfe = new DocumentoFiscal
            {
                Chave = notaFiscal.SelectSingleNode("//infNFe").Attributes["Id"].Value.Replace("NFe", ""),
                Emitente = notaFiscal.SelectSingleNode("//emit/xNome")?.InnerText,
                Destinatario = notaFiscal.SelectSingleNode("//dest/xNome")?.InnerText,
                DataEmissao = DateTime.Parse(notaFiscal.SelectSingleNode("//ide/dhEmi")?.InnerText ?? notaFiscal.SelectSingleNode("//ide/dEmi")?.InnerText),
                ValorTotal = decimal.Parse(notaFiscal.SelectSingleNode("//total/ICMSTot/vNF")?.InnerText, CultureInfo.InvariantCulture),
                Raw = xml
            };

            return nfe;
        }

        private async Task<DocumentoFiscal> ProcessarNotaFiscalServico(string xml)
        {
            var notaFiscalServico = new XmlDocument();
            notaFiscalServico.LoadXml(xml);

            var padraoUtilizado = "";

            //criar método para descobrir qual o padrão será utilizado.
            //Criar factory para cada padrão processando o XML

            return new DocumentoFiscal();

        }

        private async Task<DocumentoFiscal> ProcessarConhecimentoTransporte(string xml)
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

            return cte;
        }
    }
}
