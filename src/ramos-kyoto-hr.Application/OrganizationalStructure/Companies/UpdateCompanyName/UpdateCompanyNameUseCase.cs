using ramos_kyoto_hr.Domain.Exceptions;
using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.UpdateCompanyName;

public class UpdateCompanyNameUseCase : IUpdateCompanyNameUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyNameUseCase(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    }

    public async Task<UpdateCompanyNameResult> ExecuteAsync(Guid id, UpdateCompanyNameInput companyInput)
    {
        if (companyInput == null)
            throw new ArgumentNullException(nameof(companyInput), "Os dados de entrada são obrigatórios.");

        var currentCompany = await _companyRepository.GetByIdAsync(id);
        
        if (currentCompany == null)
        {
            throw new EntityNotFoundException("Company", id);
        }

        var razaoSocial = RazaoSocial.Create(companyInput.NewName);
        
        var newCompany = currentCompany.UpdateRazaoSocial(companyInput.EffectiveStartDate, razaoSocial);

        if (newCompany == null)
        {
            return new UpdateCompanyNameResult(
                currentCompany.Id,
                currentCompany.EffectiveStartDate,
                currentCompany.Cnpj,
                currentCompany.RazaoSocial,
                currentCompany.IsActive,
                currentCompany.CreatedAt
            );
        }

        await _companyRepository.AddAsync(newCompany);
        
        return new UpdateCompanyNameResult(
            newCompany.Id,
            newCompany.EffectiveStartDate,
            newCompany.Cnpj,
            newCompany.RazaoSocial,
            newCompany.IsActive,
            newCompany.CreatedAt
        );
    }
}