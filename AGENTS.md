# AGENTS.md - Instruções para o Agente

## Fluxo de Desenvolvimento - TDD (Test Driven Development)

**OBRIGATÓRIO**: Para toda nova feature, o fluxo DEVE ser sempre:

1. **Escrever testes primeiro** (vermelho)
2. **Implementar código** (verde)
3. **Refatorar** (azul)

```
Testes → Implementação → Refatoração
```

### Passos Obrigatórios

1. Escrever teste que falha (vermelhor)
2. Implementar código mínimo para passar (verde)
3. Executar testes: `dotnet test` ou `npm run test`
4. Se passar, refatorar código se necessário
5. Executar lint/typecheck: `dotnet build` e `npm run lint`

### Estrutura de Testes

**.NET** - xUnit + FluentAssertions + Moq
```
tests/
├── Crm.Api.Tests/
│   └── Services/
│       └── ProdutoServiceTests.cs
```

**React** - Vitest + React Testing Library
```
Crm.Web.Tests/
├── services/
│   └── auth.test.ts
└── components/
    └── Button.test.tsx
```

### Checklist pré-implementação

- [ ] Teste escrito e falhando
- [ ] Implementação faz teste passar
- [ ] `dotnet test` / `npm run test` passando
- [ ] `dotnet build` / `npm run lint` passando
- [ ] Código refatorado se necessário

### Regra de Ouro

**NUNCA** escreva implementação sem antes escrever o teste.
Se não há teste, não há implementação.

---

# SPEC-011: Arquitetura do Projeto

## Visão Geral

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENTE                                  │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │                    React (SPA)                          │  │
│  │  ├── Pages (Dashboard, Pedidos, Leads, etc)              │  │
│  │  ├── Components (UI reused)                             │  │
│  │  ├── Hooks (auth, api)                                   │  │
│  │  └── Services (API calls)                                │  │
│  └─────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ HTTPS (JSON)
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                        SERVIDOR                                 │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │                 .NET 10 Minimal API                      │  │
│  │  ├── Controllers/Endpoints                            │  │
│  │  ├── Middleware (JWT, CORS, Error)                      │  │
│  │  └── Swagger (OpenAPI)                                  │  │
│  └─────────────────────────────────────────────────────────┘  │
│                              │                                 │
│  ┌──────────────────┐  ┌────┴──────────────────────────┐  │
│  │    Services      │  │         DTOs                      │  │
│  │  (Business Logic)│  │    (Data Transfer)               │  │
│  └──────────────────┘  └─────────────────────────────────┘  │
│           │                                                    │
│  ┌────────┴────────────────────────────────────────────────┐ │
│  │              Entity Framework Core                        │  │
│  │              (PostgreSQL)                              │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## Estrutura de Pastas

```
crm/
├── src/
│   ├── Crm.Domain/           # Entidades, interfaces, enums
│   ├── Crm.Application/       # Services, DTOs, mappings
│   ├── Crm.Infrastructure/    # EF Core, config, migrations
│   ├── Crm.Api/             # Controllers, endpoints
│   └── Crm.Web/             # React SPA
│       ├── src/
│       ├── public/
│       └── vite.config.ts
│
├── specs/
├── docker-compose.yml
├── Crm.sln
└── README.md
```

## Clean Architecture - Projetos .NET

### Visão Geral dos Projetos

```
┌─────────────────────────────────────────────────────────────┐
│                      CLIENTE (React)                      │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Crm.Api (Presentation)                │
│              Controllers, Minimal APIs, Filters            │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 Crm.Application (Application)               │
│           Services, DTOs, Requests, Responses               │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   Crm.Domain (Domain)                       │
│        Entities, Interfaces, Enums, Value Objects          │
└─────────────────────────────────────────────────────────────┘
                              ▲
                              │
┌─────────────────────────────────────────────────────────────┐
│                Crm.Infrastructure (Infrastructure)            │
│          DbContext, Configurations, Migrations              │
└─────────────────────────────────────────────────────────────┘
```

### Dependências entre Projetos

```
Crm.Api            → Crm.Application
Crm.Application    → Crm.Domain
Crm.Infrastructure  → Crm.Domain
Crm.Domain         → (nenhuma dependência externa)
```

### Estrutura Detalhada por Projeto

