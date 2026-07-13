using Microsoft.AspNetCore.Mvc;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.DisableCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.EnableCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyByCnpj;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyById;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.UpdateCompanyName;

namespace ramos_kyoto_hr.WebApi.Controllers.OrganizationalStructure;

[ApiController]
[Route("api/organization-structure/[controller]")] 

public class CompaniesController : ControllerBase
{
    public CompaniesController()
    {
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCompanyInput companyInput,
        [FromServices] IRegisterCompanyUseCase userCase)
    {
        var result = await userCase.ExecuteAsync(companyInput);
        return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
    }
    
    [HttpPut("{id:guid}/name")]
    public async Task<IActionResult> UpdateName(
        [FromRoute] Guid id,
        [FromBody] UpdateCompanyNameInput companyInput,
        [FromServices] IUpdateCompanyNameUseCase useCase)
    {
        var input = new UpdateCompanyNameInput(companyInput.EffectiveStartDate, companyInput.NewName);
        await useCase.ExecuteAsync(id, input);
        return NoContent();
    }
    
    [HttpPut("{id:guid}/enable")]
    public async Task<IActionResult> Enable(
        [FromRoute] Guid id,
        [FromBody] DateOnly effectiveStartDate,
        [FromServices] IEnableCompanyByIdUseCase useCase)
    {
        var input = new EnableCompanyByIdInput(effectiveStartDate, id);
        var result = await useCase.ExecuteAsync(input);
        return CreatedAtAction(nameof(Enable), new { id = result.Id }, result);
    }
    
    [HttpPut("{id:guid}/disable")]
    public async Task<IActionResult> Disable(
        [FromRoute] Guid id,
        [FromBody] DateOnly effectiveStartDate,
        [FromServices] IDisableCompanyByIdUseCase useCase)
    {
        var input = new DisableCompanyByIdInput(effectiveStartDate, id);
        var result = await useCase.ExecuteAsync(input);
        return CreatedAtAction(nameof(Disable), new { id = result.Id }, result);
    }
    
    [HttpGet("{id:guid}/id")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        [FromServices] IGetCompanyByIdUseCase useCase)
    {
        var input = new GetCompanyByIdInput(id);
        var result = await useCase.ExecuteAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    
    [HttpGet("{cnpj}/cnpj")]
    public async Task<IActionResult> GetByCnpj(
        [FromRoute] string cnpj,
        [FromServices] IGetCompanyByCnpjUseCase useCase)
    {
        var input = new GetCompanyByCnpjInput(cnpj);
        var result = await useCase.ExecuteAsync(input);
        return CreatedAtAction(nameof(GetByCnpj), new { cnpj = result.Cnpj }, result);
    }
}