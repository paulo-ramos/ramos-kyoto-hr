using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.EnableCompany;

public class EnableCompanyByIdUseCase : IEnableCompanyByIdUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public EnableCompanyByIdUseCase(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }
    
    public async Task<EnableCompanyByIdResult> ExecuteAsync(EnableCompanyByIdInput companyInput)
    {
        if (companyInput == null)
            throw new ArgumentNullException(nameof(companyInput), "Os dados de entrada são obrigatórios.");

        var company = await _companyRepository.GetByIdAsync(companyInput.CompanyId);
        
        if (company == null)
        {
            throw new KeyNotFoundException($"Empresa com o ID {companyInput.CompanyId} não foi encontrada.");
        }

        company.Enable(companyInput.EffectiveStartDate);

        await _companyRepository.UpdateAsync(company);
        
        return new EnableCompanyByIdResult(
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