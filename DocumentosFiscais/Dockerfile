FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DocumentosFiscais/DocumentosFiscais.csproj", "DocumentosFiscais/"]
RUN dotnet restore "./DocumentosFiscais/DocumentosFiscais.csproj"
COPY . .
WORKDIR "/src/DocumentosFiscais"
RUN dotnet build "./DocumentosFiscais.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DocumentosFiscais.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Define ambiente para produção (usará appsettings.json)
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "DocumentosFiscais.dll"]
