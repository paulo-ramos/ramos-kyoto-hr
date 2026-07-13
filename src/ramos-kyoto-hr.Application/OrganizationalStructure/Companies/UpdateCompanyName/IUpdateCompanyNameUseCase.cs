namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.UpdateCompanyName;

public interface IUpdateCompanyNameUseCase
{
    Task<UpdateCompanyNameResult> ExecuteAsync(Guid id, UpdateCompanyNameInput companyInput);
}