using DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.ReceberDocumentoFiscal;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml; 

namespace DocumentosFiscais.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentoFiscalController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DocumentoFiscalController(IMediator mediator)
        {
            _mediator = mediator; 
        }

        //1. Receba arquivos XML fiscais (NFe, CTe ou NFSe). 
        [HttpPost]
        public async Task<IActionResult> CarregarDocumentoFiscal(IFormFile arquivo)
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

            using var stream = arquivo.OpenReadStream();
            using var reader = new StreamReader(stream);
            var conteudo = reader.ReadToEnd();

            var command = new ReceberDocumentoFiscalCommand() { ArquivoRecebido = conteudo};

            var response = await _mediator.Send(command);

            if (response.Sucesso)
            {
                return Ok(response.DocumentoFiscal);
            }
            else
            {
                return BadRequest(response.Mensagem);
            }
        } 
    }
}
