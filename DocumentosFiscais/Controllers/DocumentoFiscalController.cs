using DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.ReceberDocumentoFiscal;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.UpdateDocumentoFiscal;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.DeleteDocumentoFiscal;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.ListarDocumentosFiscais;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.GetDocumentoFiscalById;
using DocumentosFiscais.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentosFiscais.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DocumentoFiscalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentoFiscalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(DocumentoFiscalViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CarregarDocumentoFiscal(
            [Required(ErrorMessage = "Arquivo é obrigatório")] IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            if (!arquivo.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("O arquivo enviado não é um arquivo XML.");
            }

            using var stream = arquivo.OpenReadStream();
            using var reader = new StreamReader(stream);
            var conteudo = await reader.ReadToEndAsync();

            var command = new ReceberDocumentoFiscalCommand { ArquivoRecebido = conteudo };
            var response = await _mediator.Send(command);

            if (response.Sucesso)
            {
                return Ok(response.DocumentoFiscal);
            }

            return BadRequest(response.Mensagem);
        }
         
        [HttpGet]
        [ProducesResponseType(typeof(ResultadoComPaginacao<DocumentoFiscalViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDocumentosFiscais(
            [FromQuery] [Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery] [Range(1, 100)] int pageSize = 10,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null,
            [FromQuery] string? cnpjEmitente = null,
            [FromQuery] string? tipo = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Parâmetros de paginação inválidos. PageNumber deve ser >= 1 e PageSize entre 1 e 100.");
            }

            var filters = new FiltroBuscaDocumentoFiscal
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                CnpjEmitente = cnpjEmitente,
                Tipo = tipo
            };

            var query = new ListarDocumentosFiscaisQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                FiltroBuscaDocumentoFiscal = filters
            };

            var response = await _mediator.Send(query);

            if (response.Sucesso)
            {
                return Ok(response.DocumentosFiscais);
            }

            return BadRequest(response.Mensagem);
        }
         
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DocumentoFiscalViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDocumentoFiscalById(
            [Required] string id)
        {
            var query = new ListarDocumentoPorIdQuery { Id = id };
            var response = await _mediator.Send(query);

            if (response.Sucesso && response.DocumentoFiscal != null)
            {
                return Ok(response.DocumentoFiscal);
            }

            if (!response.Sucesso && response.Mensagem.Contains("não encontrado"))
            {
                return NotFound(response.Mensagem);
            }

            return BadRequest(response.Mensagem);
        }
         
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DocumentoFiscalViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDocumentoFiscal(
            [Required] string id,
            [Required(ErrorMessage = "Arquivo é obrigatório")] IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            if (!arquivo.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("O arquivo enviado não é um arquivo XML.");
            }

            using var stream = arquivo.OpenReadStream();
            using var reader = new StreamReader(stream);
            var conteudo = await reader.ReadToEndAsync();

            var command = new AtualizarDocumentoFiscalCommand
            {
                Id = id,
                ArquivoXml = conteudo
            };

            var response = await _mediator.Send(command);

            if (response.Sucesso && response.DocumentoFiscal != null)
            {
                return Ok(response.DocumentoFiscal);
            }

            if (!response.Sucesso && response.Mensagem.Contains("não encontrado"))
            {
                return NotFound(response.Mensagem);
            }

            return BadRequest(response.Mensagem);
        }
         
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteDocumentoFiscal(
            [Required] string id)
        {
            var command = new DeletarDocumentoFiscalCommand { Id = id };
            var response = await _mediator.Send(command);

            if (response.Sucesso)
            {
                return NoContent();
            }

            if (response.Mensagem.Contains("não encontrado"))
            {
                return NotFound(response.Mensagem);
            }

            return BadRequest(response.Mensagem);
        }
    }
}
