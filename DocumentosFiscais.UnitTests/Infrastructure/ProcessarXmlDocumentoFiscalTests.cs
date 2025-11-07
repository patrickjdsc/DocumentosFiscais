using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Application.Contracts.Services;
using DocumentosFiscais.Domain.Entities;
using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal;
using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies;
using FluentAssertions;
using Shouldly;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentosFiscais.UnitTests.Infrastructure.Services.ProcessarXmlDocumentoFiscal
{
    [TestFixture]
    public class ProcessarXmlDocumentoFiscalTests
    {
        private IProcessarXmlDocumentoFiscal _service;
        private Mock<IProcessarTipoDocumentoFiscal> _mockProcessadorTipo1;
        private Mock<IProcessarTipoDocumentoFiscal> _mockProcessadorTipo2;
        private Mock<IDocumentoFiscalRepository> _mockRepository;
        private List<IProcessarTipoDocumentoFiscal> _processadores;

        private const string XmlValido = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<nfeProc xmlns=""http://www.portalfiscal.inf.br/nfe"">
    <NFe>
        <infNFe Id=""NFe35210114200166000187550010000000041176842531"">
            <ide>
                <serie>1</serie>
                <nNF>4</nNF>
                <dhEmi>2021-01-01T10:00:00-03:00</dhEmi>
            </ide>
        </infNFe>
    </NFe>
</nfeProc>";

        [SetUp]
        public void Setup()
        {
            _mockProcessadorTipo1 = new Mock<IProcessarTipoDocumentoFiscal>();
            _mockProcessadorTipo2 = new Mock<IProcessarTipoDocumentoFiscal>();
            _mockRepository = new Mock<IDocumentoFiscalRepository>();

            _processadores = new List<IProcessarTipoDocumentoFiscal>
            {
                _mockProcessadorTipo1.Object,
                _mockProcessadorTipo2.Object
            };

            _service = new DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.ProcessarXmlDocumentoFiscal(_processadores, _mockRepository.Object);
        }

        [TestFixture]
        public class ProcessarTests : ProcessarXmlDocumentoFiscalTests
        {
            [Test]
            public async Task Processar_XmlValido_PrimeiroProcessadorPodeProcessar_DeveRetornarDocumento()
            {
                // Arrange
                var documentoEsperado = new DocumentoFiscal
                {
                    Id = "123",
                    Tipo = "nfe",
                    Chave = "35210114200166000187550010000000041176842531"
                };

                _mockProcessadorTipo1.Setup(x => x.PodeProcessar(XmlValido))
                    .ReturnsAsync(true);
                _mockProcessadorTipo1.Setup(x => x.ProcessarXml(XmlValido))
                    .ReturnsAsync(documentoEsperado);

                // Act
                var resultado = await _service.Processar(XmlValido);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Should().BeEquivalentTo(documentoEsperado);
                _mockProcessadorTipo1.Verify(x => x.PodeProcessar(XmlValido), Times.Once);
                _mockProcessadorTipo1.Verify(x => x.ProcessarXml(XmlValido), Times.Once);
                _mockProcessadorTipo2.Verify(x => x.PodeProcessar(It.IsAny<string>()), Times.Never);
            }

            [Test]
            public async Task Processar_XmlValido_SegundoProcessadorPodeProcessar_DeveRetornarDocumento()
            {
                // Arrange
                var documentoEsperado = new DocumentoFiscal
                {
                    Id = "456",
                    Tipo = "cte",
                    Chave = "35210114200166000187570010000000041176842531"
                };

                _mockProcessadorTipo1.Setup(x => x.PodeProcessar(XmlValido))
                    .ReturnsAsync(false);
                _mockProcessadorTipo2.Setup(x => x.PodeProcessar(XmlValido))
                    .ReturnsAsync(true);
                _mockProcessadorTipo2.Setup(x => x.ProcessarXml(XmlValido))
                    .ReturnsAsync(documentoEsperado);

                // Act
                var resultado = await _service.Processar(XmlValido);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Should().BeEquivalentTo(documentoEsperado);
                _mockProcessadorTipo1.Verify(x => x.PodeProcessar(XmlValido), Times.Once);
                _mockProcessadorTipo1.Verify(x => x.ProcessarXml(It.IsAny<string>()), Times.Never);
                _mockProcessadorTipo2.Verify(x => x.PodeProcessar(XmlValido), Times.Once);
                _mockProcessadorTipo2.Verify(x => x.ProcessarXml(XmlValido), Times.Once);
            }

            [Test]
            public async Task Processar_XmlValido_NenhumProcessadorPodeProcessar_DeveLancarNotSupportedException()
            {
                // Arrange
                _mockProcessadorTipo1.Setup(x => x.PodeProcessar(XmlValido))
                    .ReturnsAsync(false);
                _mockProcessadorTipo2.Setup(x => x.PodeProcessar(XmlValido))
                    .ReturnsAsync(false);

                // Act & Assert
                var exception = Assert.CatchAsync(
                    async () => await _service.Processar(XmlValido));

                exception.Message.Should().Be("Tipo de documento fiscal não suportado.");
                _mockProcessadorTipo1.Verify(x => x.PodeProcessar(XmlValido), Times.Once);
                _mockProcessadorTipo2.Verify(x => x.PodeProcessar(XmlValido), Times.Once);
                _mockProcessadorTipo1.Verify(x => x.ProcessarXml(It.IsAny<string>()), Times.Never);
                _mockProcessadorTipo2.Verify(x => x.ProcessarXml(It.IsAny<string>()), Times.Never);
            } 

            [Test]
            public async Task Processar_XmlVazio_DeveLancarArgumentException()
            {
                // Act & Assert
                var exception =   Assert.ThrowsAsync<ArgumentException>(
                    async () => await _service.Processar(""));

                exception.Message.Should().Contain("XML não pode ser vazio ou nulo.");
                exception.ParamName.Should().Be("xml");
                _mockProcessadorTipo1.Verify(x => x.PodeProcessar(It.IsAny<string>()), Times.Never);
                _mockProcessadorTipo2.Verify(x => x.PodeProcessar(It.IsAny<string>()), Times.Never);
            }

            [Test]
            public async Task Processar_XmlComEspacos_DeveLancarArgumentException()
            {
                // Act & Assert
                var exception = Assert.ThrowsAsync<ArgumentException>(
                    async () => await _service.Processar("   "));

                exception.Message.Should().Contain("XML não pode ser vazio ou nulo.");
                exception.ParamName.Should().Be("xml");
                _mockProcessadorTipo1.Verify(x => x.PodeProcessar(It.IsAny<string>()), Times.Never);
                _mockProcessadorTipo2.Verify(x => x.PodeProcessar(It.IsAny<string>()), Times.Never);
            }

            [Test]
            public async Task Processar_ProcessadorLancaExcecao_DevePropagarExcecao()
            {
                // Arrange
                var exceptionEsperada = new InvalidOperationException("Erro no processamento");

                _mockProcessadorTipo1.Setup(x => x.PodeProcessar(XmlValido))
                    .ReturnsAsync(true);
                _mockProcessadorTipo1.Setup(x => x.ProcessarXml(XmlValido))
                    .ThrowsAsync(exceptionEsperada);

                // Act & Assert
                var exception = Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await _service.Processar(XmlValido));

                //exception.Should().Be(exceptionEsperada);
                //_mockProcessadorTipo1.Verify(x => x.PodeProcessar(XmlValido), Times.Once);
                //_mockProcessadorTipo1.Verify(x => x.ProcessarXml(XmlValido), Times.Once);
            } 
        }

        [TestFixture]
        public class IntegrationTests : ProcessarXmlDocumentoFiscalTests
        {
            [Test]
            public async Task Processar_FluxoCompleto_DeveProcessarComSucesso()
            {
                // Arrange
                var documentoEsperado = new DocumentoFiscal
                {
                    Id = "789",
                    Tipo = "nfe",
                    Chave = "35210114200166000187550010000000041176842531",
                    CnpjEmitente = "14200166000187",
                    ValorTotal = 1500.00m
                };

                _mockProcessadorTipo1.Setup(x => x.PodeProcessar(XmlValido))
                    .ReturnsAsync(true);
                _mockProcessadorTipo1.Setup(x => x.ProcessarXml(XmlValido))
                    .ReturnsAsync(documentoEsperado);

                // Act
                var resultado = await _service.Processar(XmlValido);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Id.Should().Be("789");
                resultado.Tipo.Should().Be("nfe");
                resultado.Chave.Should().Be("35210114200166000187550010000000041176842531");
                resultado.ValorTotal.Should().Be(1500.00m);
            }
        }
    }
}
