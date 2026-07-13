using ramos_kyoto_hr.Domain.ObjectValue;

namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.UpdateCompanyName;

public record UpdateCompanyNameResult(
    Guid Id,
    DateOnly EffectiveStartDate,
    string Cnpj,
    string RazaoSocial,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);