#### CRM.Domain
```
Crm.Domain/
├── Entities/
│   ├── Usuario.cs
│   ├── Lead.cs
│   ├── Cliente.cs
│   ├── Produto.cs
│   ├── Pedido.cs
│   ├── PedidoItem.cs
│   └── LinkPagamento.cs
├── Enums/
│   ├── OrigemLead.cs
│   └── StatusPedido.cs
├── Interfaces/
│   ├── IUsuarioRepository.cs
│   ├── ILeadRepository.cs
│   ├── IClienteRepository.cs
│   ├── IProdutoRepository.cs
│   ├── IPedidoRepository.cs
│   └── IUnitOfWork.cs
└── ValueObjects/
    └── Telefone.cs
```

#### CRM.Application
```
Crm.Application/
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IProdutoService.cs
│   │   ├── ILeadService.cs
│   │   ├── IClienteService.cs
│   │   ├── IPedidoService.cs
│   │   ├── ICheckoutService.cs
│   │   ├── IDashboardService.cs
│   │   └── IWebhookService.cs
│   └── Implementations/
│       ├── AuthService.cs
│       ├── ProdutoService.cs
│       ├── LeadService.cs
│       ├── ClienteService.cs
│       ├── PedidoService.cs
│       ├── CheckoutService.cs
│       ├── DashboardService.cs
│       └── WebhookService.cs
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequest.cs
│   │   └── LoginResponse.cs
│   ├── Produto/
│   │   ├── ProdutoDto.cs
│   │   ├── CreateProdutoRequest.cs
│   │   └── UpdateProdutoRequest.cs
│   ├── Lead/
│   ├── Cliente/
│   ├── Pedido/
│   └── Dashboard/
├── Mappings/
│   └── MappingProfile.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

#### CRM.Infrastructure
```
Crm.Infrastructure/
├── Data/
│   ├── CrmDbContext.cs
│   └── DesignTimeDbContextFactory.cs
├── Configurations/
│   ├── UsuarioConfiguration.cs
│   ├── LeadConfiguration.cs
│   ├── ClienteConfiguration.cs
│   ├── ProdutoConfiguration.cs
│   ├── PedidoConfiguration.cs
│   ├── PedidoItemConfiguration.cs
│   └── LinkPagamentoConfiguration.cs
├── Repositories/
│   ├── UsuarioRepository.cs
│   ├── LeadRepository.cs
│   ├── ClienteRepository.cs
│   ├── ProdutoRepository.cs
│   └── PedidoRepository.cs
├── Migrations/
│   └── (...)
└── DependencyInjection/
    └── InfrastructureExtensions.cs
```

#### CRM.Api
```
Crm.Api/
├── Controllers/
│   ├── AuthController.cs
│   ├── ProdutosController.cs
│   ├── LeadsController.cs
│   ├── ClientesController.cs
│   ├── PedidosController.cs
│   ├── CheckoutController.cs
│   ├── WebhookController.cs
│   └── DashboardController.cs
├── Filters/
│   └── ValidationFilter.cs
├── Middleware/
│   └── ErrorHandlingMiddleware.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── appsettings.Development.json
└── appsettings.json
```

## Arquitetura Backend

### Padrão de camadas

```
Controllers (API Layer)
       │
       ▼
   Services (Business Logic)
       │
       ▼
 Repository Pattern (EF Core)
       │
       ▼
    PostgreSQL
```

### Responsabilidades

| Projeto | Responsabilidade |
|---------|------------------|
| Crm.Domain | Entidades, interfaces de repositório, enums |
| Crm.Application | Services, DTOs, validação, regras de negócio |
| Crm.Infrastructure | EF Core, repositories, migrations |
| Crm.Api | Controllers, HTTP, autenticação |

### Estrutura de um Service (Application)

```csharp
// Crm.Application/Services/Interfaces/IProdutoService.cs
namespace Crm.Application.Services.Interfaces;

public interface IProdutoService
{
    Result<List<ProdutoDto>> GetAll(bool incluirInativos = false);
    Result<ProdutoDto> GetById(Guid id);
    Result<ProdutoDto> Create(CreateProdutoRequest request);
    Result<ProdutoDto> Update(Guid id, UpdateProdutoRequest request);
    Result Delete(Guid id);
}

// Crm.Application/Services/Implementations/ProdutoService.cs
namespace Crm.Application.Services.Implementations;

