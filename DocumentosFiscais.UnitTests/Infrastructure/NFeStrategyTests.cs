using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies;
using FluentAssertions;
using System.Globalization;

namespace DocumentosFiscais.UnitTests.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies
{
    [TestFixture]
    public class NFeStrategyTests
    {
        private NFeStrategy _strategy;

        [SetUp]
        public void SetUp()
        {
            _strategy = new NFeStrategy();
        }

        [TestFixture]
        public class PodeProcessarTests : NFeStrategyTests
        {
            [Test]
            public async Task PodeProcessar_ComXmlNFeValido_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<nfeProc><NFe><infNFe Id=\"NFe35200114200166000187550010000000046501234567890\"></nfeProc>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_ComTagNFeMinuscula_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<nfe:nfeProc><nfe:NFe></nfe:nfeProc>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_ComTagNFeMaiuscula_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<NFE:NFEPROC><NFE:NFE></NFE:NFEPROC>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_ComXmlVazio_DeveRetornarFalse()
            {
                // Arrange
                var xmlContent = string.Empty;

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeFalse();
            }

            [Test]
            public async Task PodeProcessar_ComXmlNull_DeveRetornarFalse()
            {
                // Arrange
                string xmlContent = null;

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeFalse();
            }

            [Test]
            public async Task PodeProcessar_ComXmlSemTagNFe_DeveRetornarFalse()
            {
                // Arrange
                var xmlContent = "<cteProc><CTe><infCTe></infCTe></CTe></cteProc>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeFalse();
            }

            [Test]
            public async Task PodeProcessar_ComEspacosEmBranco_DeveRetornarFalse()
            {
                // Arrange
                var xmlContent = "   ";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeFalse();
            }
        }

        [TestFixture]
        public class ProcessarXmlTests : NFeStrategyTests
        {
            private string _xmlNFeCompleta;

            [SetUp]
            public void SetUpProcessarXml()
            {
                _xmlNFeCompleta = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<nfeProc xmlns=""http://www.portalfiscal.inf.br/nfe"">
    <NFe>
        <infNFe Id=""NFe35200114200166000187550010000000046501234567890"">
            <ide>
                <nNF>46</nNF>
                <serie>1</serie>
                <dhEmi>2020-01-15T10:30:00-03:00</dhEmi>
            </ide>
            <emit>
                <CNPJ>14200166000187</CNPJ>
            </emit>
            <dest>
                <CNPJ>11222333000181</CNPJ>
            </dest>
            <total>
                <ICMSTot>
                    <vNF>1500.50</vNF>
                </ICMSTot>
            </total>
        </infNFe>
    </NFe>
</nfeProc>";
            }

            [Test]
            public async Task ProcessarXml_ComNFeCompleta_DeveProcessarCorretamente()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(_xmlNFeCompleta);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Id.Should().NotBeNullOrEmpty();
                resultado.Tipo.Should().Be("nfe");
                resultado.Chave.Should().Be("35200114200166000187550010000000046501234567890");
                resultado.CnpjEmitente.Should().Be("14200166000187");
                resultado.Destinatario.Should().Be("11222333000181");
                resultado.DataEmissao.Should().Be(DateTime.Parse("2020-01-15T10:30:00-03:00"));
                resultado.ValorTotal.Should().Be(1500.50m);
                resultado.Numero.Should().Be("46");
                resultado.Serie.Should().Be("1");
                resultado.Raw.Should().Be(_xmlNFeCompleta);
            }
 
            [Test]
            public async Task ProcessarXml_ComDataEmissaoSimples_DeveProcessarCorretamente()
            {
                // Arrange
                var xmlComDataSimples = _xmlNFeCompleta.Replace(
                    "<dhEmi>2020-01-15T10:30:00-03:00</dhEmi>",
                    "<dEmi>2020-01-15</dEmi>");

                // Act
                var resultado = await _strategy.ProcessarXml(xmlComDataSimples);

                // Assert
                resultado.DataEmissao.Should().Be(DateTime.Parse("2020-01-15"));
            }

            [Test]
            public async Task ProcessarXml_SemDataEmissao_DeveUsarDataAtual()
            {
                // Arrange
                var dataAntes = DateTime.Now.AddSeconds(-1);
                var xmlSemData = _xmlNFeCompleta.Replace(
                    "<dhEmi>2020-01-15T10:30:00-03:00</dhEmi>",
                    "");
                var dataDepois = DateTime.Now.AddSeconds(1);

                // Act
                var resultado = await _strategy.ProcessarXml(xmlSemData);

                // Assert
                resultado.DataEmissao.Should().BeAfter(dataAntes).And.BeBefore(dataDepois);
            }

            [Test]
            public async Task ProcessarXml_ComValorDecimalAmericano_DeveProcessarCorretamente()
            {
                // Arrange
                var xmlComValorAmericano = _xmlNFeCompleta.Replace(
                    "<vNF>1500.50</vNF>",
                    "<vNF>2999.99</vNF>");

                // Act
                var resultado = await _strategy.ProcessarXml(xmlComValorAmericano);

                // Assert
                resultado.ValorTotal.Should().Be(2999.99m);
            }

            [Test]
            public void ProcessarXml_ComXmlInvalido_DeveLancarExcecao()
            {
                // Arrange
                var xmlInvalido = "xml inválido";

                // Act & Assert
                Assert.ThrowsAsync<System.Xml.XmlException>(
                    async () => await _strategy.ProcessarXml(xmlInvalido));
            }

            [Test]
            public void ProcessarXml_ComXmlSemCamposObrigatorios_DeveLancarExcecao()
            {
                // Arrange
                var xmlSemCamposObrigatorios = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<nfeProc xmlns=""http://www.portalfiscal.inf.br/nfe"">
    <NFe>
    </NFe>
</nfeProc>";

                // Act & Assert
                Assert.ThrowsAsync<NullReferenceException>(
                    async () => await _strategy.ProcessarXml(xmlSemCamposObrigatorios));
            }

            [Test]
            public async Task ProcessarXml_GerarIdUnico_DeveTerFormatoCorreto()
            {
                // Act
                var resultado1 = await _strategy.ProcessarXml(_xmlNFeCompleta);
                var resultado2 = await _strategy.ProcessarXml(_xmlNFeCompleta);

                // Assert
                resultado1.Id.Should().NotBeNullOrEmpty();
                resultado2.Id.Should().NotBeNullOrEmpty();
                resultado1.Id.Should().NotBe(resultado2.Id);
                resultado1.Id.Should().NotContain("-");
                resultado1.Id.Length.Should().Be(32); // GUID sem hifens
            }
        }
    }
}