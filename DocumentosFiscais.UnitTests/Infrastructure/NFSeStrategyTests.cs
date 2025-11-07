using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies;
using FluentAssertions;
using System;
using System.Threading.Tasks;

namespace DocumentosFiscais.UnitTests.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies
{
    [TestFixture]
    public class NFSeStrategyTests
    {
        private NFSeStrategy _strategy;
        private const string XmlNFSeValido = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<CompNfse xmlns=""http://www.abrasf.org.br/nfse.xsd"">
    <Nfse xmlns=""http://www.abrasf.org.br/nfse.xsd"">
        <InfNfse>
            <IdentificacaoNfse>
                <CodigoVerificacao>ABC123XYZ</CodigoVerificacao>
                <NumeroNfse>123</NumeroNfse>
                <Serie>1</Serie>
            </IdentificacaoNfse>
            <DataEmissaoNfse>2021-01-01T10:00:00</DataEmissaoNfse>
            <PrestadorServico>
                <CpfCnpj>14200166000187</CpfCnpj>
            </PrestadorServico>
            <Tomador>
                <CpfCnpj>12345678000195</CpfCnpj>
            </Tomador>
            <ValoresNfse>
                <ValorServicos>1250.50</ValorServicos>
            </ValoresNfse>
        </InfNfse>
    </Nfse>
</CompNfse>";

        private const string XmlNFSeSemNamespace = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<CompNfse>
    <Nfse>
        <InfNfse>
            <CodigoVerificacao>XYZ789ABC</CodigoVerificacao>
            <NumeroNfse>456</NumeroNfse>
            <DataEmissao>2021-02-15</DataEmissao>
            <PrestadorServico>
                <CpfCnpj>98765432000123</CpfCnpj>
            </PrestadorServico>
            <Tomador>
                <IdentificacaoTomador>
                    <CpfCnpj>11122233000144</CpfCnpj>
                </IdentificacaoTomador>
            </Tomador>
            <ValorServicos>750.25</ValorServicos>
        </InfNfse>
    </Nfse>
</CompNfse>";

        private const string XmlNFSeEstruturaDiferente = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<CompNfse xmlns=""http://www.abrasf.org.br/nfse.xsd"">
    <CodigoVerificacao>DEF456GHI</CodigoVerificacao>
    <Numero>789</Numero>
    <DataEmissaoNfse>2021-03-20T15:30:00</DataEmissaoNfse>
    <Prestador>
        <CpfCnpj>55566677000188</CpfCnpj>
    </Prestador>
    <Tomador>
        <CpfCnpj>99988877000166</CpfCnpj>
    </Tomador>
    <ValorServicos>2000.00</ValorServicos>
</CompNfse>";

        [SetUp]
        public void Setup()
        {
            _strategy = new NFSeStrategy();
        }

        [TestFixture]
        public class PodeProcessarTests : NFSeStrategyTests
        {
            [Test]
            public async Task PodeProcessar_XmlComTagCompNfse_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<CompNfse xmlns=\"http://www.abrasf.org.br/nfse.xsd\"></CompNfse>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_XmlComTagCompNfseUpperCase_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<COMPNFSE xmlns=\"http://www.abrasf.org.br/nfse.xsd\"></COMPNFSE>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_XmlComTagCompNfseLowerCase_DeveRetornarTrue()
            {
                // Arrange
                var xmlContent = "<compnfse xmlns=\"http://www.abrasf.org.br/nfse.xsd\"></compnfse>";

                // Act
                var resultado = await _strategy.PodeProcessar(xmlContent);

                // Assert
                resultado.Should().BeTrue();
            }

            [Test]
            public async Task PodeProcessar_XmlSemTagCompNfse_DeveRetornarFalse()
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
        public class ProcessarXmlTests : NFSeStrategyTests
        {
           
