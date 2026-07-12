namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;

public record RegisterCompanyResult(
    Guid Id,
    string Cnpj,
    string RazaoSocial,
    DateTime CreatedAt
);