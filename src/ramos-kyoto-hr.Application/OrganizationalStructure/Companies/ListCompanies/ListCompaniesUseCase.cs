using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.ListCompanies;

public class ListCompaniesUseCase : IListCompaniesUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public ListCompaniesUseCase(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ListCompaniesResult> ExecuteAsync(
        ListCompaniesInput input,
        CancellationToken cancellationToken = default)
    {
        var (companies, totalItems) = await _companyRepository.GetPagedAsync(
            input.SearchTerm,
            input.IsActive,
            input.Page,
            input.PageSize,
            cancellationToken
        );

        var companyDtos = companies.Select(c => new CompanyDto(
            Id: c.Id,
            EffectiveStartDate: c.EffectiveStartDate,
            Cnpj: c.Cnpj,
            RazaoSocial: c.RazaoSocial,
            IsActive: c.IsActive,
            CreatedAt: c.CreatedAt

        )).ToList();

        int totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling((double)totalItems / input.PageSize);

        return new ListCompaniesResult(
            Items: companyDtos.AsReadOnly(),
            CurrentPage: input.Page,
            PageSize: input.PageSize,
            TotalItems: totalItems,
            TotalPages: totalPages
        );
    }
}