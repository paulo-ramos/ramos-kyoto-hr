using FluentAssertions;
using ramos_kyoto_hr.Domain.Entities;
using ramos_kyoto_hr.Domain.ObjectValue;
using Xunit;

namespace ramos_kyoto_hr.Tests.Domain;

public class CompanyStateTests
{
    private readonly DateOnly _effectiveDate;
    private readonly RazaoSocial _razaoSocial;
    private readonly Cnpj _cnpj;
    private readonly bool  _isActive;

    public CompanyStateTests()
    {
        _effectiveDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _razaoSocial = RazaoSocial.Create("Empresa Teste Ltda");
        _cnpj = Cnpj.Create("AM.0WR.Y5D/0001-81");
        _isActive = true;
    }


    #region Enable Tests

    [Fact]
    public void Enable_WhenCompanyIsInactive_ShouldActivate_AndReturnTrue()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, _isActive);
        company.Disable(_effectiveDate);
        var effectiveAtBefore = company.EffectiveStartDate;

        // Act
        var hasChanged = company.Enable(_effectiveDate.AddDays(1));

        // Assert
        hasChanged.Should().Be(true, "porque o estado mudou de inativo para ativo");
        company.IsActive.Should().BeTrue("porque a empresa foi ativada");
        company.IsStatusActive().Should().BeTrue();
        company.EffectiveStartDate.Should().BeAfter(effectiveAtBefore, "porque houve mudança de estado");
    }

    [Fact]
    public void Enable_WhenCompanyIsAlreadyActive_ShouldNotChange_AndReturnFalse()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, _isActive);
        var effectiveStartDateBefore = company.EffectiveStartDate;

        // Act
        var hasChanged = company.Enable(_effectiveDate.AddDays(1));

        // Assert
        hasChanged.Should().Be(false, "porque a empresa já estava ativa");
        company.IsActive.Should().BeTrue("porque continua ativa");
        company.EffectiveStartDate.Should().Be(effectiveStartDateBefore, "porque a data não deve ser alterada se já está ativo");
    }

    [Fact]
    public void Enable_CalledMultipleTimes_ShouldBeIdempotent()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, _isActive);
        company.Disable(_effectiveDate);

        // Act
        var firstCall = company.Enable(_effectiveDate.AddDays(1));
        var effectiveAtAfterFirstCall = company.EffectiveStartDate;
        
        var secondCall = company.Enable(_effectiveDate.AddDays(2));
        var thirdCall = company.Enable(_effectiveDate.AddDays(3));

        // Assert
        firstCall.Should().Be(true, "porque a primeira chamada mudou o estado");
        secondCall.Should().Be(false, "porque já estava ativo");
        thirdCall.Should().Be(false, "porque já estava ativo");
        company.IsActive.Should().BeTrue();
        company.EffectiveStartDate.Should().Be(effectiveAtAfterFirstCall, "porque as chamadas subsequentes não mudaram nada");
    }

    #endregion

    #region Disable Tests

    [Fact]
    public void Disable_WhenCompanyIsActive_ShouldDeactivate_AndReturnTrue()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, _isActive);
        var effectiveAtBefore = company.EffectiveStartDate;

        // Act
        var hasChanged = company.Disable(_effectiveDate.AddDays(1));

        // Assert
        hasChanged.Should().Be(true, "porque o estado mudou de ativo para inativo");
        company.IsActive.Should().BeFalse("porque a empresa foi desativada");
        company.IsStatusDeactive().Should().BeTrue();
        company.EffectiveStartDate.Should().BeAfter(effectiveAtBefore, "porque houve mudança de estado");
    }

    [Fact]
    public void Disable_WhenCompanyIsAlreadyInactive_ShouldNotChange_AndReturnFalse()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, !_isActive);
        company.Disable(_effectiveDate);
        var effectiveAtBefore = company.EffectiveStartDate;
        var effectiveStartDateBefore = company.EffectiveStartDate;

        // Act
        var hasChanged = company.Disable(_effectiveDate.AddDays(1));

        // Assert
        hasChanged.Should().Be(false, "porque a empresa já estava inativa");
        company.IsActive.Should().BeFalse("porque continua inativa");
        company.EffectiveStartDate.Should().Be(effectiveAtBefore, "porque não houve mudança de estado (idempotente)");
        company.EffectiveStartDate.Should().Be(effectiveStartDateBefore, "porque a data não deve ser alterada se já está inativo");
    }

    #endregion

    #region State Transition Tests

    [Fact]
    public void StateTransition_FromActiveToInactiveToActive_ShouldWorkCorrectly()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, _isActive);

        // Act & Assert - Estado inicial: ATIVO
        company.IsActive.Should().BeTrue("porque é criada ativa por padrão");

        // Transição 1: ATIVO → INATIVO
        var disableResult = company.Disable(_effectiveDate.AddDays(1));
        disableResult.Should().Be(true, "porque mudou de estado");
        company.IsActive.Should().BeFalse();

        // Transição 2: INATIVO → ATIVO
        var enableResult = company.Enable(_effectiveDate.AddDays(2));
        enableResult.Should().Be(true, "porque mudou de estado");
        company.IsActive.Should().BeTrue();
    }

    [Fact]
    public void EffectiveStartDate_ShouldOnlyChangeWhenStateChanges()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, _isActive);
        var initialDate = _effectiveDate;

        // Act - Primeira desativação (muda)
        var date1 = _effectiveDate.AddDays(10);
        company.Disable(date1);
        company.EffectiveStartDate.Should().Be(date1);

        // Act - Segunda desativação (não muda)
        var date2 = _effectiveDate.AddDays(20);
        company.Disable(date2);
        company.EffectiveStartDate.Should().Be(date1, "porque a segunda chamada foi idempotente");

        // Act - Ativação (muda)
        var date3 = _effectiveDate.AddDays(30);
        company.Enable(date3);
        company.EffectiveStartDate.Should().Be(date3);

        // Act - Segunda ativação (não muda)
        var date4 = _effectiveDate.AddDays(40);
        company.Enable(date4);
        company.EffectiveStartDate.Should().Be(date3, "porque a segunda chamada foi idempotente");
    }

    #endregion

    #region Integration with UpdatedAt

    [Fact]
    public void UpdatedAt_ShouldOnlyChangeWhenStateActuallyChanges()
    {
        // Arrange
        var company = new Company(_effectiveDate, _razaoSocial, _cnpj, _isActive);
        var initialUpdatedAt = company.EffectiveStartDate;

        // Act - Tentativa de Enable quando já está ativo
        Thread.Sleep(10); // Garantir que o tempo passe
        company.Enable(_effectiveDate);
        
        // Assert - UpdatedAt não deve mudar
        company.EffectiveStartDate.Should().Be(initialUpdatedAt, "porque não houve mudança real de estado");

        // Act - Disable (muda de verdade)
        Thread.Sleep(10);
        company.Disable(_effectiveDate);
        
        // Assert - UpdatedAt deve mudar
        company.EffectiveStartDate.Should().BeAfter(initialUpdatedAt, "porque houve mudança real de estado");
    }

    #endregion
}

