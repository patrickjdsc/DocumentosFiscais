using DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.ReceberDocumentoFiscal;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.UpdateDocumentoFiscal;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Commands.DeleteDocumentoFiscal;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.ListarDocumentosFiscais;
using DocumentosFiscais.Application.Features.DocumentoFiscal.Queries.GetDocumentoFiscalById;
using DocumentosFiscais.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using DocumentosFiscais.Domain.Entities;

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

        /// <summary>
        /// Recebe e processa arquivos XML fiscais (NFe, CTe ou NFSe)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CarregarDocumentoFiscal(IFormFile arquivo)
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

        /// <summary>
        /// Lista documentos fiscais com paginação e filtros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDocumentosFiscais(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
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

        /// <summary>
        /// Consulta detalhes de um documento fiscal específico
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDocumentoFiscalById(string id)
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

        /// <summary>
        /// Atualiza um documento fiscal existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDocumentoFiscal(string id, [FromBody] DocumentoFiscal documentoFiscal)
        { 
            var command = new AtualizarDocumentoFiscalCommand
            {
                Id = id,
                DocumentoFiscal = documentoFiscal
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

        /// <summary>
        /// Exclui um documento fiscal
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteDocumentoFiscal(string id)
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
