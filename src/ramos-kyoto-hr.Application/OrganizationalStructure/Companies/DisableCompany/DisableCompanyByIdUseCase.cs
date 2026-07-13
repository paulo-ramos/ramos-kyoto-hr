using ramos_kyoto_hr.Domain.Exceptions;
using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.DisableCompany;

public class DisableCompanyByIdUseCase : IDisableCompanyByIdUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public DisableCompanyByIdUseCase(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }
    
    public async Task<DisableCompanyByIdResult> ExecuteAsync(DisableCompanyByIdInput companyInput)
    {
        if (companyInput == null)
            throw new ArgumentNullException(nameof(companyInput), "Os dados de entrada são obrigatórios.");

        var company = await _companyRepository.GetByIdAsync(companyInput.CompanyId);
        
        if (company == null)
        {
            throw new EntityNotFoundException("Company", companyInput.CompanyId);
        }
        
        var hasChanged = company.Disable(companyInput.EffectiveStartDate);
        
        if (hasChanged)
        {
            await _companyRepository.UpdateAsync(company);
        }

        return new DisableCompanyByIdResult(
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