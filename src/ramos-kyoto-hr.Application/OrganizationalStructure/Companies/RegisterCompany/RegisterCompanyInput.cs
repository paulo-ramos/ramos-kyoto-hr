namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;

public record RegisterCompanyInput(
    DateOnly EffectiveStartDate,
    string Cnpj, 
    string RazaoSocial
);