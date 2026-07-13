# Checklist de Revisao Arquitetural (DDD + Clean Architecture)

Data da revisao: 2026-07-12
Escopo: estado atual da aplicacao apos as ultimas correcoes.

## Como usar

- [ ] Marque cada item conforme for aplicado.
- [ ] Priorize P0 antes de P1/P2.
- [ ] Valide via teste/manual a cada bloco concluido.

---
(corrigir primeiro)

### API e Semantica HTTP
- [ ] Trocar `CreatedAtAction(...)` por respostas corretas em endpoints nao-POST:
  - `Enable` -> `200 Ok(...)` ou `204 NoContent()`
  - `Disable` -> `200 Ok(...)` ou `204 NoContent()`
  - `GetById` -> `200 Ok(...)`
  - `GetByCnpj` -> `200 Ok(...)`
  - Arquivo: `src/ramos-kyoto-hr.WebApi/Controllers/OrganizationalStructure/CompaniesController.cs`

### Rota de CNPJ e contrato de API
- [ ] Padronizar a forma de consulta por CNPJ para evitar ambiguidade:
  - Opcao A: `GET /companies/by-cnpj/{cnpjSemMascara}`
  - Opcao B: `GET /companies?cnpj=...`
- [ ] Garantir consistencia do exemplo no arquivo HTTP (um unico formato de CNPJ).
  - Arquivo: `src/ramos-kyoto-hr.WebApi/ramos-kyoto-hr.WebApi.http`

---
111
## P1 - Media prioridade (melhora de design e manutencao)

### Controller mais limpa (Clean Architecture)
- [ ] Migrar dependencias de `[FromServices]` por action para injecao via construtor na `CompaniesController`.
- [ ] Remover construtor vazio redundante.
  - Arquivo: `src/ramos-kyoto-hr.WebApi/Controllers/OrganizationalStructure/CompaniesController.cs`

### Regras de negocio de estado (DDD)
- [ ] Definir comportamento explicito para estado duplicado:
  - `Enable` quando ja esta ativa
  - `Disable` quando ja esta inativa
- [ ] Escolher estrategia:
  - idempotente silencioso (nao falha), ou
  - excecao de dominio especifica
- [ ] Implementar no dominio (preferencial) e refletir nos use cases.
  - Arquivos:
    - `src/ramos-kyoto-hr.Domain/Entities/Company.cs`
    - `src/ramos-kyoto-hr.Application/OrganizationalStructure/Companies/EnableCompany/EnableCompanyByIdUseCase.cs`
    - `src/ramos-kyoto-hr.Application/OrganizationalStructure/Companies/DisableCompany/DisableCompanyByIdUseCase.cs`

---

## P2 - Baixa prioridade (padronizacao e robustez)

### Tratamento padrao de erros
- [ ] Criar excecoes de aplicacao/dominio para cenarios comuns:
  - entidade nao encontrada
  - validacao de entrada
  - transicao de estado invalida
- [ ] Mapear excecoes para `ProblemDetails` na API (middleware/filtro global).

### Testes
- [ ] Adicionar testes de integracao para rotas:
  - `PUT /{id}/enable`
  - `PUT /{id}/disable`
  - `GET /{id}`
  - `GET by CNPJ`
- [ ] Cobrir cenarios de erro:
  - ID inexistente
  - CNPJ invalido
  - estado ja ativo/inativo

---

## Itens ja verificados como OK (nao entram no backlog imediato)

- [x] Modulo de `DisableCompany` sem typo no path atual.
- [x] Registros DI interface -> implementacao estao consistentes em `Application.IoC`.
- [x] Validacoes de CNPJ existem no value object `Cnpj`.

---

## Definicao de pronto (DoD) para encerrar a revisao

- [ ] Endpoints retornando codigos HTTP coerentes por tipo de operacao.
- [ ] Contrato de CNPJ padronizado em controller e arquivo `.http`.
- [ ] Controller sem service locator por parametro (`[FromServices]`).
- [ ] Regras de estado ativo/inativo definidas e testadas.
- [ ] Erros de negocio mapeados para respostas API padronizadas.
- [ ] Testes cobrindo fluxo feliz + principais cenarios de falha.

