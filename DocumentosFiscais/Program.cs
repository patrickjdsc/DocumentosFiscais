using DocumentosFiscais.Application;
using DocumentosFiscais.Infrastructure;
using DocumentosFiscais.Infrastructure.Messaging;
using DocumentosFiscais.Infrastructure.Messaging.Contratos;
using DocumentosFiscais.Infrastructure.Models;
using DocumentosFiscais.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ConfiguracoesRabbitMQ>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