            [Test]
            public async Task ProcessarXml_XmlSemNamespace_DeveExtrairDadosCorretamente()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(XmlNFSeSemNamespace);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Chave.Should().Be("XYZ789ABC");
                resultado.CnpjEmitente.Should().Be("98765432000123");
                resultado.Destinatario.Should().Be("11122233000144");
                resultado.DataEmissao.Should().Be(new DateTime(2021, 2, 15));
                resultado.ValorTotal.Should().Be(750.25m);
                resultado.Numero.Should().Be("456");
            }

            [Test]
            public async Task ProcessarXml_IdGerado_DeveSerGuidSemHifens()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(XmlNFSeValido);

                // Assert
                resultado.Id.Should().NotContain("-");
                resultado.Id.Length.Should().Be(32); // GUID sem hífens tem 32 caracteres
            }

            [Test]
            public async Task ProcessarXml_XmlNFSeValido_DevePreserverXmlOriginal()
            {
                // Act
                var resultado = await _strategy.ProcessarXml(XmlNFSeValido);

                // Assert
                resultado.Raw.Should().Be(XmlNFSeValido);
            }

            [Test]
            public async Task ProcessarXml_XmlSemCamposObrigatorios_DeveRetornarValoresPadrao()
            {
                // Arrange
                var xmlSemCampos = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <CompNfse xmlns=""http://www.abrasf.org.br/nfse.xsd"">
                </CompNfse>";

                // Act
                var resultado = await _strategy.ProcessarXml(xmlSemCampos);

                // Assert
                resultado.Should().NotBeNull();
                resultado.Tipo.Should().Be("nfse");
                resultado.Chave.Should().BeNull();
                resultado.CnpjEmitente.Should().BeNull();
                resultado.Destinatario.Should().BeNull();
                resultado.ValorTotal.Should().Be(0m);
                resultado.Numero.Should().BeNull();
                resultado.Serie.Should().BeNull();
                resultado.DataEmissao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
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
            public async Task ProcessarXml_XmlComValorDecimalInvalido_DeveUsarZeroComoValorPadrao()
            {
                // Arrange
                var xmlComValorInvalido = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <CompNfse>
                    <CodigoVerificacao>TEST123</CodigoVerificacao>
                </CompNfse>";

                // Act
                var resultado = await _strategy.ProcessarXml(xmlComValorInvalido);

                // Assert
                resultado.ValorTotal.Should().Be(0m);
            }

            [Test]
            public async Task ProcessarXml_MultiplasChamadasMesmoXml_DeveGerarIdsUnicos()
            {
                // Act
                var resultado1 = await _strategy.ProcessarXml(XmlNFSeValido);
                var resultado2 = await _strategy.ProcessarXml(XmlNFSeValido);

                // Assert
                resultado1.Id.Should().NotBe(resultado2.Id);
            }
        }

        [TestFixture]
        public class IntegrationTests : NFSeStrategyTests
        {
            

            [Test]
            public async Task FluxoCompleto_XmlNaoNFSe_NaoDeveProcessar()
            {
                // Arrange
                var xmlNaoNFSe = "<nfe xmlns=\"http://www.portalfiscal.inf.br/nfe\"></nfe>";

                // Act
                var podeProcessar = await _strategy.PodeProcessar(xmlNaoNFSe);

                // Assert
                podeProcessar.Should().BeFalse();
            }

            [Test]
            public async Task FluxoCompleto_DiferentesEstruturasXml_DeveProcessarTodas()
            {
                // Arrange
                var xmls = new[] { XmlNFSeValido, XmlNFSeSemNamespace, XmlNFSeEstruturaDiferente };

                // Act & Assert
                foreach (var xml in xmls)
                {
                    var podeProcessar = await _strategy.PodeProcessar(xml);
                    podeProcessar.Should().BeTrue($"XML deveria ser processável: {xml.Substring(0, Math.Min(100, xml.Length))}...");

                    var documento = await _strategy.ProcessarXml(xml);
                    documento.Should().NotBeNull();
                    documento.Tipo.Should().Be("nfse");
                    documento.Id.Should().NotBeNullOrEmpty();
                }
            }
        }
    }
}