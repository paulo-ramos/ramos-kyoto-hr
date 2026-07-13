namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.ListCompanies;

public interface IListCompaniesUseCase
{
    Task<ListCompaniesResult> ExecuteAsync(ListCompaniesInput input, CancellationToken cancellationToken = default);
}