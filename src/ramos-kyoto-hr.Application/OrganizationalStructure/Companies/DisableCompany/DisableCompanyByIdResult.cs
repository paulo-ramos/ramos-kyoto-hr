namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.DisableCompany;

public record DisableCompanyByIdResult(
    Guid Id,
    DateOnly EffectiveStartDate,
    string RazaoSocial, 
    string CnpjFormatado,
    bool IsActive,
    DateTime CreatedAt
);