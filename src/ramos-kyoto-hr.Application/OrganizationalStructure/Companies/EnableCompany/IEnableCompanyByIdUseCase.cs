namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.EnableCompany;

public interface IEnableCompanyByIdUseCase
{
    Task<EnableCompanyByIdResult> ExecuteAsync(EnableCompanyByIdInput companyInput);
}