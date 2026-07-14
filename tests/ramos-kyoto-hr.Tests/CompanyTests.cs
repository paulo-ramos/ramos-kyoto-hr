using System.Text;
using ramos_kyoto_hr.Domain.Entities;
using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Domain.Utils;
using Xunit;

namespace ramos_kyoto_hr.Tests;

public class CompanyTests
{
    private const string CnpjValido = "11.222.333/0001-81";
    public DateOnly effectiveStartDate =  DateOnly.FromDateTime(DateTime.UtcNow);
    
    #region Testes de Criação

    [Fact]
    public void DeveCriarCompanyComDadosValidos()
    {
        // Arrange
        var effectiveStartDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var id = GuidGenerator.GuidOrganizationalStructure(cnpj, effectiveStartDate);

        // Act
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);

        // Assert
        Assert.NotNull(company);
        Assert.NotEqual(Guid.Empty, company.Id);
        Assert.Equal(razaoSocial, company.RazaoSocial);
        Assert.Equal(cnpj, company.Cnpj);
        Assert.True(company.IsActive);
        Assert.NotEqual(default, company.CreatedAt);
        Assert.Equal(company.Id, id);
    }

    [Fact]
    public void DeveCriarCompanyComIdUnicoParaCadaInstancia()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);

        // Act
        var company1 = new Company(effectiveStartDate, razaoSocial, cnpj);
        var company2 = new Company(effectiveStartDate.AddDays(1), razaoSocial, cnpj);

        // Assert
        Assert.NotEqual(company1.Id, company2.Id);
    }

    [Fact]
    public void NaoDeveCriarCompanyComRazaoSocialNula()
    {
        // Arrange
        var cnpj = Cnpj.Create(CnpjValido);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Company(effectiveStartDate, null!, cnpj));
        Assert.Equal("razaoSocial", exception.ParamName);
    }

    [Fact]
    public void NaoDeveCriarCompanyComCnpjNulo()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Company(effectiveStartDate, razaoSocial, null!));
        Assert.Equal("cnpj", exception.ParamName);
    }

    [Fact]
    public void DeveCriarCompanyComCreatedAtPreenchido()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var antes = DateTime.UtcNow;

        // Act
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);
        var depois = DateTime.UtcNow;

        // Assert
        Assert.True(company.CreatedAt >= antes && company.CreatedAt <= depois);
    }

    #endregion

    #region Testes de Alteração

    [Fact]
    public void DeveAlterarRazaoSocialComSucesso()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");

        // Act
        var companyAlterada = company.UpdateRazaoSocial(effectiveStartDate.AddDays(1), novaRazaoSocial);

        // Assert
        Assert.Equal(novaRazaoSocial, companyAlterada?.RazaoSocial);
        Assert.NotEqual(razaoSocialInicial, companyAlterada?.RazaoSocial);
        Assert.Equal(razaoSocialInicial, company.RazaoSocial);
    }

    [Fact]
    public void DeveAtualizarUpdatedAtAoAlterarRazaoSocial()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");
        var antes = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        company.UpdateRazaoSocial(effectiveStartDate.AddDays(1), novaRazaoSocial);
        var depois = DateOnly.FromDateTime(DateTime.UtcNow);

        // Assert
        Assert.True(company.EffectiveStartDate >= antes && company.EffectiveStartDate <= depois);
    }

    [Fact]
    public void NaoDeveAlterarRazaoSocialComValorNulo()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocialInicial, cnpj);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => company.UpdateRazaoSocial(effectiveStartDate,null!));
        Assert.Equal("novaRazaoSocial", exception.ParamName);
        
        // Verifica que nada foi alterado
        Assert.Equal(razaoSocialInicial, company.RazaoSocial);
    }

    [Fact]
    public void DevePermitirMultiplasAlteracoesDeRazaoSocial()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocialInicial, cnpj);
        
        var razaoSocial1 = RazaoSocial.Create("Empresa Alterada 1 LTDA");
        var razaoSocial2 = RazaoSocial.Create("Empresa Alterada 2 LTDA");

        // Act
        var company1 = company.UpdateRazaoSocial(effectiveStartDate.AddDays(1), razaoSocial1);
        var primeiraAtualizacao = company1?.CreatedAt;
        
        Thread.Sleep(10); // Pequeno delay para garantir timestamps diferentes
        
        var company2 = company1?.UpdateRazaoSocial(effectiveStartDate.AddDays(2), razaoSocial2);
        var segundaAtualizacao = company2?.CreatedAt;

        // Assert
        Assert.Equal(razaoSocial2, company2?.RazaoSocial);
        Assert.True(segundaAtualizacao > primeiraAtualizacao);
    }

    [Fact]
    public void DeveAlterarRazaoSocialEManterCnpjInalterado()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");

        // Act
        var companyAlterada = company.UpdateRazaoSocial(effectiveStartDate.AddDays(1), novaRazaoSocial);

        // Assert - CNPJ não deve ser alterado
        Assert.Equal(cnpj, company.Cnpj);
        Assert.Equal(cnpj, companyAlterada?.Cnpj);
        Assert.NotEqual(novaRazaoSocial, company.RazaoSocial);
        Assert.Equal(novaRazaoSocial, companyAlterada?.RazaoSocial);
    }

    [Fact]
    public void DeveAlterarRazaoSocialEManterIdECreatedAtInalterados()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocialInicial, cnpj);
        
        var idOriginal = company.Id;
        var createdAtOriginal = company.CreatedAt;
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");

        // Act
        var companyAlterada = company.UpdateRazaoSocial(effectiveStartDate.AddDays(1), novaRazaoSocial);

        // Assert - Id não deve ser alterado
        Assert.Equal(idOriginal, company.Id);
        Assert.Equal(createdAtOriginal, company.CreatedAt);
        Assert.NotEqual(razaoSocialInicial, companyAlterada?.RazaoSocial);
    }

    #endregion

    #region Testes de Enable e Disable

    [Fact]
    public void DeveIniciarComIsActiveTrue()
    {
        // Arrange & Act
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);

        // Assert
        Assert.True(company.IsActive);
    }

    [Fact]
    public void DeveDesativarEmpresaComSucesso()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);
        Assert.True(company.IsActive);

        // Act
        var company1 = company.Disable(effectiveStartDate.AddDays(1));

        // Assert
        Assert.True(company.IsActive);
        Assert.False(company1?.IsActive);
    }

    [Fact]
    public void DeveAtivarEmpresaComSucesso()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);
        var companyAlterada1 = company.Disable(effectiveStartDate.AddDays(1));
        Assert.False(companyAlterada1?.IsActive);

        // Act
        var companyAlterada2 = companyAlterada1?.Enable(effectiveStartDate.AddDays(2));

        // Assert
        Assert.True(companyAlterada2?.IsActive);
    }

    [Fact]
    public void DeveAtualizarUpdatedAtAoDesativar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);

        var antes = DateTime.UtcNow;

        // Act
        var company1 = company.Disable(effectiveStartDate.AddDays(1));
        var depois = DateTime.UtcNow;

        // Assert
        Assert.True(company1?.CreatedAt >= antes && company1?.CreatedAt <= depois);
    }

    [Fact]
    public void DeveAlternarEntreAtivoEInativo()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);

        Assert.True(company.IsActive);

        var companyAlterada1 = company.Disable(effectiveStartDate.AddDays(1));
        Assert.False(companyAlterada1?.IsActive);

        var companyAlterada2 = companyAlterada1?.Enable(effectiveStartDate.AddDays(2));
        Assert.True(companyAlterada2?.IsActive);

        var companyAlterada3 = companyAlterada2?.Disable(effectiveStartDate.AddDays(3));
        Assert.False(companyAlterada3?.IsActive);
    }

    [Fact]
    public void DeveManterRazaoSocialECnpjAoDesativar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);

        // Act
        var companyAlterada = company.Disable(effectiveStartDate.AddDays(1));

        // Assert - Razão Social e CNPJ não devem ser alterados
        Assert.Equal(razaoSocial, company.RazaoSocial);
        Assert.Equal(cnpj, company.Cnpj);
        Assert.True(company.IsActive);
        Assert.False(companyAlterada?.IsActive);
    }

    [Fact]
    public void DeveManterRazaoSocialECnpjAoAtivar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(effectiveStartDate, razaoSocial, cnpj);
        var company1 = company.Disable(effectiveStartDate.AddDays(1));

        // Act
        var company2 = company1.Enable(effectiveStartDate.AddDays(2));

        // Assert - Razão Social e CNPJ não devem ser alterados
        Assert.Equal(razaoSocial, company2?.RazaoSocial);
        Assert.Equal(cnpj, company2?.Cnpj);
        Assert.True(company2?.IsActive);
    }

    #endregion

    #region Testes de Igualdade

    [Fact]
    public void DeveTratarCompaniesComMesmoIdComoIguais()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company1 = new Company(effectiveStartDate, razaoSocial, cnpj);
        var company2 = company1;

        // Act & Assert
        Assert.True(company1.Equals(company2));
        Assert.Equal(company1, company2);
    }

    [Fact]
    public void DeveTratarCompaniesComIdsDistintosComoDiferentes()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company1 = new Company(effectiveStartDate, razaoSocial, cnpj);
        var company2 = new Company(effectiveStartDate.AddDays(1), razaoSocial, cnpj);

        // Act & Assert
        Assert.False(company1.Equals(company2));
        Assert.NotEqual(company1, company2);
    }

    #endregion
}

