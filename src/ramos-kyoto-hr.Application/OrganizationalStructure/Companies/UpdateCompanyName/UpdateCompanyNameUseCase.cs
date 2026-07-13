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

        var company = await _companyRepository.GetByIdAsync(id);
        
        if (company == null)
        {
            throw new EntityNotFoundException("Company", id);
        }

        var razaoSocial = RazaoSocial.Create(companyInput.NewName);
        
        var hasChanged = company.UpdateRazaoSocial(companyInput.EffectiveStartDate, razaoSocial);

        if (hasChanged)
        {
            await _companyRepository.UpdateAsync(company);
        }

        await _companyRepository.UpdateAsync(company);
        
        return new UpdateCompanyNameResult(
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