using ramos_kyoto_hr.Domain.ObjectValue;
using Xunit;

namespace ramos_kyoto_hr.Tests.ObjectValue;

public class RazaoSocialTests
{
    #region Testes de Criação com Sucesso

    [Fact]
    public void DeveCriarRazaoSocialComValorValido()
    {
        // Arrange
        var valor = "Empresa Teste LTDA";

        // Act
        var razaoSocial = RazaoSocial.Create(valor);

        // Assert
        Assert.NotNull(razaoSocial);
        Assert.Equal(valor, razaoSocial.Valor);
    }

    [Fact]
    public void DeveCriarRazaoSocialComTamanhoMinimo()
    {
        // Arrange
        var valor = "ABC"; // 3 caracteres (tamanho mínimo)

        // Act
        var razaoSocial = RazaoSocial.Create(valor);

        // Assert
        Assert.NotNull(razaoSocial);
        Assert.Equal(valor, razaoSocial.Valor);
    }

    [Fact]
    public void DeveCriarRazaoSocialComTamanhoMaximo()
    {
        // Arrange
        var valor = new string('A', 200); // 200 caracteres (tamanho máximo)

        // Act
        var razaoSocial = RazaoSocial.Create(valor);

        // Assert
        Assert.NotNull(razaoSocial);
        Assert.Equal(200, razaoSocial.Valor.Length);
    }

    [Fact]
    public void DeveRemoverEspacosEmBrancoNasExtremidesAoCriar()
    {
        // Arrange
        var valorComEspacos = "   Empresa Teste LTDA   ";
        var valorEsperado = "Empresa Teste LTDA";

        // Act
        var razaoSocial = RazaoSocial.Create(valorComEspacos);

        // Assert
        Assert.Equal(valorEsperado, razaoSocial.Valor);
    }

    [Theory]
    [InlineData("Empresa de Tecnologia LTDA")]
    [InlineData("ABC Indústria e Comércio S.A.")]
    [InlineData("XYZ Serviços Empresariais EIRELI")]
    [InlineData("Consultoria & Assessoria Ltda")]
    public void DeveCriarRazaoSocialComDiferentesFormatos(string valor)
    {
        // Act
        var razaoSocial = RazaoSocial.Create(valor);

        // Assert
        Assert.NotNull(razaoSocial);
        Assert.Equal(valor, razaoSocial.Valor);
    }

    #endregion

    #region Testes de Validação - Casos Inválidos

