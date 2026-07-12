namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.DisableCompany;

public interface IDisableCompanyByIdUseCase
{
    Task<DisableCompanyByIdResult> ExecuteAsync(DisableCompanyByIdInput companyInput);
}