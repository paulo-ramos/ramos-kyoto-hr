namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyById;

public record GetCompanyByIdResult(
    Guid Id,
    string Cnpj,
    string RazaoSocial,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);