public class ProdutoService : IProdutoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProdutoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Result<List<ProdutoDto>> GetAll(bool incluirInativos = false)
    {
        var query = _context.Produtos.AsQueryable();
        
        if (!incluirInativos)
            query = query.Where(x => x.Ativo);

        var produtos = query.OrderBy(x => x.Nome).ToList();
        return Result.Ok(_mapper.Map<List<ProdutoDto>>(produtos));
    }

    public Result<ProdutoDto> GetById(Guid id)
    {
        var produto = _context.Produtos.Find(id);
        return produto is not null
            ? Result.Ok(_mapper.Map<ProdutoDto>(produto))
            : Result.Fail(Errors.Produto.NaoEncontrado(id));
    }

    public Result<ProdutoDto> Create(CreateProdutoRequest request)
    {
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Preco = request.Preco,
            Descricao = request.Descricao,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _context.Produtos.Add(produto);
        _context.SaveChanges();

        return Result.Ok(_mapper.Map<ProdutoDto>(produto));
    }
}
```

### Estrutura de Entidade (Domain)

```csharp
// Crm.Domain/Entities/Produto.cs
namespace Crm.Domain.Entities;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
}
```

### Estrutura de Repository (Infrastructure)

```csharp
// Crm.Domain/Interfaces/IProdutoRepository.cs
namespace Crm.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<List<Produto>> GetAllAsync(bool incluirInativos = false);
    Task<Produto?> GetByIdAsync(Guid id);
    void Add(Produto produto);
    void Update(Produto produto);
}

// Crm.Infrastructure/Repositories/ProdutoRepository.cs
namespace Crm.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly CrmDbContext _context;

    public ProdutoRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task<List<Produto>> GetAllAsync(bool incluirInativos = false)
    {
        var query = _context.Produtos.AsQueryable();
        
        if (!incluirInativos)
            query = query.Where(x => x.Ativo);

        return await query.OrderBy(x => x.Nome).ToListAsync();
    }
    
    public void Add(Produto produto) => _context.Produtos.Add(produto);
    public void Update(Produto produto) => _context.Produtos.Update(produto);
}
```

### Criação dos Projetos

```bash
# Criar solução
dotnet new sln -n Crm

# Criar projetos
dotnet new classlib -n Crm.Domain -o src/Crm.Domain
dotnet new classlib -n Crm.Application -o src/Crm.Application
dotnet new classlib -n Crm.Infrastructure -o src/Crm.Infrastructure
dotnet new webapi -n Crm.Api -o src/Crm.Api

# Adicionar à solução
dotnet sln add src/Crm.Domain/Crm.Domain.csproj
dotnet sln add src/Crm.Application/Crm.Application.csproj
dotnet sln add src/Crm.Infrastructure/Crm.Infrastructure.csproj
dotnet sln add src/Crm.Api/Crm.Api.csproj

# Adicionar referências entre projetos
dotnet add src/Crm.Application/Crm.Application.csproj reference src/Crm.Domain/Crm.Domain.csproj
dotnet add src/Crm.Infrastructure/Crm.Infrastructure.csproj reference src/Crm.Domain/Crm.Domain.csproj
dotnet add src/Crm.Api/Crm.Api.csproj reference src/Crm.Application/Crm.Application.csproj
dotnet add src/Crm.Api/Crm.Api.csproj reference src/Crm.Infrastructure/Crm.Infrastructure.csproj

# Adicionar pacotes NuGet

# Crm.Domain
dotnet add src/Crm.Domain/Crm.Domain.csproj package Microsoft.Extensions.DependencyInjection.Abstractions

# Crm.Application  
dotnet add src/Crm.Application/Crm.Application.csproj package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add src/Crm.Application/Crm.Application.csproj package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add src/Crm.Application/Crm.Application.csproj package FluentValidation.DependencyInjectionExtensions

# Crm.Infrastructure
dotnet add src/Crm.Infrastructure/Crm.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add src/Crm.Infrastructure/Crm.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/Crm.Infrastructure/Crm.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design

# Crm.Api
dotnet add src/Crm.Api/Crm.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/Crm.Api/Crm.Api.csproj package Swashbuckle.AspNetCore
```

### Criação dos Projetos

```bash
# Criar solução
dotnet new sln -n Crm

# Criar projetos
dotnet new classlib -n Crm.Domain -o src/Crm.Domain
dotnet new classlib -n Crm.Application -o src/Crm.Application
dotnet new classlib -n Crm.Infrastructure -o src/Crm.Infrastructure
dotnet new webapi -n Crm.Api -o src/Crm.Api

# Adicionar à solução
dotnet sln add src/Crm.Domain/Crm.Domain.csproj
dotnet sln add src/Crm.Application/Crm.Application.csproj
dotnet sln add src/Crm.Infrastructure/Crm.Infrastructure.csproj
dotnet sln add src/Crm.Api/Crm.Api.csproj

