using System.Reflection;
using ramos_kyoto_hr.Domain.Base;

namespace ramos_kyoto_hr.Tests.Base;

/// <summary>
/// Entidade concreta para testar a classe abstrata Entity
/// </summary>
public class EntityTesteConcreta : Entity
{
    public string Nome { get; private set; }
    public int Contador { get; private set; }
    
    public bool OnBeforeUpdateChamado { get; private set; }
    public bool OnAfterUpdateChamado { get; private set; }
    public DateTime? UpdatedAtAntes { get; private set; }
    public DateTime? UpdatedAtDepois { get; private set; }
    public string? NomeAntes { get; private set; }
    public string? NomeDepois { get; private set; }

    // Construtor padrão
    public EntityTesteConcreta(string nome)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Contador = 0;
    }
    
    // Construtor com parâmetros (para reconstituir de banco de dados, por exemplo)
    public EntityTesteConcreta(Guid id, bool isActive, DateTime createdAt, DateTime updatedAt, string nome, int contador)
        : base(id, isActive, createdAt, updatedAt)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Contador = contador;
    }

    public void AlterarNome(string novoNome)
    {
        if (string.IsNullOrWhiteSpace(novoNome))
            throw new ArgumentException("Nome não pode ser vazio", nameof(novoNome));

        Update(() =>
        {
            Nome = novoNome;
        });
    }

    public void IncrementarContador()
    {
        Update(() =>
        {
            Contador++;
        });
    }

    protected override void OnBeforeUpdate()
    {
        OnBeforeUpdateChamado = true;
        UpdatedAtAntes = UpdatedAt;
        NomeAntes = Nome;
    }

    protected override void OnAfterUpdate()
    {
        OnAfterUpdateChamado = true;
        UpdatedAtDepois = UpdatedAt;
        NomeDepois = Nome;
    }

    public void ResetarFlags()
    {
        OnBeforeUpdateChamado = false;
        OnAfterUpdateChamado = false;
        UpdatedAtAntes = null;
        UpdatedAtDepois = null;
        NomeAntes = null;
        NomeDepois = null;
    }
}

public class EntityTests
{
    #region Testes de Criação

    [Fact]
    public void DeveCriarEntityComIdAutomaticoGerado()
    {
        // Act
        var entity = new EntityTesteConcreta("Teste");

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
    }

    [Fact]
    public void DeveCriarEntityComIsActiveTrue()
    {
        // Act
        var entity = new EntityTesteConcreta("Teste");

        // Assert
        Assert.True(entity.IsActive);
    }

    [Fact]
    public void DeveCriarEntityComCreatedAtPreenchido()
    {
        // Arrange
        var antes = DateTime.UtcNow;

        // Act
        var entity = new EntityTesteConcreta("Teste");
        var depois = DateTime.UtcNow;

        // Assert
        Assert.True(entity.CreatedAt >= antes && entity.CreatedAt <= depois);
    }

    [Fact]
    public void DeveCriarEntityComUpdatedAtNulo()
    {
        // Act
        var entity = new EntityTesteConcreta("Teste");

        // Assert
        Assert.Null(entity.UpdatedAt);
    }

    [Fact]
    public void DeveCriarEntitiesComIdsUnicos()
    {
        // Act
        var entity1 = new EntityTesteConcreta("Teste 1");
        var entity2 = new EntityTesteConcreta("Teste 2");

        // Assert
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    #endregion

    #region Testes do Construtor com Parâmetros

    [Fact]
    public void DeveCriarEntityComIdEspecifico()
    {
        // Arrange
        var idEspecifico = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var updatedAt = DateTime.UtcNow.AddDays(-5);

        // Act
        var entity = new EntityTesteConcreta(idEspecifico, true, createdAt, updatedAt, "Teste", 5);

        // Assert
        Assert.Equal(idEspecifico, entity.Id);
    }

    [Fact]
    public void DeveCriarEntityComIsActiveSempreTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var updatedAt = DateTime.UtcNow.AddDays(-5);

        // Act - Mesmo passando false, deve ser true (conforme implementação)
        var entity = new EntityTesteConcreta(id, false, createdAt, updatedAt, "Teste", 5);

        // Assert
        Assert.True(entity.IsActive); // Sempre true conforme linha 21 da Entity
    }

