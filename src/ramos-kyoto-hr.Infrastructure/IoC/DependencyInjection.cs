using Microsoft.Extensions.DependencyInjection;
using ramos_kyoto_hr.Domain.Repositories;
using ramos_kyoto_hr.Infrastructure.Persistence;

namespace ramos_kyoto_hr.Infrastructure.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();

        return services;
    }    
}