# Adicionar referências
dotnet add src/Crm.Application/Crm.Application.csproj reference src/Crm.Domain/Crm.Domain.csproj
dotnet add src/Crm.Infrastructure/Crm.Infrastructure.csproj reference src/Crm.Domain/Crm.Domain.csproj
dotnet add src/Crm.Api/Crm.Api.csproj reference src/Crm.Application/Crm.Application.csproj
dotnet add src/Crm.Api/Crm.Api.csproj reference src/Crm.Infrastructure/Crm.Infrastructure.csproj

# Adicionar pacotes
# Crm.Domain
dotnet add Crm.Domain package Microsoft.Extensions.DependencyInjection.Abstractions

# Crm.Application  
dotnet add Crm.Application package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add Crm.Application package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add Crm.Application package FluentValidation

# Crm.Infrastructure
dotnet add Crm.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add Crm.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add Crm.Infrastructure package Microsoft.EntityFrameworkCore.Design

# Crm.Api
dotnet add Crm.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add Crm.Api package Swashbuckle.AspNetCore
```

### Estrutura de um Service

```csharp
public interface IProdutoService
{
    Result<List<ProdutoDto>> GetAll(bool incluirInativos = false);
    Result<ProdutoDto> GetById(Guid id);
    Result<ProdutoDto> Create(CreateProdutoRequest request);
    Result<ProdutoDto> Update(Guid id, UpdateProdutoRequest request);
    Result Delete(Guid id);
}

public class ProdutoService : IProdutoService
{
    private readonly CrmDbContext _context;

    public ProdutoService(CrmDbContext context)
    {
        _context = context;
    }

    public Result<List<ProdutoDto>> GetAll(bool incluirInativos = false)
    {
        var query = _context.Produtos.AsQueryable();
        
        if (!incluirInativos)
            query = query.Where(x => x.Ativo);

        var produtos = query.OrderBy(x => x.Nome).ToList();
        return Result.Ok(_mapper.Map<List<ProdutoDto>>(produtos));
    }

    public Result<ProdutoDto> GetById(Guid id)
    {
        var produto = _context.Produtos.Find(id);
        return produto is not null
            ? Result.Ok(_mapper.Map<ProdutoDto>(produto))
            : Result.Fail(Errors.Produto.NaoEncontrado(id));
    }
}
```

## Arquitetura Frontend

### Stack Tecnológica

- **Framework**: React 18 + TypeScript + Vite
- **UI Library**: [shadcn/ui](https://ui.shadcn.com/) (Radix UI + Tailwind CSS)
- **Styling**: Tailwind CSS 3.4+
- **State Management**: Zustand
- **Forms**: React Hook Form + Zod
- **HTTP Client**: Axios
- **Routing**: React Router 6

### shadcn/ui

Componentes são instalados individualmente via CLI:

```bash
# Instalar componente
npx shadcn@latest add button
npx shadcn@latest add input
npx shadcn@latest add table
npx shadcn@latest add card
npx shadcn@latest add dialog
npx shadcn@latest add dropdown-menu
npx shadcn@latest add toast
# etc...
```

Estrutura dos componentes UI:

```
Crm.Web/src/
├── components/ui/           # Componentes shadcn/ui
│   ├── button.tsx
│   ├── input.tsx
│   ├── table.tsx
│   ├── card.tsx
│   ├── dialog.tsx
│   ├── dropdown-menu.tsx
│   ├── toast.tsx
│   └── sonner.tsx
│
├── components/ui/advanced/ # Componentes compostos customizados
│   ├── data-table.tsx        # Tabela com ordenação, paginação
│   ├── delete-dialog.tsx     # Dialog de confirmação de exclusão
│   └── form-field.tsx        # Wrapper para inputs com label/error
│
├── components/layout/
│   ├── Header.tsx
│   ├── Sidebar.tsx
│   └── Layout.tsx
│
├── lib/
│   ├── utils.ts              # cn() - utilitário para merge de classes
│   └── api.ts                # Cliente Axios configurado
│
├── hooks/
│   ├── useAuth.ts
│   └── useDebounce.ts
│
├── pages/
│   ├── Dashboard.tsx
│   ├── Login.tsx
│   ├── Produtos/
│   │   ├── index.tsx
│   │   └── [id].tsx
│   └── ...
│
├── services/
│   ├── api/
│   │   ├── index.ts
│   │   ├── auth.ts
│   │   ├── produtos.ts
│   │   └── ...
│   └── types.ts
│
└── layouts/
    ├── AuthLayout.tsx
    └── DashboardLayout.tsx