    [Fact]
    public void DeveCriarEntityComCreatedAtEspecifico()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAtEspecifico = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
        var updatedAt = DateTime.UtcNow;

        // Act
        var entity = new EntityTesteConcreta(id, true, createdAtEspecifico, updatedAt, "Teste", 5);

        // Assert
        Assert.Equal(createdAtEspecifico, entity.CreatedAt);
    }

    [Fact]
    public void DeveCriarEntityComUpdatedAtEspecifico()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-10);
        var updatedAtEspecifico = new DateTime(2024, 1, 20, 15, 45, 0, DateTimeKind.Utc);

        // Act
        var entity = new EntityTesteConcreta(id, true, createdAt, updatedAtEspecifico, "Teste", 5);

        // Assert
        Assert.Equal(updatedAtEspecifico, entity.UpdatedAt);
    }

    [Fact]
    public void DeveCriarEntityComTodosOsParametrosEspecificos()
    {
        // Arrange
        var id = Guid.NewGuid();
        var createdAt = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var updatedAt = new DateTime(2024, 1, 10, 15, 30, 0, DateTimeKind.Utc);
        var nome = "Entidade Reconstituída";
        var contador = 42;

        // Act
        var entity = new EntityTesteConcreta(id, true, createdAt, updatedAt, nome, contador);

        // Assert
        Assert.Equal(id, entity.Id);
        Assert.True(entity.IsActive);
        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(updatedAt, entity.UpdatedAt);
        Assert.Equal(nome, entity.Nome);
        Assert.Equal(contador, entity.Contador);
    }

    #endregion

    #region Testes do Método Update

    [Fact]
    public void DeveExecutarActionAoAtualizar()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Nome Inicial");

        // Act
        entity.AlterarNome("Nome Alterado");

        // Assert
        Assert.Equal("Nome Alterado", entity.Nome);
    }

    [Fact]
    public void DeveAtualizarUpdatedAtAoAtualizar()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");
        Assert.Null(entity.UpdatedAt);
        
        var antes = DateTime.UtcNow;

        // Act
        entity.AlterarNome("Novo Nome");
        var depois = DateTime.UtcNow;

        // Assert
        Assert.NotNull(entity.UpdatedAt);
        Assert.True(entity.UpdatedAt >= antes && entity.UpdatedAt <= depois);
    }

    [Fact]
    public void DeveLancarExcecaoQuandoActionForNula()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() =>
        {
            var updateMethod = typeof(Entity).GetMethod("Update", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateMethod?.Invoke(entity, new object?[] { null });
        });
        
        Assert.IsType<ArgumentNullException>(exception.InnerException);
    }

    [Fact]
    public void DeveAtualizarUpdatedAtEmMultiplasAlteracoes()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act
        entity.AlterarNome("Nome 1");
        var primeiraAtualizacao = entity.UpdatedAt;
        
        Thread.Sleep(10); // Pequeno delay
        
        entity.AlterarNome("Nome 2");
        var segundaAtualizacao = entity.UpdatedAt;

        // Assert
        Assert.NotNull(primeiraAtualizacao);
        Assert.NotNull(segundaAtualizacao);
        Assert.True(segundaAtualizacao > primeiraAtualizacao);
    }

    #endregion

    #region Testes dos Hooks OnBeforeUpdate e OnAfterUpdate

    [Fact]
    public void DeveChamarOnBeforeUpdateAntesDeExecutarAction()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Nome Inicial");

        // Act
        entity.AlterarNome("Nome Alterado");

        // Assert
        Assert.True(entity.OnBeforeUpdateChamado);
    }

    [Fact]
    public void DeveChamarOnAfterUpdateAposExecutarAction()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act
        entity.AlterarNome("Novo Nome");

        // Assert
        Assert.True(entity.OnAfterUpdateChamado);
    }

    [Fact]
    public void OnBeforeUpdateDeveSerChamadoComUpdatedAtAindaNulo()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");
        Assert.Null(entity.UpdatedAt);

        // Act
        entity.AlterarNome("Novo Nome");

        // Assert
        Assert.Null(entity.UpdatedAtAntes); // UpdatedAt ainda era null no OnBeforeUpdate
    }

    [Fact]
    public void OnAfterUpdateDeveSerChamadoComUpdatedAtPreenchido()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act
        entity.AlterarNome("Novo Nome");

        // Assert
        Assert.NotNull(entity.UpdatedAtDepois); // UpdatedAt já foi preenchido no OnAfterUpdate
    }

    [Fact]
    public void OnBeforeUpdateDeveCapturarValorAntesDaAlteracao()
    {
        // Arrange
        var nomeInicial = "Nome Inicial";
        var entity = new EntityTesteConcreta(nomeInicial);

        // Act
        entity.AlterarNome("Nome Novo");

        // Assert
        Assert.Equal(nomeInicial, entity.NomeAntes); // Capturado no OnBeforeUpdate antes da alteração
        Assert.Equal("Nome Novo", entity.Nome); // Valor atual após alteração
    }

    [Fact]
    public void OnAfterUpdateDeveCapturarValorDepoisDaAlteracao()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Nome Inicial");
        var nomeNovo = "Nome Alterado";

        // Act
        entity.AlterarNome(nomeNovo);

        // Assert
        Assert.Equal(nomeNovo, entity.NomeDepois); // Capturado no OnAfterUpdate após alteração
        Assert.Equal(nomeNovo, entity.Nome); // Valor atual
    }

    [Fact]
    public void OnBeforeUpdateDeveSerChamadoAntesDeAlterarPropriedades()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Nome Inicial");

        // Act
        entity.AlterarNome("Nome Alterado");

        // Assert - Verifica que o hook foi chamado antes da alteração
        // O NomeAntes captura o estado no OnBeforeUpdate (deve ser o valor inicial)
        Assert.True(entity.OnBeforeUpdateChamado);
        Assert.Equal("Nome Inicial", entity.NomeAntes);
        Assert.Equal("Nome Alterado", entity.NomeDepois);
    }

    [Fact]
    public void OnBeforeUpdateDeveSerChamadoComUpdatedAtAnteriorNaSegundaAtualizacao()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Nome Inicial");
        
        // Primeira atualização
        entity.AlterarNome("Nome 1");
        var primeiroUpdatedAt = entity.UpdatedAt;
        
        // Reset e aguarda
        entity.ResetarFlags();
        Thread.Sleep(10);

        // Act - Segunda atualização
        entity.AlterarNome("Nome 2");

        // Assert - OnBeforeUpdate deve capturar o UpdatedAt da primeira atualização
        Assert.Equal(primeiroUpdatedAt, entity.UpdatedAtAntes);
    }

    [Fact]
    public void DeveChamarAmbosOsHooksEmCadaAtualizacao()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act
        entity.AlterarNome("Nome 1");

        // Assert
        Assert.True(entity.OnBeforeUpdateChamado);
        Assert.True(entity.OnAfterUpdateChamado);
    }

    [Fact]
    public void DeveChamarHooksEmMultiplasAtualizacoes()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act & Assert - Primeira atualização
        entity.AlterarNome("Nome 1");
        Assert.True(entity.OnBeforeUpdateChamado);
        Assert.True(entity.OnAfterUpdateChamado);

        // Reset para segunda atualização
        entity.ResetarFlags();
        Assert.False(entity.OnBeforeUpdateChamado);
        Assert.False(entity.OnAfterUpdateChamado);

        // Act & Assert - Segunda atualização
        entity.AlterarNome("Nome 2");
        Assert.True(entity.OnBeforeUpdateChamado);
        Assert.True(entity.OnAfterUpdateChamado);
    }

    [Fact]
    public void HooksVaziosNaoDevemCausarErros()
    {
        // Arrange - Usa entidade que não sobrescreve os hooks
        var entity = new EntitySemHooks("Teste");

        // Act - Não deve lançar exceção
        entity.AlterarNome("Novo Nome");

        // Assert
        Assert.Equal("Novo Nome", entity.Nome);
        Assert.NotNull(entity.UpdatedAt);
    }

    [Fact]
    public void DeveChamarOnBeforeUpdateAntesDeChamarOnAfterUpdate()
    {
        // Arrange
        var ordemChamadas = new List<string>();
        var entity = new EntityComOrdemDeLog("Teste", ordemChamadas);

        // Act
        entity.AlterarNome("Novo Nome");

        // Assert
        Assert.Equal(3, ordemChamadas.Count);
        Assert.Equal("OnBeforeUpdate", ordemChamadas[0]);
        Assert.Equal("Action", ordemChamadas[1]);
        Assert.Equal("OnAfterUpdate", ordemChamadas[2]);
    }

    #endregion

    #region Testes de Igualdade

    [Fact]
    public void DeveSerIgualQuandoMesmaInstancia()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act & Assert
        Assert.True(entity.Equals(entity));
        Assert.Equal(entity, entity);
    }

    [Fact]
    public void DeveSerIgualQuandoMesmoId()
    {
        // Arrange
        var entity1 = new EntityTesteConcreta("Teste 1");
        var entity2 = entity1;

        // Act & Assert
        Assert.True(entity1.Equals(entity2));
        Assert.Equal(entity1, entity2);
    }

    [Fact]
    public void NaoDeveSerIgualQuandoIdsDistintos()
    {
        // Arrange
        var entity1 = new EntityTesteConcreta("Teste");
        var entity2 = new EntityTesteConcreta("Teste");

        // Act & Assert
        Assert.False(entity1.Equals(entity2));
        Assert.NotEqual(entity1, entity2);
    }

    [Fact]
    public void NaoDeveSerIgualANull()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");
        Entity? entityNula = null;

        // Act & Assert
        Assert.False(entity.Equals(entityNula));
    }

    [Fact]
    public void DeveRetornarHashCodeBaseadoNoId()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act
        var hashCode = entity.GetHashCode();

        // Assert
        Assert.Equal(entity.Id.GetHashCode(), hashCode);
    }

    [Fact]
    public void EntitiesComMesmoIdDevemTerMesmoHashCode()
    {
        // Arrange
        var entity1 = new EntityTesteConcreta("Teste");
        var entity2 = entity1;

        // Act & Assert
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    #endregion

    #region Testes do ToString

    [Fact]
    public void ToStringDeveRetornarNomeDaClasseEId()
    {
        // Arrange
        var entity = new EntityTesteConcreta("Teste");

        // Act
        var resultado = entity.ToString();

        // Assert
        Assert.Contains("EntityTesteConcreta", resultado);
        Assert.Contains(entity.Id.ToString(), resultado);
    }

    #endregion
}

