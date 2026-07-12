namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyByCnpj;

public interface IGetCompanyByCnpjUseCase
{
    Task<GetCompanyByCnpjResult> ExecuteAsync(GetCompanyByCnpjInput companyInput);
}