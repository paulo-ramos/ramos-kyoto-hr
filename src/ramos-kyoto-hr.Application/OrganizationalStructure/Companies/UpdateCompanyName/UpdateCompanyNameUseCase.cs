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

    public async Task ExecuteAsync(UpdateCompanyNameInput companyInput)
    {
        if (companyInput == null)
            throw new ArgumentNullException(nameof(companyInput), "Os dados de entrada são obrigatórios.");

        var company = await _companyRepository.GetByIdAsync(companyInput.Id);
        
        if (company == null)
        {
            throw new KeyNotFoundException($"Empresa com o ID {companyInput.Id} não foi encontrada.");
        }

        var razaoSocial = RazaoSocial.Create(companyInput.NewName);
        
        company.UpdateRazaoSocial(razaoSocial);

        await _companyRepository.UpdateAsync(company);
    }
}