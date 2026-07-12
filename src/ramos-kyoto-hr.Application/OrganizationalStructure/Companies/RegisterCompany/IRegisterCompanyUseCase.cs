namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;

public interface IRegisterCompanyUseCase
{
    Task<RegisterCompanyResult> ExecuteAsync(RegisterCompanyInput companyInput);
}