using ramos_kyoto_hr.Domain.Exceptions;
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

        var currentCompany = await _companyRepository.GetByIdAsync(companyInput.CompanyId);
        
        if (currentCompany == null)
        {
            throw new EntityNotFoundException("Company", companyInput.CompanyId);
        }
        
        var newCompany = currentCompany.Enable(companyInput.EffectiveStartDate);

        if (newCompany == null)
        {
            return new EnableCompanyByIdResult(
                currentCompany.Id,
                currentCompany.EffectiveStartDate,
                currentCompany.Cnpj,
                currentCompany.RazaoSocial,
                currentCompany.IsActive,
                currentCompany.CreatedAt
            );
        }
        
        await _companyRepository.AddAsync(newCompany);
        
        return new EnableCompanyByIdResult(
            newCompany.Id,
            newCompany.EffectiveStartDate,
            newCompany.Cnpj,
            newCompany.RazaoSocial,
            newCompany.IsActive,
            newCompany.CreatedAt
        );
    }
}