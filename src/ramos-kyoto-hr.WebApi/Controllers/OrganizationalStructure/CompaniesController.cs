using Microsoft.AspNetCore.Mvc;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.DisableCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.EnableCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyByCnpj;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.GetCompanyById;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.ListCompanies;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.RegisterCompany;
using ramos_kyoto_hr.Application.OrganizationalStructure.Companies.UpdateCompanyName;

namespace ramos_kyoto_hr.WebApi.Controllers.OrganizationalStructure;

[ApiController]
[Route("api/organization-structure/[controller]")] 

public class CompaniesController : ControllerBase
{
    private readonly IRegisterCompanyUseCase _registerCompanyUseCase;
    private readonly IUpdateCompanyNameUseCase _updateCompanyNameUseCase;
    private readonly IGetCompanyByIdUseCase _getCompanyByIdUseCase;
    private readonly IGetCompanyByCnpjUseCase _getCompanyByCnpjUseCase;
    private readonly IListCompaniesUseCase _listCompaniesUseCase;
    private readonly IEnableCompanyByIdUseCase _enableCompanyByIdUseCase;
    private readonly IDisableCompanyByIdUseCase _disableCompanyByIdUseCase;
    
    public CompaniesController(
        IRegisterCompanyUseCase registerCompanyUseCase, 
        IUpdateCompanyNameUseCase updateCompanyNameUseCase, 
        IGetCompanyByIdUseCase getCompanyByIdUseCase, 
        IGetCompanyByCnpjUseCase getCompanyByCnpjUseCase,
        IListCompaniesUseCase listCompaniesUseCase,
        IEnableCompanyByIdUseCase enableCompanyByIdUseCase, 
        IDisableCompanyByIdUseCase disableCompanyByIdUseCase)
    {
        _registerCompanyUseCase = registerCompanyUseCase;
        _updateCompanyNameUseCase = updateCompanyNameUseCase;
        _getCompanyByIdUseCase = getCompanyByIdUseCase;
        _getCompanyByCnpjUseCase = getCompanyByCnpjUseCase;
        _listCompaniesUseCase = listCompaniesUseCase;
        _enableCompanyByIdUseCase = enableCompanyByIdUseCase;
        _disableCompanyByIdUseCase = disableCompanyByIdUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCompanyInput companyInput)
    {
        var result = await _registerCompanyUseCase.ExecuteAsync(companyInput);
        return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
    }
    
    [HttpPut("{id:guid}/name")]
    public async Task<IActionResult> UpdateName(
        [FromRoute] Guid id,
        [FromBody] UpdateCompanyNameInput companyInput)
    {
        var input = new UpdateCompanyNameInput(companyInput.EffectiveStartDate, companyInput.NewName);
        var result = await _updateCompanyNameUseCase.ExecuteAsync(id, input);
        return Ok(result);
    }
    
    [HttpPut("{id:guid}/enable")]
    public async Task<IActionResult> Enable(
        [FromRoute] Guid id,
        [FromBody] DateOnly effectiveStartDate)
    {
        var input = new EnableCompanyByIdInput(effectiveStartDate, id);
        var result = await _enableCompanyByIdUseCase.ExecuteAsync(input);
        return Ok(result);
    }
    
    [HttpPut("{id:guid}/disable")]
    public async Task<IActionResult> Disable(
        [FromRoute] Guid id,
        [FromBody] DateOnly effectiveStartDate)
    {
        var input = new DisableCompanyByIdInput(effectiveStartDate, id);
        var result = await _disableCompanyByIdUseCase.ExecuteAsync(input);
        return Ok(result);
    }
    
    [HttpGet("by-id/{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id)
    {
        var input = new GetCompanyByIdInput(id);
        var result = await _getCompanyByIdUseCase.ExecuteAsync(input);
        return Ok(result);
    }
    
    [HttpGet("by-cnpj/{cnpj}")]
    public async Task<IActionResult> GetByCnpj(
        [FromRoute] string cnpj)
    {
        var input = new GetCompanyByCnpjInput(cnpj);
        var result = await _getCompanyByCnpjUseCase.ExecuteAsync(input);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ListCompaniesInput input)
    {
        var sanitizedInput = input.Normalize();
    
        var result = await _listCompaniesUseCase.ExecuteAsync(sanitizedInput);
        return Ok(result);
    }
}