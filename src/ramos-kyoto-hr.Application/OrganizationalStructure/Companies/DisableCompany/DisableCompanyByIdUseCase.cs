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

        var currentCompany = await _companyRepository.GetByIdAsync(companyInput.CompanyId);
        
        if (currentCompany == null)
        {
            throw new EntityNotFoundException("Company", companyInput.CompanyId);
        }
        
        var newCompany = currentCompany.Disable(companyInput.EffectiveStartDate);
        
        if (newCompany == null)
        {
            return new DisableCompanyByIdResult(
                currentCompany.Id,
                currentCompany.EffectiveStartDate,
                currentCompany.Cnpj,
                currentCompany.RazaoSocial,
                currentCompany.IsActive,
                currentCompany.CreatedAt
            );
        }

        await _companyRepository.AddAsync(newCompany);

        return new DisableCompanyByIdResult(
            newCompany.Id,
            newCompany.EffectiveStartDate,
            newCompany.Cnpj,
            newCompany.RazaoSocial,
            newCompany.IsActive,
            newCompany.CreatedAt
        );
    }
}