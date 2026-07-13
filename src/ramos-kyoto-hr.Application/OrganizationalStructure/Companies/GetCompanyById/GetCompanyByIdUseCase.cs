using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyById;

public class GetCompanyByIdUseCase : IGetCompanyByIdUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyByIdUseCase(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    }
    
    public async Task<GetCompanyByIdResult> ExecuteAsync(GetCompanyByIdInput companyInput)
    {
        if (companyInput == null)
            throw new ArgumentNullException(nameof(companyInput), "Input data is required.");

        var company = await _companyRepository.GetByIdAsync(companyInput.CompanyId);

        if (company == null)
        {
            throw new KeyNotFoundException($"Company with ID {companyInput.CompanyId} was not found.");
        }

        return new GetCompanyByIdResult(
            company.Id,
            company.EffectiveStartDate,
            company.Cnpj,
            company.RazaoSocial,
            company.IsActive,
            company.CreatedAt,
            company.UpdatedAt
        );
    }
}