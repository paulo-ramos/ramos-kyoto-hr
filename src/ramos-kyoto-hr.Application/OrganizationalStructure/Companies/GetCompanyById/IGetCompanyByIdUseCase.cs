namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyById;

public interface IGetCompanyByIdUseCase
{
    Task<GetCompanyByIdResult> ExecuteAsync(GetCompanyByIdInput companyInput);
}