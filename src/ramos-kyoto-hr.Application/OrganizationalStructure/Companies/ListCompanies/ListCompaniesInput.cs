namespace ramos_kyoto_hr.Application.OrganizationalStructure.Companies.ListCompanies;

public record ListCompaniesInput(
    string? SearchTerm = null,
    bool? IsActive = null,

    int Page = 1,
    int PageSize = 10
)
{
    public ListCompaniesInput Normalize()
    {
        return this with
        {
            Page = Page < 1 ? 1 : Page,
            PageSize = PageSize is < 1 or > 100 ? 10 : PageSize
        };
    }
}