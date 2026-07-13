namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.EnableCompany;

public record EnableCompanyByIdInput(
    DateOnly EffectiveStartDate,
    Guid CompanyId
);