using ramos_kyoto_hr.Domain.ObjectValue;
using Xunit;

namespace ramos_kyoto_hr.Tests.ObjectValue;

public class CnpjTests
{
    private const string CnpjValidoFormatado = "11.222.333/0001-81";
    private const string CnpjValidoSemFormatacao = "11222333000181";
    
    #region Testes de Criação com Sucesso

    [Fact]
    public void DeveCriarCnpjComValorValido()
    {
        // Act
        var cnpj = Cnpj.Create(CnpjValidoFormatado);

        // Assert
        Assert.NotNull(cnpj);
        Assert.Equal(CnpjValidoSemFormatacao, cnpj.Valor);
    }

    [Fact]
    public void DeveCriarCnpjSemFormatacao()
    {
        // Act
        var cnpj = Cnpj.Create(CnpjValidoSemFormatacao);

        // Assert
        Assert.NotNull(cnpj);
        Assert.Equal(CnpjValidoSemFormatacao, cnpj.Valor);
    }

    [Fact]
    public void DeveRemoverCaracteresEspeciaisAoCriarCnpj()
    {
        // Arrange
        var cnpjComCaracteresEspeciais = "11.222.333/0001-81";

        // Act
        var cnpj = Cnpj.Create(cnpjComCaracteresEspeciais);

        // Assert
        Assert.Equal("11222333000181", cnpj.Valor);
        Assert.DoesNotContain(".", cnpj.Valor);
        Assert.DoesNotContain("/", cnpj.Valor);
        Assert.DoesNotContain("-", cnpj.Valor);
    }

    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11222333000181")]
    [InlineData("11-222-333-0001-81")]
    [InlineData("  11.222.333/0001-81  ")]
    public void DeveCriarCnpjComDiferentesFormatos(string cnpjFormatado)
    {
        // Act
        var cnpj = Cnpj.Create(cnpjFormatado);

        // Assert
        Assert.Equal(CnpjValidoSemFormatacao, cnpj.Valor);
    }

    #endregion

    #region Testes de Validação - Casos Inválidos

    [Fact]
    public void NaoDeveCriarCnpjComValorNulo()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Cnpj.Create(null!));
        Assert.Contains("não pode ser vazio ou nulo", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarCnpjComValorVazio()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Cnpj.Create(""));
        Assert.Contains("não pode ser vazio ou nulo", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarCnpjComEspacosEmBranco()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Cnpj.Create("   "));
        Assert.Contains("não pode ser vazio ou nulo", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarCnpjComTamanhoIncorreto()
    {
        // Arrange
        var cnpjCurto = "11222333000";
        var cnpjLongo = "112223330001811";

        // Act & Assert
        var exceptionCurto = Assert.Throws<ArgumentException>(() => Cnpj.Create(cnpjCurto));
        Assert.Contains("14 caracteres", exceptionCurto.Message);

        var exceptionLongo = Assert.Throws<ArgumentException>(() => Cnpj.Create(cnpjLongo));
        Assert.Contains("14 caracteres", exceptionLongo.Message);
    }

    [Theory]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("22222222222222")]
    public void NaoDeveCriarCnpjComTodosDigitosIguais(string cnpjInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Cnpj.Create(cnpjInvalido));
        Assert.Contains("inválido", exception.Message);
    }

    [Theory]
    [InlineData("11.222.333/0001-80")] // DV incorreto
    [InlineData("11.222.333/0001-99")] // DV incorreto
    [InlineData("12.345.678/0001-00")] // DV incorreto
    public void NaoDeveCriarCnpjComDigitoVerificadorInvalido(string cnpjInvalido)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Cnpj.Create(cnpjInvalido));
        Assert.Contains("inválido", exception.Message);
    }

    #endregion

    #region Testes de Igualdade

    [Fact]
    public void DeveTratarCnpjsComMesmoValorComoIguais()
    {
        // Arrange
        var cnpj1 = Cnpj.Create(CnpjValidoFormatado);
        var cnpj2 = Cnpj.Create(CnpjValidoSemFormatacao);

        // Act & Assert
        Assert.True(cnpj1.Equals(cnpj2));
        Assert.True(cnpj1 == cnpj2);
        Assert.False(cnpj1 != cnpj2);
        Assert.Equal(cnpj1.GetHashCode(), cnpj2.GetHashCode());
    }

    [Fact]
    public void DeveTratarCnpjsComValoresDiferentesComoDiferentes()
    {
        // Arrange
        var cnpj1 = Cnpj.Create("11.222.333/0001-81");
        var cnpj2 = Cnpj.Create("11.222.333/0002-62");

        // Act & Assert
        Assert.False(cnpj1.Equals(cnpj2));
        Assert.False(cnpj1 == cnpj2);
        Assert.True(cnpj1 != cnpj2);
        Assert.NotEqual(cnpj1.GetHashCode(), cnpj2.GetHashCode());
    }

    [Fact]
    public void DeveTratarCnpjNuloComoNaoIgual()
    {
        // Arrange
        var cnpj = Cnpj.Create(CnpjValidoFormatado);
        Cnpj? cnpjNulo = null;

        // Act & Assert
        Assert.False(cnpj.Equals(cnpjNulo));
        Assert.False(cnpj == cnpjNulo);
        Assert.True(cnpj != cnpjNulo);
    }

    [Fact]
    public void DeveTratarDoisCnpjsNulosComoIguais()
    {
        // Arrange
        Cnpj? cnpj1 = null;
        Cnpj? cnpj2 = null;

        // Act & Assert
        Assert.True(cnpj1 == cnpj2);
        Assert.False(cnpj1 != cnpj2);
    }

    [Fact]
    public void DeveCompararCnpjComObjetoNaoNulo()
    {
        // Arrange
        var cnpj = Cnpj.Create(CnpjValidoFormatado);
        object? obj = null;

        // Act & Assert
        Assert.False(cnpj.Equals(obj));
    }

    #endregion

    #region Testes de ToString e Conversão Implícita

    [Fact]
    public void DeveConverterCnpjParaString()
    {
        // Arrange
        var cnpj = Cnpj.Create(CnpjValidoFormatado);

        // Act
        var resultado = cnpj.ToString();

        // Assert
        Assert.Equal(CnpjValidoSemFormatacao, resultado);
    }

    [Fact]
    public void DevePermitirConversaoImplicitaParaString()
    {
        // Arrange
        var cnpj = Cnpj.Create(CnpjValidoFormatado);

        // Act
        string valorString = cnpj;

        // Assert
        Assert.Equal(CnpjValidoSemFormatacao, valorString);
    }

    #endregion

    #region Testes de CNPJs Válidos Reais

    [Theory]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11.222.333/0002-62")]
    [InlineData("00.000.000/0001-91")]
    public void DeveAceitarCnpjsValidosComDigitoVerificadorCorreto(string cnpjValido)
    {
        // Act
        var cnpj = Cnpj.Create(cnpjValido);

        // Assert
        Assert.NotNull(cnpj);
        Assert.Equal(14, cnpj.Valor.Length);
    }

    #endregion
}

