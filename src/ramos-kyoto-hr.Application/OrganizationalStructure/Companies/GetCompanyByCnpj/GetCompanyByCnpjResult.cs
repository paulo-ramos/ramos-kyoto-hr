namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyByCnpj;

public record GetCompanyByCnpjResult(
    Guid CompanyId,
    DateOnly EffectiveStartDate,
    string Cnpj,
    string RazaoSocial,
    bool IsActive,
    DateTime CreatedAt
);