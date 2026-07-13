using ramos_kyoto_hr.Domain.Entities;
using ramos_kyoto_hr.Domain.ObjectValue;

namespace ramos_kyoto_hr.Domain.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Company?> GetByCnpjAsync(Cnpj cnpj, CancellationToken cancellationToken = default);
    Task AddAsync(Company company, CancellationToken cancellationToken = default);
    Task UpdateAsync(Company company, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Company> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}