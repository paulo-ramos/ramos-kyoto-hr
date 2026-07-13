namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.EnableCompany;

public record EnableCompanyByIdResult(
    Guid Id,
    DateOnly EffectiveStartDate,
    string RazaoSocial, 
    string CnpjFormatado,
    bool IsActive,
    DateTime CreatedAt, 
    DateTime? UpdatedAt);