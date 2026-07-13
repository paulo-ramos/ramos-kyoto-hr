using ramos_kyoto_hr.Domain.Entities;
using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Domain.Repositories;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;

public class RegisterCompanyUseCase : IRegisterCompanyUseCase
{
    private readonly ICompanyRepository _companyRepository;

    public RegisterCompanyUseCase(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    }

    public async Task<RegisterCompanyResult> ExecuteAsync(RegisterCompanyInput companyInput)
    {
        if (companyInput == null)
            throw new ArgumentNullException(nameof(companyInput), "Os dados de entrada são obrigatórios.");

        var cnpj = Cnpj.Create(companyInput.Cnpj);
        var razaoSocial = RazaoSocial.Create(companyInput.RazaoSocial);

        var existingCompany = await _companyRepository.GetByCnpjAsync(cnpj);
        if (existingCompany != null)
        {
            throw new InvalidOperationException($"Já existe uma empresa cadastrada com o CNPJ {cnpj}.");
        }

        var company = new Company(companyInput.EffectiveStartDate, razaoSocial, cnpj);

        await _companyRepository.AddAsync(company);

        return new RegisterCompanyResult(
            company.Id,
            company.EffectiveStartDate,
            company.Cnpj,
            company.RazaoSocial,
            company.CreatedAt
        );
    }
}