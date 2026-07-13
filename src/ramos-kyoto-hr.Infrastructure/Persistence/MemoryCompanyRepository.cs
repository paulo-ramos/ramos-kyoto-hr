using System.Collections.Concurrent;
using ramos_kyoto_hr.Domain.Entities;
using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Infrastructure.Persistence;

public sealed class MemoryCompanyRepository : ICompanyRepository
{
    private static readonly ConcurrentDictionary<Guid, Company> _bancoEmMemoria = new();

    public Task AddAsync(Company company, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_bancoEmMemoria.TryAdd(company.Id, company))
        {
            throw new InvalidOperationException("Falha ao persistir a company na memória.");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_bancoEmMemoria.ContainsKey(company.Id))
        {
            throw new KeyNotFoundException($"Empresa com o ID {company.Id} não foi localizada.");
        }

        _bancoEmMemoria[company.Id] = company;

        return Task.CompletedTask;
    }

    public Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _bancoEmMemoria.TryGetValue(id, out var company);
        return Task.FromResult(company);
    }

    public Task<Company?> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var company = _bancoEmMemoria.Values
            .FirstOrDefault(e => e.Cnpj.Equals(cnpj));

        return Task.FromResult(company);
    }

    public Task<IEnumerable<Company>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult<IEnumerable<Company>>(_bancoEmMemoria.Values.ToList());
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_bancoEmMemoria.TryRemove(id, out _))
        {
            throw new KeyNotFoundException($"Falha ao tentar remover: empresa com ID {id} não encontrada.");
        }

        return Task.CompletedTask;
    }

    public Task<(IEnumerable<Company> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm, 
        bool? isActive, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Validações de entrada
        if (page < 1)
            throw new ArgumentException("O número da página deve ser maior ou igual a 1.", nameof(page));
        
        if (pageSize < 1)
            throw new ArgumentException("O tamanho da página deve ser maior ou igual a 1.", nameof(pageSize));

        // Iniciar com todos os registros
        var query = _bancoEmMemoria.Values.AsEnumerable();

        // Aplicar filtro de termo de busca (CNPJ ou Razão Social)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLowerInvariant();
            
            query = query.Where(company =>
                company.Cnpj.Valor.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                company.RazaoSocial.Valor.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase)
            );
        }

        // Aplicar filtro de status ativo/inativo
        if (isActive.HasValue)
        {
            query = query.Where(company => company.IsActive == isActive.Value);
        }

        // Materializar a query após os filtros para evitar múltipla enumeração
        var filteredCompanies = query
            .OrderBy(c => c.RazaoSocial.Valor) // Ordenação padrão por Razão Social
            .ThenBy(c => c.Cnpj.Valor)
            .ToList();

        // Contar total de registros APÓS os filtros (mas antes da paginação)
        var totalCount = filteredCompanies.Count;

        // Aplicar paginação
        var items = filteredCompanies
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult<(IEnumerable<Company> Items, int TotalCount)>((items, totalCount));
    }
}