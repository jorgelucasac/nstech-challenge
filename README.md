 NSTech Challenge - Web API (.NET 10)

Este projeto implementa uma API REST para **autenticação de usuários** e **gestão de pedidos**, seguindo uma arquitetura em camadas (estilo Clean Architecture).

## Visão geral da arquitetura

A solução está dividida em quatro projetos principais:

- **Challenge.Api**: camada de apresentação (HTTP), responsável por controllers, middlewares, versionamento e Swagger.
- **Challenge.Application**: camada de aplicação, responsável por casos de uso, validações, DTOs e fluxo de comandos/queries.
- **Challenge.Domain**: camada de domínio, com entidades, regras de negócio e exceções de domínio.
- **Challenge.Infrastructure**: camada de infraestrutura, com acesso a dados (EF Core + PostgreSQL), autenticação JWT, hashing de senha e logging.

## Camadas e responsabilidades

### 1) Camada `Challenge.Api`

Responsabilidades:

- Expor endpoints REST (`/api/v{version}/auth` e `/api/v{version}/orders`).
- Receber requests HTTP e mapear para comandos/queries da aplicação.
- Configurar pipeline da aplicação:
  - versionamento da API;
  - Swagger/OpenAPI;
  - middlewares customizados (correlação e tratamento global de erro);
  - registro das dependências de Application e Infrastructure.

Principais componentes:

- `Controllers/AuthController`:
  - `POST /auth/register`
  - `POST /auth/token`
- `Controllers/OrdersController` (com `[Authorize]`):
  - `GET /orders/{id}`
  - `GET /orders`
  - `POST /orders`
  - `POST /orders/{id}/confirm`
  - `POST /orders/{id}/cancel`
- `Middlewares/ErrorHandlingMiddleware`: tratamento global de exceções e retorno padronizado HTTP 500.
- `Middlewares/CorrellationMiddleware`: gera/propaga `X-Correlation-Id` para rastreabilidade.

### 2) Camada `Challenge.Application`

Responsabilidades:

- Orquestrar casos de uso (sem depender de infraestrutura concreta).
- Definir contratos (interfaces) de repositórios e serviços.
- Processar comandos e consultas via **MediatR**.
- Validar entrada com **FluentValidation**.
- Retornar resultados padronizados com `Result<T>` / `Error`.

Principais blocos:

- `Features/Commands`: fluxos de escrita (registro, token, criação/confirmação/cancelamento de pedidos).
- `Features/Queries`: fluxos de leitura (listar pedidos e buscar por ID).
- `Contracts/Repositories` e `Contracts/Services`: abstrações para persistência e serviços externos.
- `Behaviors/ValidationBehavior`: pipeline behavior para validação automática dos requests.

### 3) Camada `Challenge.Domain`

Responsabilidades:

- Modelar o núcleo de negócio com entidades e invariantes.
- Centralizar regras de negócio e validações de domínio.
- Evitar dependências de frameworks de infraestrutura.

Principais elementos:

- Entidades: `User`, `Product`, `Order`, `OrderItem`.
- Enum: `OrderStatus`.
- Exceções e constantes de domínio para regras e limites.

### 4) Camada `Challenge.Infrastructure`

Responsabilidades:

- Implementar os contratos da camada de aplicação.
- Persistir dados com **Entity Framework Core** + **PostgreSQL**.
- Configurar autenticação **JWT Bearer**.
- Implementar hashing de senha e logging estruturado.
- Aplicar migrações automaticamente na inicialização.

Principais componentes:

- `Persistence/AppDbContext` + configurações de entidades (`Configurations/*`).
- Repositórios concretos (`Repositories/*`).
- `Auth/JwtTokenService` e `Auth/BCryptPasswordHasher`.
- `Persistence/DbInitializer`: aplica migrações pendentes no startup.

## Bibliotecas e tecnologias utilizadas

### API (`Challenge.Api`)

- `Swashbuckle.AspNetCore`: documentação Swagger/OpenAPI.
- `Asp.Versioning.Mvc` e `Asp.Versioning.Mvc.ApiExplorer`: versionamento de API.

### Aplicação (`Challenge.Application`)

- `MediatR` + `MediatR.Extensions.Microsoft.DependencyInjection`: padrão mediator (commands/queries).
- `FluentValidation.DependencyInjectionExtensions`: validações desacopladas por request.
- `Microsoft.Extensions.Logging.Abstractions`.

### Infraestrutura (`Challenge.Infrastructure`)

- `Npgsql.EntityFrameworkCore.PostgreSQL`: provedor PostgreSQL para EF Core.
- `Microsoft.EntityFrameworkCore.Design` e `Microsoft.EntityFrameworkCore.Tools`.
- `Microsoft.AspNetCore.Authentication.JwtBearer`: autenticação com token JWT.
- `BCrypt.Net-Next`: hashing seguro de senhas.
- `Serilog.AspNetCore`, `Serilog.Settings.Configuration`, `Serilog.Sinks.Console`: observabilidade/logging.

## Pré-requisitos

- **Docker** e **Docker Compose** instalados.
- Porta `5432` livre (PostgreSQL) e porta `8080` livre (API).

## Como executar com Docker Compose

### 1) Subir os containers

Na raiz do projeto:

```bash
docker compose up --build
```

Isso irá subir:

- `postgres` (`postgres:16`) com banco `nstech-challange`.
- `webapi` (build local a partir do `Dockerfile`) exposta em `http://localhost:8080`.

### 2) Verificar saúde/subida

Comandos úteis:

```bash
docker compose ps
docker compose logs -f webapi
docker compose logs -f postgres
```

### 3) Acessar a API e documentação

- Base URL: `http://localhost:8080`
- Swagger UI:
  - `http://localhost:8080/swagger`
  - **Importante (token JWT):** no botão **Authorize** do Swagger, informe **apenas o token** (sem o prefixo `Bearer`). O próprio Swagger adiciona o prefixo automaticamente no header `Authorization`.

### 4) Encerrar ambiente

```bash
docker compose down
```

Para remover também o volume do banco de dados:

```bash
docker compose down -v
```

## Fluxo de autenticação (resumo)

1. Registrar usuário em `POST /api/v1/auth/register`.
2. Gerar token em `POST /api/v1/auth/token`.
3. Enviar `Authorization: Bearer <token>` para acessar endpoints protegidos de pedidos.

## Testes

O repositório possui projeto de testes unitários em:

- `tests/Challenge.UnitTests`

Execução local (opcional):

```bash
dotnet test
```
