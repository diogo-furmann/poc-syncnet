# SyncNet API - Quick Start

## O que foi feito

1. API .NET 8 para sincronização bidirecional (protocolo WatermelonDB)
2. SQLite como banco de dados local
3. Entidades: Workspace → Project → Task → Comment
4. Endpoint `GET /sync/pull` - busca mudanças do servidor
5. Endpoint `POST /sync/push` - envia mudanças do cliente
6. Clock Drift Protection (timestamp antes das queries)
7. Server Authority (servidor controla timestamps)
8. Soft Delete (deleção lógica com flag IsDeleted)
9. Transações atômicas (tudo ou nada)
10. Ordenação hierárquica (respeita integridade referencial)
11. Seed automático com dados de exemplo
12. Middleware de erro global com JSON padronizado

## Como rodar

```bash
cd SyncNet.Api
dotnet run --urls "http://localhost:5000"
```

## Como testar

### Rodar test suite (ordem recomendada)
```bash
./tests/run-tests.sh
```

### Rodar testes individuais
```bash
hurl --test tests/pull.hurl        # Read-only tests
hurl --test tests/push.hurl        # Basic operations
hurl --test tests/sync-flow.hurl   # Complete workflow
hurl --test tests/edge-cases.hurl  # Edge cases & errors
```

### Rodar todos de uma vez (ordem alfabética)
```bash
hurl --test tests/*.hurl
```

### Swagger UI (opcional)
```
http://localhost:5000/swagger
```

## Dados de exemplo

A aplicação já vem com dados seed:
- 2 workspaces
- 3 projetos
- 5 tarefas
- 3 comentários

Pronto para testar!
