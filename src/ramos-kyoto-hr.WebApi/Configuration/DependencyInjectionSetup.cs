using ramos_kyoto_hr.Application.IoC;
using ramos_kyoto_hr.Infrastructure.IoC;

namespace ramos_kyoto_hr.WebApi.Configuration;

public static class DependencyInjectionSetup
{
    public static IServiceCollection RegisterAllServices(this IServiceCollection services)
    {
        // 1. Configurações de infraestrutura da própria API (Apresentação)
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // 2. Registro das dependências das camadas internas (Negócio e Dados)
        services
            .AddApplication()
            .AddInfrastructure();

        return services;
    } 
}