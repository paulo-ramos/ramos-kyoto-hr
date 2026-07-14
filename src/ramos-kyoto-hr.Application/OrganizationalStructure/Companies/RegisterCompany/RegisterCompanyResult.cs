namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;

public record RegisterCompanyResult(
    Guid Id,
    DateOnly EffectiveStartDate,
    string Cnpj,
    string RazaoSocial,
    bool IsActive,
    DateTime CreatedAt
);