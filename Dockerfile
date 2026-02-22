FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copia somente o csproj para cache de restore
COPY ["src/Challenge.Api/Challenge.Api.csproj", "src/Challenge.Api/"]

RUN dotnet restore "src/Challenge.Api/Challenge.Api.csproj"

# copia o restante
COPY . .

WORKDIR "/src/Challenge.Api"
RUN dotnet build "Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Challenge.Api.dll"]