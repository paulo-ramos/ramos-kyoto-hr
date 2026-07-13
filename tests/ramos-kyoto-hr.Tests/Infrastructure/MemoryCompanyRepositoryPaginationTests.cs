using FluentAssertions;
using ramos_kyoto_hr.Domain.Entities;
using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Infrastructure.Persistence;
using Xunit;

namespace ramos_kyoto_hr.Tests.Infrastructure;

/// <summary>
/// Testes para validar a implementação de paginação do MemoryCompanyRepository
/// </summary>
public class MemoryCompanyRepositoryPaginationTests : IDisposable
{
    private readonly MemoryCompanyRepository _repository;
    private readonly DateOnly _effectiveDate;
    private readonly List<Company> _testCompanies;

    public MemoryCompanyRepositoryPaginationTests()
    {
        _repository = new MemoryCompanyRepository();
        _effectiveDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _testCompanies = new List<Company>();
    }

    public void Dispose()
    {
        // Limpar repositório após cada teste
        foreach (var company in _testCompanies)
        {
            try
            {
                _repository.DeleteAsync(company.Id, TestContext.Current.CancellationToken).Wait();
            }
            catch
            {
                // Ignora erros de limpeza
            }
        }
    }

    private async Task<Company> CreateAndAddCompany(string cnpj, string razaoSocial, bool isActive = true)
    {
        var company = new Company(
            _effectiveDate,
            RazaoSocial.Create(razaoSocial),
            Cnpj.Create(cnpj)
        );

        if (!isActive)
        {
            company.Disable(_effectiveDate);
        }

        await _repository.AddAsync(company, TestContext.Current.CancellationToken);
        _testCompanies.Add(company);
        
        return company;
    }

    #region Basic Pagination Tests

    [Fact]
    public async Task GetPagedAsync_WithValidPageAndSize_ShouldReturnCorrectPage()
    {
        // Arrange
        await CreateAndAddCompany("ZN.0RH.W1P/0001-65", "Empresa A");
        await CreateAndAddCompany("G5.1Z4.CS0/0001-39", "Empresa B");
        await CreateAndAddCompany("RW.3YJ.B8X/0001-72", "Empresa C");
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa D");
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Empresa E");

        // Act - Página 1 (2 itens por página)
        var (itemsPage1, totalCount1) = await _repository.GetPagedAsync(null, null, 1, 2, TestContext.Current.CancellationToken);

        // Assert - Página 1
        itemsPage1.Should().HaveCount(2);
        totalCount1.Should().Be(5);

        // Act - Página 2
        var (itemsPage2, totalCount2) = await _repository.GetPagedAsync(null, null, 2, 2, TestContext.Current.CancellationToken);

        // Assert - Página 2
        itemsPage2.Should().HaveCount(2);
        totalCount2.Should().Be(5);

        // Act - Página 3 (última página com 1 item)
        var (itemsPage3, totalCount3) = await _repository.GetPagedAsync(null, null, 3, 2, TestContext.Current.CancellationToken);

        // Assert - Página 3
        itemsPage3.Should().HaveCount(1);
        totalCount3.Should().Be(5);
    }

    [Fact]
    public async Task GetPagedAsync_WithPageBeyondResults_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Empresa A");
        await CreateAndAddCompany("9V.N4S.E4L/0001-38", "Empresa B");

