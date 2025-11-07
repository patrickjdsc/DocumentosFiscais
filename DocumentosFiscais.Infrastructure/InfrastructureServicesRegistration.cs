using DocumentosFiscais.Application.Contracts.Services;
using DocumentosFiscais.Infrastructure.Messaging;
using DocumentosFiscais.Infrastructure.Messaging.Contratos;
using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal;
using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentosFiscais.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddScoped<IDocumentoFiscalPublisher, DocumentoFiscalPublisher>();

            services.AddTransient<IProcessarTipoDocumentoFiscal, NFeStrategy>();
            services.AddTransient<IProcessarTipoDocumentoFiscal, CTeStrategy>();
            services.AddTransient<IProcessarTipoDocumentoFiscal, NFSeStrategy>();

            services.AddHostedService<DocumentoFiscalConsumer>();
            services.AddScoped<IProcessarXmlDocumentoFiscal, ProcessarXmlDocumentoFiscal>();

            return services;
        }
    }
}
