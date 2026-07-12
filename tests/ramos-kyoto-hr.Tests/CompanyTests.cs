using System.Text;
using ramos_kyoto_hr.Domain.Entities;
using ramos_kyoto_hr.Domain.ObjectValue;
using Xunit;

namespace ramos_kyoto_hr.Tests;

public class CompanyTests
{
    private const string CnpjValido = "11.222.333/0001-81"; // CNPJ válido para testes
    
    #region Testes de Criação

    [Fact]
    public void DeveCriarCompanyComDadosValidos()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);

        // Act
        var company = new Company(razaoSocial, cnpj);

        // Assert
        Assert.NotNull(company);
        Assert.NotEqual(Guid.Empty, company.Id);
        Assert.Equal(razaoSocial, company.RazaoSocial);
        Assert.Equal(cnpj, company.Cnpj);
        Assert.True(company.IsActive);
        Assert.NotEqual(default(DateTime), company.CreatedAt);
        Assert.Null(company.UpdatedAt);
    }

    [Fact]
    public void DeveCriarCompanyComIdUnicoParaCadaInstancia()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);

        // Act
        var company1 = new Company(razaoSocial, cnpj);
        var company2 = new Company(razaoSocial, cnpj);

        // Assert
        Assert.NotEqual(company1.Id, company2.Id);
    }

    [Fact]
    public void NaoDeveCriarCompanyComRazaoSocialNula()
    {
        // Arrange
        var cnpj = Cnpj.Create(CnpjValido);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Company(null!, cnpj));
        Assert.Equal("razaoSocial", exception.ParamName);
    }

    [Fact]
    public void NaoDeveCriarCompanyComCnpjNulo()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new Company(razaoSocial, null!));
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
        var company = new Company(razaoSocial, cnpj);
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
        var company = new Company(razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");

        // Act
        company.UpdateRazaoSocial(novaRazaoSocial);

        // Assert
        Assert.Equal(novaRazaoSocial, company.RazaoSocial);
        Assert.NotEqual(razaoSocialInicial, company.RazaoSocial);
    }

    [Fact]
    public void DeveAtualizarUpdatedAtAoAlterarRazaoSocial()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        Assert.Null(company.UpdatedAt);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");
        var antes = DateTime.UtcNow;

        // Act
        company.UpdateRazaoSocial(novaRazaoSocial);
        var depois = DateTime.UtcNow;

        // Assert
        Assert.NotNull(company.UpdatedAt);
        Assert.True(company.UpdatedAt >= antes && company.UpdatedAt <= depois);
    }

    [Fact]
    public void NaoDeveAlterarRazaoSocialComValorNulo()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => company.UpdateRazaoSocial(null!));
        Assert.Equal("novaRazaoSocial", exception.ParamName);
        
        // Verifica que nada foi alterado
        Assert.Equal(razaoSocialInicial, company.RazaoSocial);
        Assert.Null(company.UpdatedAt);
    }

    [Fact]
    public void DevePermitirMultiplasAlteracoesDeRazaoSocial()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var razaoSocial1 = RazaoSocial.Create("Empresa Alterada 1 LTDA");
        var razaoSocial2 = RazaoSocial.Create("Empresa Alterada 2 LTDA");

        // Act
        company.UpdateRazaoSocial(razaoSocial1);
        var primeiraAtualizacao = company.UpdatedAt;
        
        Thread.Sleep(10); // Pequeno delay para garantir timestamps diferentes
        
        company.UpdateRazaoSocial(razaoSocial2);
        var segundaAtualizacao = company.UpdatedAt;

        // Assert
        Assert.Equal(razaoSocial2, company.RazaoSocial);
        Assert.NotNull(primeiraAtualizacao);
        Assert.NotNull(segundaAtualizacao);
        Assert.True(segundaAtualizacao > primeiraAtualizacao);
    }

    [Fact]
    public void DeveAlterarRazaoSocialParaMesmoValor()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        
        var mesmaRazaoSocial = RazaoSocial.Create("Empresa Teste LTDA");

        // Act
        company.UpdateRazaoSocial(mesmaRazaoSocial);

        // Assert - Mesmo sendo o mesmo valor, deve atualizar o UpdatedAt
        Assert.Equal(mesmaRazaoSocial, company.RazaoSocial);
        Assert.NotNull(company.UpdatedAt);
    }

    [Fact]
    public void DeveAlterarRazaoSocialEManterCnpjInalterado()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");

        // Act
        company.UpdateRazaoSocial(novaRazaoSocial);

        // Assert - CNPJ não deve ser alterado
        Assert.Equal(cnpj, company.Cnpj);
        Assert.Equal(novaRazaoSocial, company.RazaoSocial);
    }

    [Fact]
    public void DeveAlterarRazaoSocialEManterIdECreatedAtInalterados()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var idOriginal = company.Id;
        var createdAtOriginal = company.CreatedAt;
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");

        // Act
        company.UpdateRazaoSocial(novaRazaoSocial);

        // Assert - Id e CreatedAt não devem ser alterados
        Assert.Equal(idOriginal, company.Id);
        Assert.Equal(createdAtOriginal, company.CreatedAt);
        Assert.NotEqual(razaoSocialInicial, company.RazaoSocial);
    }

    #endregion

    #region Testes de Enable e Disable

    [Fact]
    public void DeveIniciarComIsActiveTrue()
    {
        // Arrange & Act
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);

        // Assert
        Assert.True(company.IsActive);
    }

    [Fact]
    public void DeveDesativarEmpresaComSucesso()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        Assert.True(company.IsActive);

        // Act
        company.Disable();

        // Assert
        Assert.False(company.IsActive);
    }

    [Fact]
    public void DeveAtivarEmpresaComSucesso()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        company.Disable();
        Assert.False(company.IsActive);

        // Act
        company.Enable();

        // Assert
        Assert.True(company.IsActive);
    }

    [Fact]
    public void DeveAtualizarUpdatedAtAoDesativar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        Assert.Null(company.UpdatedAt);

        var antes = DateTime.UtcNow;

        // Act
        company.Disable();
        var depois = DateTime.UtcNow;

        // Assert
        Assert.NotNull(company.UpdatedAt);
        Assert.True(company.UpdatedAt >= antes && company.UpdatedAt <= depois);
    }

    [Fact]
    public void DeveAtualizarUpdatedAtAoAtivar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        company.Disable();
        
        var updatedAtAnterior = company.UpdatedAt;
        Thread.Sleep(10);
        
        var antes = DateTime.UtcNow;

        // Act
        company.Enable();
        var depois = DateTime.UtcNow;

        // Assert
        Assert.NotNull(company.UpdatedAt);
        Assert.True(company.UpdatedAt >= antes && company.UpdatedAt <= depois);
        Assert.True(company.UpdatedAt > updatedAtAnterior);
    }

    [Fact]
    public void DevePermitirMultiplasAtivacoes()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);

        // Act - Ativar quando já está ativo
        company.Enable();

        // Assert - Deve continuar ativo e atualizar UpdatedAt
        Assert.True(company.IsActive);
        Assert.NotNull(company.UpdatedAt);
    }

    [Fact]
    public void DevePermitirMultiplasDesativacoes()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        company.Disable();

        // Act - Desativar quando já está inativo
        company.Disable();

        // Assert - Deve continuar inativo e atualizar UpdatedAt
        Assert.False(company.IsActive);
        Assert.NotNull(company.UpdatedAt);
    }

    [Fact]
    public void DeveAlternarEntreAtivoEInativo()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);

        // Act & Assert - Ciclo de ativação/desativação
        Assert.True(company.IsActive);

        company.Disable();
        Assert.False(company.IsActive);

        company.Enable();
        Assert.True(company.IsActive);

        company.Disable();
        Assert.False(company.IsActive);
    }

    [Fact]
    public void DeveManterRazaoSocialECnpjAoDesativar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);

        // Act
        company.Disable();

        // Assert - Razão Social e CNPJ não devem ser alterados
        Assert.Equal(razaoSocial, company.RazaoSocial);
        Assert.Equal(cnpj, company.Cnpj);
        Assert.False(company.IsActive);
    }

    [Fact]
    public void DeveManterRazaoSocialECnpjAoAtivar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        company.Disable();

        // Act
        company.Enable();

        // Assert - Razão Social e CNPJ não devem ser alterados
        Assert.Equal(razaoSocial, company.RazaoSocial);
        Assert.Equal(cnpj, company.Cnpj);
        Assert.True(company.IsActive);
    }

    [Fact]
    public void DeveChamarHooksAoDesativar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.Disable();
            
            Console.Out.Flush();
            Console.SetOut(originalConsoleOut);
            writer.Close();
            
            var consoleOutput = output.ToString();

            // Assert - Hooks devem ser executados
            Assert.Contains("[BEFORE]", consoleOutput);
            Assert.Contains("[AFTER]", consoleOutput);
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    [Fact]
    public void DeveChamarHooksAoAtivar()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocial, cnpj);
        company.Disable();
        
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.Enable();
            
            Console.Out.Flush();
            Console.SetOut(originalConsoleOut);
            writer.Close();
            
            var consoleOutput = output.ToString();

            // Assert - Hooks devem ser executados
            Assert.Contains("[BEFORE]", consoleOutput);
            Assert.Contains("[AFTER]", consoleOutput);
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    #endregion

    #region Testes de Logs no Console

    [Fact]
    public void DiagnosticoCapturaDeLogs()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");
        
        // Captura a saída do console
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.UpdateRazaoSocial(novaRazaoSocial);
            
            Console.Out.Flush();
            writer.Flush();
            
            var consoleOutput = output.ToString();
            
            // Restaura antes de imprimir o diagnóstico
            Console.SetOut(originalConsoleOut);
            
            // Diagnóstico - imprime o conteúdo capturado
            System.Console.WriteLine("=== CONTEÚDO CAPTURADO ===");
            System.Console.WriteLine($"Tamanho: {consoleOutput.Length} caracteres");
            System.Console.WriteLine(consoleOutput);
            System.Console.WriteLine("=== FIM DO CONTEÚDO ===");
            
            // Assert - apenas verifica que algo foi capturado
            Assert.NotEmpty(consoleOutput);
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    [Fact]
    public void DeveExibirLogsNoConsoleAntesEDepoisDaAlteracao()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");
        
        // Captura a saída do console
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.UpdateRazaoSocial(novaRazaoSocial);
            
            // Força o flush e fecha o writer para garantir que todo o conteúdo seja capturado
            Console.Out.Flush();
            Console.SetOut(originalConsoleOut);
            writer.Close();
            
            var consoleOutput = output.ToString();

            // Assert
            Assert.Contains($"[BEFORE] Empresa {company.Id} está sendo atualizada...", consoleOutput);
            Assert.Contains($"[BEFORE] Razão Social atual: {razaoSocialInicial.Valor}", consoleOutput);
            Assert.Contains("[BEFORE] UpdatedAt atual: null", consoleOutput);
            Assert.Contains("[AFTER] UpdatedAt agora é:", consoleOutput);
            Assert.Contains($"Razão Social alterada para: {novaRazaoSocial.Valor}", consoleOutput);
            Assert.Contains(new string('-', 60), consoleOutput);
        }
        finally
        {
            // Restaura o console original
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    [Fact]
    public void DeveExibirLogComUpdatedAtNullNaPrimeiraAlteracao()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");
        
        // Captura a saída do console
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.UpdateRazaoSocial(novaRazaoSocial);
            
            Console.Out.Flush();
            Console.SetOut(originalConsoleOut);
            writer.Close();
            
            var consoleOutput = output.ToString();

            // Assert
            Assert.Contains("[BEFORE] UpdatedAt atual: null", consoleOutput);
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    [Fact]
    public void DeveExibirLogComUpdatedAtPreenchidoNaSegundaAlteracao()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var primeiraRazaoSocial = RazaoSocial.Create("Empresa Primeira Alteração LTDA");
        var segundaRazaoSocial = RazaoSocial.Create("Empresa Segunda Alteração LTDA");
        
        // Primeira alteração (sem captura de log)
        company.UpdateRazaoSocial(primeiraRazaoSocial);
        
        // Captura a saída do console na segunda alteração
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.UpdateRazaoSocial(segundaRazaoSocial);
            
            Console.Out.Flush();
            Console.SetOut(originalConsoleOut);
            writer.Close();
            
            var consoleOutput = output.ToString();

            // Assert
            // Na segunda alteração, o UpdatedAt não deve estar como "null"
            Assert.DoesNotContain("[BEFORE] UpdatedAt atual: null", consoleOutput);
            // Deve conter uma data válida
            Assert.Matches(@"\[BEFORE\] UpdatedAt atual: \d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}", consoleOutput);
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    [Fact]
    public void DeveExibirRegistroDeAuditoriaNoFormatoCorreto()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Nova LTDA");
        
        // Captura a saída do console
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.UpdateRazaoSocial(novaRazaoSocial);
            
            Console.Out.Flush();
            Console.SetOut(originalConsoleOut);
            writer.Close();
            
            var consoleOutput = output.ToString();

            // Assert
            // Verifica o formato do registro de auditoria: yyyy-MM-dd HH:mm:ss - Razão Social alterada para: ...
            Assert.Matches(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} - Razão Social alterada para: Empresa Nova LTDA", consoleOutput);
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    [Fact]
    public void DeveExibirTodosOsLogsNaOrdemCorreta()
    {
        // Arrange
        var razaoSocialInicial = RazaoSocial.Create("Empresa Inicial LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company = new Company(razaoSocialInicial, cnpj);
        
        var novaRazaoSocial = RazaoSocial.Create("Empresa Alterada LTDA");
        
        // Captura a saída do console
        var output = new StringBuilder();
        var originalConsoleOut = Console.Out;
        var writer = new StringWriter(output);
        Console.SetOut(writer);

        try
        {
            // Act
            company.UpdateRazaoSocial(novaRazaoSocial);
            
            Console.Out.Flush();
            Console.SetOut(originalConsoleOut);
            writer.Close();
            
            var consoleOutput = output.ToString();
            var lines = consoleOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Assert - Verifica a ordem dos logs
            var beforeIndex = Array.FindIndex(lines, l => l.Contains("[BEFORE]"));
            var afterIndex = Array.FindIndex(lines, l => l.Contains("[AFTER]"));
            var separatorIndex = Array.FindIndex(lines, l => l.Contains(new string('-', 60)));
            
            Assert.True(beforeIndex >= 0, "Deve conter logs [BEFORE]");
            Assert.True(afterIndex >= 0, "Deve conter logs [AFTER]");
            Assert.True(separatorIndex >= 0, "Deve conter separador");
            Assert.True(beforeIndex < afterIndex, "Logs [BEFORE] devem vir antes de [AFTER]");
            Assert.True(afterIndex < separatorIndex, "Logs [AFTER] devem vir antes do separador");
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            writer.Dispose();
        }
    }

    #endregion

    #region Testes de Igualdade

    [Fact]
    public void DeveTratarCompaniesComMesmoIdComoIguais()
    {
        // Arrange
        var razaoSocial = RazaoSocial.Create("Empresa Teste LTDA");
        var cnpj = Cnpj.Create(CnpjValido);
        var company1 = new Company(razaoSocial, cnpj);
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
        var company1 = new Company(razaoSocial, cnpj);
        var company2 = new Company(razaoSocial, cnpj);

        // Act & Assert
        Assert.False(company1.Equals(company2));
        Assert.NotEqual(company1, company2);
    }

    #endregion
}

