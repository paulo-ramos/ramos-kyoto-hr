using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyByCnpj;

public class GetCompanyByCnpjUseCase : IGetCompanyByCnpjUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyByCnpjUseCase(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<GetCompanyByCnpjResult> ExecuteAsync(GetCompanyByCnpjInput companyInput)
    {
        if (companyInput == null)
            throw new ArgumentNullException(nameof(companyInput), "Input data is required.");

        var cnpj = Cnpj.Create(companyInput.Cnpj);
        
        var company = await _companyRepository.GetByCnpjAsync(cnpj);

        if (company == null)
        {
            throw new KeyNotFoundException($"Company with ID {cnpj} was not found.");
        }

        return new GetCompanyByCnpjResult(
            company.Id,
            company.Cnpj,
            company.RazaoSocial,
            company.IsActive,
            company.CreatedAt,
            company.UpdatedAt
        );
    }
}