        // Act - Página 10 (além do total de registros)
        var (items, totalCount) = await _repository.GetPagedAsync(null, null, 10, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().BeEmpty();
        totalCount.Should().Be(2); // Total continua sendo 2
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidPage_ShouldThrowException()
    {
        // Act & Assert
        await FluentActions.Invoking(async () =>
            await _repository.GetPagedAsync(null, null, 0, 10, TestContext.Current.CancellationToken))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*número da página deve ser maior ou igual a 1*");
    }

    [Fact]
    public async Task GetPagedAsync_WithInvalidPageSize_ShouldThrowException()
    {
        // Act & Assert
        await FluentActions.Invoking(async () =>
            await _repository.GetPagedAsync(null, null, 1, 0, TestContext.Current.CancellationToken))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*tamanho da página deve ser maior ou igual a 1*");
    }

    #endregion

    #region Search Term Filter Tests

    [Fact]
    public async Task GetPagedAsync_WithSearchTermInRazaoSocial_ShouldFilterCorrectly()
    {
        // Arrange
        await CreateAndAddCompany("MY.4AC.J50/0001-09", "Tech Solutions Ltda");
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Tech Innovations Inc");
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Business Corp");

        // Act - Buscar por Razão Social contendo "Tech"
        var (items, totalCount) = await _repository.GetPagedAsync("Tech", null, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(c => c.RazaoSocial.Valor.Contains("Tech", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTermNotFound_ShouldReturnEmpty()
    {
        // Arrange
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa Alpha");
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Empresa Beta");

        // Act - Buscar por termo inexistente
        var (items, totalCount) = await _repository.GetPagedAsync("XYZABC123", null, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().BeEmpty();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTermCaseInsensitive_ShouldWork()
    {
        // Arrange
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "EMPRESA MAIÚSCULA");
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "empresa minúscula");

        // Act - Buscar com diferentes cases
        var (items1, _) = await _repository.GetPagedAsync("EMPRESA", null, 1, 10, TestContext.Current.CancellationToken);
        var (items2, _) = await _repository.GetPagedAsync("empresa", null, 1, 10, TestContext.Current.CancellationToken);
        var (items3, _) = await _repository.GetPagedAsync("EmPrEsA", null, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items1.Should().HaveCount(2);
        items2.Should().HaveCount(2);
        items3.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_WithEmptySearchTerm_ShouldReturnAll()
    {
        // Arrange
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Empresa A");
        await CreateAndAddCompany("9V.N4S.E4L/0001-38", "Empresa B");
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa C");

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync("", null, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().HaveCount(3);
        totalCount.Should().Be(3);
    }

    #endregion

    #region Active Status Filter Tests

    [Fact]
    public async Task GetPagedAsync_FilterByActiveTrue_ShouldReturnOnlyActive()
    {
        // Arrange
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Empresa Ativa 1", isActive: true);
        await CreateAndAddCompany("9V.N4S.E4L/0001-38", "Empresa Ativa 2", isActive: true);
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa Inativa 1", isActive: false);
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Empresa Inativa 2", isActive: false);

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(null, isActive: true, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(c => c.IsActive == true);
    }

    [Fact]
    public async Task GetPagedAsync_FilterByActiveFalse_ShouldReturnOnlyInactive()
    {
        // Arrange
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Empresa Ativa 1", isActive: true);
        await CreateAndAddCompany("9V.N4S.E4L/0001-38", "Empresa Ativa 2", isActive: true);
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa Inativa 1", isActive: false);
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Empresa Inativa 2", isActive: false);

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(null, isActive: false, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(c => c.IsActive == false);
    }

    [Fact]
    public async Task GetPagedAsync_FilterByActiveNull_ShouldReturnAll()
    {
        // Arrange
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Empresa Ativa 1", isActive: true);
        await CreateAndAddCompany("9V.N4S.E4L/0001-38", "Empresa Ativa 2", isActive: true);
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa Inativa 1", isActive: false);

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(null, isActive: null, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().HaveCount(3);
        totalCount.Should().Be(3);
    }

    #endregion

    #region Combined Filters Tests

    [Fact]
    public async Task GetPagedAsync_WithSearchTermAndActiveFilter_ShouldCombineFilters()
    {
        // Arrange
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Tech Solutions Ativa", isActive: true);
        await CreateAndAddCompany("9V.N4S.E4L/0001-38", "Tech Innovations Ativa", isActive: true);
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Tech Corp Inativa", isActive: false);
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Business Corp Ativa", isActive: true);

        // Act - Buscar "Tech" e apenas ativas
        var (items, totalCount) = await _repository.GetPagedAsync("Tech", isActive: true, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(c => 
            c.RazaoSocial.Valor.Contains("Tech", StringComparison.OrdinalIgnoreCase) && 
            c.IsActive == true);
    }

    #endregion

    #region Ordering Tests

    [Fact]
    public async Task GetPagedAsync_ShouldOrderByRazaoSocialThenByCnpj()
    {
        // Arrange - Criar empresas em ordem aleatória
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Zebra Corp");
        await CreateAndAddCompany("9V.N4S.E4L/0001-38", "Alpha Inc");
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Beta Ltd");
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Alpha Inc"); // Mesma razão social, CNPJ diferente

        // Act
        var (items, _) = await _repository.GetPagedAsync(null, null, 1, 10, TestContext.Current.CancellationToken);
        var itemsList = items.ToList();

        // Assert - Deve estar ordenado por Razão Social e depois por CNPJ
        itemsList[0].RazaoSocial.Valor.Should().Be("Alpha Inc");
        itemsList[0].Cnpj.Valor.Should().Be("9VN4SE4L000138");
        
        itemsList[1].RazaoSocial.Valor.Should().Be("Alpha Inc");
        itemsList[1].Cnpj.Valor.Should().Be("AM0WRY5D000181");
        
        itemsList[2].RazaoSocial.Valor.Should().Be("Beta Ltd");
        itemsList[3].RazaoSocial.Valor.Should().Be("Zebra Corp");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task GetPagedAsync_WithEmptyRepository_ShouldReturnEmpty()
    {
        // Act
        var (items, totalCount) = await _repository.GetPagedAsync(null, null, 1, 10, TestContext.Current.CancellationToken);

        // Assert
        items.Should().BeEmpty();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetPagedAsync_WithPageSizeGreaterThanTotal_ShouldReturnAllItems()
    {
        // Arrange
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa A");
        await CreateAndAddCompany("4N.C2H.A4M/0001-25", "Empresa B");

        // Act - Page size maior que o total
        var (items, totalCount) = await _repository.GetPagedAsync(null, null, 1, 100, TestContext.Current.CancellationToken);

        // Assert
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetPagedAsync_WithWhitespaceSearchTerm_ShouldTreatAsNull()
    {
        // Arrange
        await CreateAndAddCompany("NA.47B.WTA/0001-44", "Empresa A");
        await CreateAndAddCompany("AM.0WR.Y5D/0001-81", "Empresa B");

        // Act
        var (items, totalCount) = await _repository.GetPagedAsync("   ", null, 1, 10, TestContext.Current.CancellationToken);

        // Assert - Whitespace deve ser tratado como null/empty
        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
    }

    #endregion
}