```

### Padrão de Página

```tsx
// pages/Produtos/index.tsx
import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { produtos } from '@/services/api/produtos';

export function ProdutosIndex() {
  const [items, setItems] = useState<Produto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadItems();
  }, []);

  const loadItems = async () => {
    try {
      const data = await produtos.getAll(true);
      setItems(data);
    } catch {
      setError('Erro ao carregar');
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Carregando...</div>;
  if (error) return <div className="text-red-500">{error}</div>;

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold">Produtos</h1>
        <Button asChild>
          <Link to="/produtos/novo">Novo</Link>
        </Button>
      </div>

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Nome</TableHead>
            <TableHead>Preço</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {items.map((item) => (
            <TableRow key={item.id}>
              <TableCell>{item.nome}</TableCell>
              <TableCell>{item.preco}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
```

### State Management

Usar Zustand para estado global:

```typescript
// hooks/useAuth.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  usuario: { id: string; nome: string; ehAdmin: boolean } | null;
  login: (token: string, usuario: AuthState['usuario']) => void;
  logout: () => void;
}

export const useAuth = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      usuario: null,
      login: (token, usuario) => set({ token, usuario }),
      logout: () => set({ token: null, usuario: null }),
    }),
    { name: 'auth-storage' }
  )
);
```

## Padrões de API

### Response Padrão

```typescript
// Sucesso
{ "data": [...] } // GET list
{ "produto": {...} } // GET single, POST, PUT

// Erro
{ "error": "mensagem" } // 400, 404, 500
{ "errors": {...} } // 422 validation errors
```

### Padrão de Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | /api/{recurso} | Listar todos |
| GET | /api/{recurso}/{id} | Buscar por ID |
| POST | /api/{recurso} | Criar |
| PUT | /api/{recurso}/{id} | Atualizar |
| DELETE | /api/{recurso}/{id} | Deletar |

### Tipos de Parâmetros

- **Query**: Filtros, paginação, ordenação
- **Body**: Dados para criação/atualização
- **Route**: ID do recurso

### Códigos HTTP

| Código | Uso |
|--------|-----|
| 200 | Sucesso (GET, PUT) |
| 201 | Criado (POST) |
| 204 | Sem conteúdo (DELETE) |
| 400 | Erro de request |
| 401 | Não autenticado |
| 403 | Não autorizado |
| 404 | Não encontrado |
| 422 | Erro de validação |
| 500 | Erro interno |

## Banco de Dados

### Conexão

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=crm;Username=postgres;Password=secret"
  }
}
```

### Migrations

```bash
# Criar migration
dotnet ef migrations add NomeDaMigration

# Aplicar
dotnet ef database update

# Reverter
dotnet ef database update NomeAnterior
```

## Configuração de Ambiente

### Variáveis de Ambiente

```bash
# .env
DATABASE_URL=postgresql://user:pass@localhost:5432/crm
JWT_KEY=sua_chave_secreta_minimo_32_caracteres
JWT_ISSUER=Crm
JWT_AUDIENCE=Crm
API_URL=http://localhost:5000
```

### docker-compose.yml

```yaml
version: '3.8'
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: secret
      POSTGRES_DB: crm
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build: ./src/Crm.Api
    ports:
      - "5000:5000"
    depends_on:
      - db
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=crm;Username=postgres;Password=secret

  web:
    build: ./src/Crm.Web
    ports:
      - "5173:5173"
    depends_on:
      - api
```

## Segurança

### Autenticação

- JWT com token de acesso
- Tempo de expiração: 8 horas
- Armazenamento: localStorage (client), httpOnly cookie (opcional)

### Autorização

- Roles: Admin, Vendedor
- Admin: acesso total
- Vendedor: criar pedidos, ver dados

### Validações

- Backend: FluentValidation ou DataAnnotations
- Frontend: HTML5 + custom validation

## Logging e Erros

### Logging

```csharp
// Program.cs
builder.Services.AddLogging();

// Usage
_logger.LogInformation("Mensagem info");
_logger.LogError(ex, "Erro ao processar");
```

### Error Handling

```csharp
// Middleware de erro global
app.UseMiddleware<ErrorHandlingMiddleware>();

// Response de erro
{ "error": "mensagem", "code": "ERROR_CODE" }
```

