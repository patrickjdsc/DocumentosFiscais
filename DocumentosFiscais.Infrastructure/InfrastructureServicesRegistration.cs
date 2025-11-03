using DocumentosFiscais.Application.Contracts.Services;
using DocumentosFiscais.Infrastructure.Services.ProcessarXmlDocumentoFiscal;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentosFiscais.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IProcessarXmlDocumentoFiscal, ProcessarXmlDocumentoFiscal>();

            return services;
        }
    }
}
