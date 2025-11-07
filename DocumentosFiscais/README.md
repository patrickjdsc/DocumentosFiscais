# API de Documentos Fiscais

API REST para recebimento, processamento e consulta de documentos fiscais eletrônicos (NFe, CTe, NFSe).

## Funcionalidades

- **Recebimento**: Upload e processamento de arquivos XML fiscais
- **Consulta**: Busca paginada com filtros por data, CNPJ e tipo
- **Detalhamento**: Consulta individual de documentos por ID
- **Atualização**: Reprocessamento de documentos existentes
- **Exclusão**: Remoção de documentos fiscais

##  Pré-requisitos

- .NET 8.0
- MongoDB
- RabbitMQ

##  Configuração

Configure as strings de conexão para o rabbitMq e o MongoDB  no arquivo `appsettings.json`:

caso queira utilizar o docker para subir imagens do MongoDB e o RabbitMQ, utilize o arquivo `docker-compose.yml` disponível na raiz do projeto.

cd .\DocumentosFiscais\
docker compose up

Para executar a aplicação localmente, utilize o comando:
dotnet run

## Decisões de arquitetura/modelagem
 
- **Clean Architecture**: Separação em camadas (API, Application, Domain, Infrastructure) para melhor manutenibilidade e testabilidade
- **CQRS Pattern**: Separação entre comandos (write) e queries (read)
- **MongoDB**: Escolhido pela natureza do banco e para acenar também o uso da tecnologia uma vez que minha especialidade é sql server
 
##  Possíveis melhorias se tivesse mais tempo


- Testes Unitários e de integração: Nessa entrega os testes unitários foram insuficientes. 
- Autenticação JWT: Implementar autenticação baseada em tokens
- Caching: Implementar Redis para cache de consultas frequentes
- Subir Xmls em Lote: Operações em lote para upload múltiplo de documentos
- Logging Estruturado: Ficou sem Log. Implementar Serilog com structured logging
- Rate Limiting: Proteção contra abuse da API
- Auditoria: Log completo de todas as operações realizadas
- Métricas: API sem monitoramento de performance e nem health
- Health Checks: Endpoints de saúde para MongoDB e RabbitMQ