## Testes

### Estrutura

```
Crm.Api.Tests/
├── Services/
│   └── ProdutoServiceTests.cs
├── Controllers/
│   └── ProdutosControllerTests.cs
└── ...

Crm.Web.Tests/
├── pages/
└── components/
```

### Frameworks

- Backend: xUnit + FluentAssertions
- Frontend: Vitest + React Testing Library

## Padrão Result<T>

### Conceito

O padrão `Result<T>` elimina exceptions para controle de fluxo. Métodos retornam objetos que indicam sucesso ou falha explicitamente.

### Por que usar Result<T>?

| Exceções | Result<T> |
|----------|----------|
| Flow control via throw/catch | Retorno direto com status |
|try/catch em todo lugar | Verificação explícita |
| Stack traces confusas | Erros tipados e descritivos |
| Nullable返回值 tricky | `Result.NotFound()` vs `null` |

### Biblioteca: FluentResults

```bash
dotnet add Crm.Application package FluentResults
```

### Estrutura do Result

```csharp
// Sucesso: Result.Ok(value)
// Falha: Result.Fail(error)
// Assíncrono: Result.OkIfAsync() / Result.FailIfAsync()
```

### Tipos de Erro

```csharp
// Erros pré-definidos (centralizados)
public static class Errors
{
    public static Error NotFound { get; } = new("NotFound", "Recurso não encontrado.");
    public static Error Validation { get; } = new("Validation", "Dados inválidos.");
    public static Error Unauthorized { get; } = new("Unauthorized", "Não autenticado.");
}

// Erros customizados por domínio
public static Error ContaNaoEncontrada(Guid id) => 
    new("ContaNaoEncontrada", $"Conta {id} não encontrada.");
```

### Processamento de Result

```csharp
// Padrão 1: Verificação explícita
var result = await _service.GetByIdAsync(id);
if (result.IsFailed)
    return result.Error.Message;

return result.Value;

// Padrão 2: OnSuccess/OnFailure (fluent)
result
    .OnSuccess(value => DoSomething(value))
    .OnFailure(errors => Log(errors));

// Padrão 3: Bind (evita pirâmide)
var result = await userService.GetUserById(id)
    .Bind(user => orderService.GetOrdersByUser(user.Id));
```

### Service com Result<T>

```csharp
// Interface
public interface IProdutoService
{
    Result<List<ProdutoDto>> GetAll(bool incluirInativos = false);
    Result<ProdutoDto> GetById(Guid id);
    Result<ProdutoDto> Create(CreateProdutoRequest request);
    Result Delete(Guid id);
}

// Implementação
public Result<ProdutoDto> GetById(Guid id)
{
    var produto = _context.Produtos.Find(id);
    return produto is not null
        ? Result.Ok(_mapper.Map<ProdutoDto>(produto))
        : Result.Fail(Errors.NotFound);
}

public Result<List<ProdutoDto>> GetAll(bool incluirInativos = false)
{
    var query = _context.Produtos.AsQueryable();
    
    if (!incluirInativos)
        query = query.Where(x => x.Ativo);

    var produtos = query.OrderBy(x => x.Nome).ToList();
    return Result.Ok(_mapper.Map<List<ProdutoDto>>(produtos));
}
```

### Controller com Result

```csharp
[HttpGet("{id:guid}")]
public IActionResult GetById(Guid id)
{
    var result = _service.GetById(id);
    
    return result.Match(
        onSuccess: Ok,
        onFailure: error => NotFound(new { error.Message })
    );
}

[HttpPost]
public IActionResult Create([FromBody] CreateProdutoRequest request)
{
    var result = _service.Create(request);
    
    return result.Match(
        onSuccess: Created($"/produtos/{result.Value.Id}"),
        onFailure: BadRequest
    );
}
```

### Regras Importantes

- **NUNCA retorne `Result.Ok(null)`** - Use `Result.Fail(NotFound)` se não existe
- **Use `ExceptionalError`** para exceções capturadas que precisam do stack trace
- **Enriqueça erros com `CausedBy`** para encadear erros sem perder contexto
- **Evite `Result` em métodos privados utilitários** que não falham em sentido de domínio

---

## Validação

- [ ] Estrutura de pastas segue padrão
- [ ] Dependency injection configurado
- [ ] Services com interfaces
- [ ] DTOs para transferência
- [ ] docker-compose funcional
- [ ] Variáveis de ambiente usadas
- [ ] JWT configurado
- [ ] Logging implementado