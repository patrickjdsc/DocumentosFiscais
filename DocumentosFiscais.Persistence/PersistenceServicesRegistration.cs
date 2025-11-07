using DocumentosFiscais.Application.Contracts.Repositories;
using DocumentosFiscais.Persistence.Models;
using DocumentosFiscais.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DocumentosFiscais.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<ConfiguracoesMongoDB>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<ConfiguracoesMongoDB>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings.Database);
            });

            services.AddScoped<IDocumentoFiscalRepository, DocumentoFiscalRepository>();

            return services;
        }
    }
}
