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
    public class ProcessarXmlDocumentoFiscal(
        IEnumerable<IProcessarTipoDocumentoFiscal> processadoresTipoDocumentoFiscal, 
        IDocumentoFiscalRepository documentoFiscalRepository
        ) : IProcessarXmlDocumentoFiscal
    {

        private readonly IEnumerable<IProcessarTipoDocumentoFiscal> _ProcessadoresTipoDocumentoFiscal = processadoresTipoDocumentoFiscal;
         

        public async Task<DocumentoFiscal> Processar(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("XML não pode ser vazio ou nulo.", nameof(xml));

            foreach (var strategy in _ProcessadoresTipoDocumentoFiscal)
            {
                if (await strategy.PodeProcessar(xml))
                {
                    var documento = await strategy.ProcessarXml(xml);  
                    return documento;
                }
            }

            throw new Exception("Tipo de documento fiscal não suportado.");
        }
    }
}
