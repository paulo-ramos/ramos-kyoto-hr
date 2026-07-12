namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.UpdateCompanyName;

public record UpdateCompanyNameInput(
    Guid Id,
    string NewName
);