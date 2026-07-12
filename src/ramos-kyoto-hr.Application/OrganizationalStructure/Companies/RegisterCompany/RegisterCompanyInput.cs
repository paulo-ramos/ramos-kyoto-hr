namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;

public record RegisterCompanyInput(
    string Cnpj, 
    string RazaoSocial
);