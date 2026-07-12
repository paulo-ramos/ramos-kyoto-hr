namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.DisableCompany;

public record DisableCompanyByIdResult(
    Guid Id, 
    string RazaoSocial, 
    string CnpjFormatado,
    bool IsActive,
    DateTime CreatedAt, 
    DateTime? UpdatedAt);