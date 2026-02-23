FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia somente o csproj (cache de restore)
COPY ./src/Challenge.Api/Challenge.Api.csproj ./src/Challenge.Api/

# Se você usa PackageReference em outros projetos, copie também os csproj deles (opcional, mas recomendado)
COPY ./src/Challenge.Application/*.csproj ./src/Challenge.Application/
COPY ./src/Challenge.Domain/*.csproj ./src/Challenge.Domain/
COPY ./src/Challenge.Infrastructure/*.csproj ./src/Challenge.Infrastructure/

RUN dotnet restore ./src/Challenge.Api/Challenge.Api.csproj

# Copia todo o restante do repo
COPY . .

# Debug (descomente se precisar enxergar estrutura dentro do container)
# RUN ls -la ./src/Challenge.Api && find ./src -maxdepth 3 -name "*.csproj" -print

WORKDIR /src/src/Challenge.Api
RUN dotnet build Challenge.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Challenge.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Challenge.Api.dll"]