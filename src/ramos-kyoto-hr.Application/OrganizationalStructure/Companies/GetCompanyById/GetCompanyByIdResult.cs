namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyById;

public record GetCompanyByIdResult(
    Guid Id,
    DateOnly EffectiveStartDate,
    string Cnpj,
    string RazaoSocial,
    bool IsActive,
    DateTime CreatedAt
);