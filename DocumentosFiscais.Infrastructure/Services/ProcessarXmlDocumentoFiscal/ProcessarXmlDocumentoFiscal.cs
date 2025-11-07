using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Application.Contracts.Services;
using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies;
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

        private readonly IEnumerable<IProcessarTipoDocumentoFiscal> _strategies;
        private readonly IDocumentoFiscalRepository _documentoFiscalRepository; 

        public ProcessarXmlDocumentoFiscal(IEnumerable<IProcessarTipoDocumentoFiscal> strategies, IDocumentoFiscalRepository documentoFiscalRepository)
        {
            _strategies = strategies;
            _documentoFiscalRepository = documentoFiscalRepository;
        }

        public async Task<DocumentoFiscal> Processar(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("XML não pode ser vazio ou nulo.", nameof(xml));

            foreach (var strategy in _strategies)
            {
                if (await strategy.PodeProcessar(xml))
                {
                    var documento = await strategy.ProcessarXml(xml);

                    await _documentoFiscalRepository.Inserir(documento);
                     
                    return documento;
                }
            }

            throw new NotSupportedException("Tipo de documento fiscal não suportado.");
        }
    }
}
