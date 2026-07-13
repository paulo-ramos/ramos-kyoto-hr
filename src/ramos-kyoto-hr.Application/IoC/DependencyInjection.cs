using Microsoft.Extensions.DependencyInjection;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.DisableCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.EnableCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyByCnpj;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyById;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.ListCompanies;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.UpdateCompanyName;

namespace ramos_kyoto_hr.Application.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //Casos de Uso do Contexto: OrganizationalStructure -> Companies
        services.AddScoped<IRegisterCompanyUseCase, RegisterCompanyUseCase>();
        services.AddScoped<IUpdateCompanyNameUseCase, UpdateCompanyNameUseCase>();
        services.AddScoped<IGetCompanyByIdUseCase, GetCompanyByIdUseCase>();
        services.AddScoped<IGetCompanyByCnpjUseCase, GetCompanyByCnpjUseCase>();
        services.AddScoped<IEnableCompanyByIdUseCase, EnableCompanyByIdUseCase>();
        services.AddScoped<IDisableCompanyByIdUseCase, DisableCompanyByIdUseCase>();
        services.AddScoped<IListCompaniesUseCase, ListCompaniesUseCase>();

        return services;
    }
}