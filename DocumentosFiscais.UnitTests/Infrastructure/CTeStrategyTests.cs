using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies;
using FluentAssertions;
using System;
using System.Threading.Tasks;

namespace DocumentosFiscais.UnitTests.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies
{
    [TestFixture]
    public class CTeStrategyTests
    {
        private CTeStrategy _strategy;
        private const string XmlCTeValido = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<cteProc xmlns=""http://www.portalfiscal.inf.br/cte"">
    <CTe xmlns=""http://www.portalfiscal.inf.br/cte"">
        <infCte Id=""CTe35210114200166000187570010000000041176842531"">
            <ide>
                <cUF>35</cUF>
                <cCT>17684253</cCT>
                <CFOP>6353</CFOP>
                <natOp>PRESTACAO DE SERVICO DE TRANSPORTE</natOp>
                <mod>57</mod>
                <serie>1</serie>
                <nCT>4</nCT>
                <dhEmi>2021-01-01T10:00:00-03:00</dhEmi>
                <tpImp>1</tpImp>
                <tpEmis>1</tpEmis>
                <cDV>1</cDV>
                <tpAmb>2</tpAmb>
                <tpCTe>0</tpCTe>
                <procEmi>0</procEmi>
                <verProc>4.00</verProc>
            </ide>
            <emit>
                <CNPJ>14200166000187</CNPJ>
                <IE>123456789</IE>
                <xNome>EMPRESA EMITENTE LTDA</xNome>
            </emit>
            <dest>
                <CNPJ>12345678000195</CNPJ>
                <IE>987654321</IE>
                <xNome>EMPRESA DESTINATARIA LTDA</xNome>
            </dest>
            <vPrest>
                <vTPrest>1250.50</vTPrest>
                <vRec>1250.50</vRec>
            </vPrest>
        </infCte>
    </CTe>
</cteProc>";

        private const string XmlCTeComCpf = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<cteProc xmlns=""http://www.portalfiscal.inf.br/cte"">
    <CTe xmlns=""http://www.portalfiscal.inf.br/cte"">
        <infCte Id=""CTe35210114200166000187570010000000041176842531"">
            <ide>
                <serie>1</serie>
                <nCT>4</nCT>
                <dEmi>2021-01-01</dEmi>
            </ide>
            <emit>
                <CNPJ>14200166000187</CNPJ>
            </emit>
            <dest>
                <CPF>12345678901</CPF>
            </dest>
            <vPrest>
                <vTPrest>500.75</vTPrest>
            </vPrest>
        </infCte>
    </CTe>
</cteProc>";

        [SetUp]
        public void Setup()
        {
            _strategy = new CTeStrategy();
        }

        [TestFixture]
        public class PodeProcessarTests : CTeStrategyTests
        {
            [Test]
            public async Task PodeProcessar_XmlComTagCte_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<cte xmlns=\"http://www.portalfiscal.inf.br/cte\"></cte>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_XmlComTagCteUpperCase_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<CTE xmlns=\"http://www.portalfiscal.inf.br/cte\"></CTE>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_XmlSemTagCte_DeveRetornarFalse()
            {
                // Arrange
                var xmlContent = "<nfe xmlns=\"http://www.portalfiscal.inf.br/nfe\"></nfe>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeFalse();
            }

            [Test]
            public async Task PodeProcessar_StringNula_DeveRetornarFalse()
            {
                // Arrange
                string xmlContent = null;

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeFalse();
            }

            [Test]
            public async Task PodeProcessar_StringVazia_DeveRetornarFalse()
            {
                // Arrange
                var xmlContent = "";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeFalse();
            }

            [Test]
            public async Task PodeProcessar_StringComEspacos_DeveRetornarFalse()
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
        public class ProcessarXmlTests : CTeStrategyTests
        {
            [Test]
            public async Task ProcessarXml_XmlCteValido_DeveExtrairDadosCorretamente()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(XmlCTeValido);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Id.Should().NotBeNullOrEmpty();
                resultado.Tipo.Should().Be("cte");
                resultado.Chave.Should().Be("35210114200166000187570010000000041176842531");
                resultado.CnpjEmitente.Should().Be("14200166000187");
                resultado.Destinatario.Should().Be("12345678000195");
                resultado.DataEmissao.Should().Be(new DateTime(2021, 1, 1, 10, 0, 0));
                resultado.ValorTotal.Should().Be(1250.50m);
                resultado.Numero.Should().Be("4");
                resultado.Serie.Should().Be("1");
                resultado.Raw.Should().Be(XmlCTeValido);
            }

            [Test]
            public async Task ProcessarXml_XmlComCpfDestinatario_DeveExtrairCpf()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(XmlCTeComCpf);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Destinatario.Should().Be("12345678901");
                resultado.ValorTotal.Should().Be(500.75m);
                resultado.DataEmissao.Should().Be(new DateTime(2021, 1, 1));
            }

            [Test]
            public async Task ProcessarXml_IdGerado_DeveSerGuidSemHifens()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(XmlCTeValido);

                // Assert
                resultado.Id.Should().NotContain("-");
                resultado.Id.Length.Should().Be(32); // GUID sem hífens tem 32 caracteres
            }

            [Test]
            public async Task ProcessarXml_XmlCteValido_DevePreserverXmlOriginal()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(XmlCTeValido);

                // Assert
                resultado.Raw.Should().Be(XmlCTeValido);
            }

            [Test]
            public void ProcessarXml_XmlInvalido_DeveLancarExcecao()
            {
                // Arrange
                var xmlInvalido = "<xml_malformado>";

                // Act & Assert
                Assert.ThrowsAsync<System.Xml.XmlException>(async () => 
                    await _strategy.ProcessarXml(xmlInvalido));
            }

            [Test]
            public void ProcessarXml_XmlSemNosObrigatorios_DeveLancarExcecao()
            {
                // Arrange
                var xmlSemNos = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <cteProc xmlns=""http://www.portalfiscal.inf.br/cte"">
                    <CTe xmlns=""http://www.portalfiscal.inf.br/cte"">
                    </CTe>
                </cteProc>";

                // Act & Assert
                Assert.ThrowsAsync<NullReferenceException>(async () => 
                    await _strategy.ProcessarXml(xmlSemNos));
            }
        }

        [TestFixture]
        public class IntegrationTests : CTeStrategyTests
        {
            [Test]
            public async Task FluxoCompleto_XmlCteValido_DeveProcessarCorretamente()
            {
                // Arrange & Act
                var podeProcessar = await _strategy.PodeProcessar(XmlCTeValido);
                var documento = podeProcessar ? await _strategy.ProcessarXml(XmlCTeValido) : null;

                // Assert
                podeProcessar.Should().BeTrue();
                documento.Should().NotBeNull();
                documento!.Tipo.Should().Be("cte");
                documento.Chave.Should().NotBeNullOrEmpty();
                documento.CnpjEmitente.Should().NotBeNullOrEmpty();
                documento.ValorTotal.Should().BeGreaterThan(0);
            }
        }
    }
}