    [Fact]
    public void NaoDeveCriarRazaoSocialComValorNulo()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RazaoSocial.Create(null!));
        Assert.Contains("não pode ser nula ou vazia", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarRazaoSocialComValorVazio()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RazaoSocial.Create(""));
        Assert.Contains("não pode ser nula ou vazia", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarRazaoSocialComApenasEspacosEmBranco()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RazaoSocial.Create("   "));
        Assert.Contains("não pode ser nula ou vazia", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarRazaoSocialComMenosDe3Caracteres()
    {
        // Arrange
        var valorCurto = "AB";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RazaoSocial.Create(valorCurto));
        Assert.Contains("pelo menos 3 caracteres", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarRazaoSocialComMaisDe200Caracteres()
    {
        // Arrange
        var valorLongo = new string('A', 201);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RazaoSocial.Create(valorLongo));
        Assert.Contains("não pode exceder 200 caracteres", exception.Message);
    }

    [Fact]
    public void NaoDeveCriarRazaoSocialComMenosDe3CaracteresAposRemoverEspacos()
    {
        // Arrange
        var valorComEspacos = "  AB  "; // Apenas 2 caracteres sem os espaços

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => RazaoSocial.Create(valorComEspacos));
        Assert.Contains("pelo menos 3 caracteres", exception.Message);
    }

    #endregion

    #region Testes de Igualdade

    [Fact]
    public void DeveTratarRazoesSociaisComMesmoValorComoIguais()
    {
        // Arrange
        var razaoSocial1 = RazaoSocial.Create("Empresa Teste LTDA");
        var razaoSocial2 = RazaoSocial.Create("Empresa Teste LTDA");

        // Act & Assert
        Assert.True(razaoSocial1.Equals(razaoSocial2));
        Assert.True(razaoSocial1 == razaoSocial2);
        Assert.False(razaoSocial1 != razaoSocial2);
        Assert.Equal(razaoSocial1.GetHashCode(), razaoSocial2.GetHashCode());
    }

    [Fact]
    public void DeveTratarRazoesSociaisComValoresDiferentesComoDiferentes()
    {
        // Arrange
        var razaoSocial1 = RazaoSocial.Create("Empresa A LTDA");
        var razaoSocial2 = RazaoSocial.Create("Empresa B LTDA");

        // Act & Assert
        Assert.False(razaoSocial1.Equals(razaoSocial2));
        Assert.False(razaoSocial1 == razaoSocial2);
        Assert.True(razaoSocial1 != razaoSocial2);
        Assert.NotEqual(razaoSocial1.GetHashCode(), razaoSocial2.GetHashCode());
    }

    [Fact]
    public void DeveTratarRazaoSocialNulaComoNaoIgual()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        RazaoSocial? razaoSocialNula = null;

        // Act & Assert
        Assert.False(razaoSocial.Equals(razaoSocialNula));
        Assert.False(razaoSocial == razaoSocialNula);
        Assert.True(razaoSocial != razaoSocialNula);
    }

    [Fact]
    public void DeveTratarDuasRazoesSociaisNulasComoIguais()
    {
        // Arrange
        RazaoSocial? razaoSocial1 = null;
        RazaoSocial? razaoSocial2 = null;

        // Act & Assert
        Assert.True(razaoSocial1 == razaoSocial2);
        Assert.False(razaoSocial1 != razaoSocial2);
    }

    [Fact]
    public void DeveCompararRazaoSocialComObjetoNulo()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        object? obj = null;

        // Act & Assert
        Assert.False(razaoSocial.Equals(obj));
    }

    [Fact]
    public void DeveSerCaseSensitiveNaComparacao()
    {
        // Arrange
        var razaoSocial1 = RazaoSocial.Create("Empresa Teste LTDA");
        var razaoSocial2 = RazaoSocial.Create("empresa teste ltda");

        // Act & Assert
        Assert.False(razaoSocial1.Equals(razaoSocial2));
        Assert.NotEqual(razaoSocial1, razaoSocial2);
    }

    #endregion

    #region Testes de ToString e Conversão Implícita

    [Fact]
    public void DeveConverterRazaoSocialParaString()
    {
        // Arrange
        var valor = "Empresa Teste LTDA";
        var razaoSocial = RazaoSocial.Create(valor);

        // Act
        var resultado = razaoSocial.ToString();

        // Assert
        Assert.Equal(valor, resultado);
    }

    [Fact]
    public void DevePermitirConversaoImplicitaParaString()
    {
        // Arrange
        var valor = "Empresa Teste LTDA";
        var razaoSocial = RazaoSocial.Create(valor);

        // Act
        string valorString = razaoSocial;

        // Assert
        Assert.Equal(valor, valorString);
    }

    #endregion

    #region Testes de Limites

    [Fact]
    public void DeveAceitarExatamente3Caracteres()
    {
        // Arrange & Act
        var razaoSocial = RazaoSocial.Create("ABC");

        // Assert
        Assert.Equal(3, razaoSocial.Valor.Length);
    }

    [Fact]
    public void DeveAceitarExatamente200Caracteres()
    {
        // Arrange
        var valor = new string('A', 200);

        // Act
        var razaoSocial = RazaoSocial.Create(valor);

        // Assert
        Assert.Equal(200, razaoSocial.Valor.Length);
    }

    [Fact]
    public void DeveRejeitarExatamente2Caracteres()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => RazaoSocial.Create("AB"));
    }

    [Fact]
    public void DeveRejeitarExatamente201Caracteres()
    {
        // Arrange
        var valor = new string('A', 201);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => RazaoSocial.Create(valor));
    }

    #endregion
}

