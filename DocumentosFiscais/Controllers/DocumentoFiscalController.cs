using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace DocumentosFiscais.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentoFiscalController : ControllerBase
    { 
         
        public class DocumentoFiscal
        {
            public Guid Id { get; set; }
            public string Tipo { get; set; } // NFe, CTe, NFSe
            public string Chave { get; set; }
            public string Destinatario { get; set; }
            public string Emitente { get; set; }
            public DateTime DataEmissao { get; set; }
            public decimal ValorTotal { get; set; }
            public string Raw { get; set; }
        }


        //1. Receba arquivos XML fiscais (NFe, CTe ou NFSe). 
        [HttpPost]
        public IActionResult CarregarDocumentoFiscal(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            //verificar se o arquivo é xml
            if (!arquivo.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("O arquivo enviado não é um arquivo XML.");
            }

            //Processar o elemento root do arquivo para determinar o tipo de documento fiscal
            using var stream = arquivo.OpenReadStream();
            using var reader = new StreamReader(stream);
            var conteudo = reader.ReadToEnd();
             
            var documentoFiscal = new DocumentoFiscal();

            if (conteudo.Contains("<nfeProc"))
            {
                documentoFiscal = ProcessarNotaFiscalProduto(conteudo).Result;
            }
            else if (conteudo.Contains("<CTe"))
            {
                documentoFiscal = ProcessarNotaFiscalServico(conteudo).Result;
            }
            else if (conteudo.Contains("<nfse"))
            {
                documentoFiscal = ProcessarConhecimentoTransporte(conteudo).Result;
            }
            else
            {
                return BadRequest("O arquivo XML não é um documento fiscal válido (NFe, CTe ou NFSe).");
            }




            // Salvar o arquivo, validar o conteúdo, etc.
            return Ok("Arquivo XML recebido com sucesso.");
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
                ValorTotal = decimal.Parse(notaFiscal.SelectSingleNode("//total/ICMSTot/vNF")?.InnerText, CultureInfo.InvariantCulture)
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
