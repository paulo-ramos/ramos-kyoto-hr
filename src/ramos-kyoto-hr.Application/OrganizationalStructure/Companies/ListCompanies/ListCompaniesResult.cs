namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.ListCompanies;

public record ListCompaniesResult(
    IReadOnlyCollection<CompanyDto> Items,
    int CurrentPage,
    int PageSize,
    int TotalItems,
    int TotalPages
);

public record CompanyDto(
    Guid Id,
    DateOnly EffectiveStartDate,
    string Cnpj,
    string RazaoSocial,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);