/// <summary>
/// Entidade auxiliar para testar ordem de execução dos hooks
/// </summary>
public class EntityTesteComLog : Entity
{
    public string Nome { get; private set; }
    private readonly Action? _onBeforeCallback;

    public EntityTesteComLog(string nome, Action? onBeforeCallback = null)
    {
        Nome = nome;
        _onBeforeCallback = onBeforeCallback;
    }

    public void AlterarNome(string novoNome)
    {
        Update(() => Nome = novoNome);
    }

    protected override void OnBeforeUpdate()
    {
        _onBeforeCallback?.Invoke();
    }
}

/// <summary>
/// Entidade que não sobrescreve os hooks (testa comportamento padrão)
/// </summary>
public class EntitySemHooks : Entity
{
    public string Nome { get; private set; }

    public EntitySemHooks(string nome)
    {
        Nome = nome;
    }

    public void AlterarNome(string novoNome)
    {
        Update(() => Nome = novoNome);
    }
    
    // Não sobrescreve OnBeforeUpdate e OnAfterUpdate
    // Usa implementação padrão (vazia) da classe base
}

/// <summary>
/// Entidade que registra ordem de chamadas dos hooks
/// </summary>
public class EntityComOrdemDeLog : Entity
{
    public string Nome { get; private set; }
    private readonly List<string> _ordemChamadas;

    public EntityComOrdemDeLog(string nome, List<string> ordemChamadas)
    {
        Nome = nome;
        _ordemChamadas = ordemChamadas;
    }

    public void AlterarNome(string novoNome)
    {
        Update(() =>
        {
            _ordemChamadas.Add("Action");
            Nome = novoNome;
        });
    }

    protected override void OnBeforeUpdate()
    {
        _ordemChamadas.Add("OnBeforeUpdate");
    }

    protected override void OnAfterUpdate()
    {
        _ordemChamadas.Add("OnAfterUpdate");
    }
}



