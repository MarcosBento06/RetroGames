
using Moq;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Interfaces;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogoRepositoryMock;
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock;
    private readonly Mock<ICondicaoJogoRepository> _condicaoRepositoryMock;
    private readonly Mock<IGeneroRepository> _generoRepositoryMock;
    private readonly Mock<IJogoGeneroRepository> _jogoGeneroRepositoryMock;

    private readonly Mock<IRepositorySet> _repositorySetMock;
    private readonly Mock<IUsuarioAtualService> _usuarioAtualMock;

    public JogoServiceTests()
    {
        // Arrange — Criar os mocks dos repositórios
        _jogoRepositoryMock = new Mock<IJogoRepository>();
        _plataformaRepositoryMock = new Mock<IPlataformaRepository>();
        _condicaoRepositoryMock = new Mock<ICondicaoJogoRepository>();
        _generoRepositoryMock = new Mock<IGeneroRepository>();
        _jogoGeneroRepositoryMock = new Mock<IJogoGeneroRepository>();

        // Mock do conjunto de repositórios
        _repositorySetMock = new Mock<IRepositorySet>();

        _repositorySetMock
            .Setup(r => r.Jogo)
            .Returns(_jogoRepositoryMock.Object);

        _repositorySetMock
            .Setup(r => r.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);

        _repositorySetMock
            .Setup(r => r.CondicaoJogo)
            .Returns(_condicaoRepositoryMock.Object);

        _repositorySetMock
            .Setup(r => r.Genero)
            .Returns(_generoRepositoryMock.Object);

        _repositorySetMock
            .Setup(r => r.JogoGenero)
            .Returns(_jogoGeneroRepositoryMock.Object);

        // Mock do usuário autenticado
        _usuarioAtualMock = new Mock<IUsuarioAtualService>();
    }

    private JogoService CriarServico()
    {
        return new JogoService(
            _repositorySetMock.Object,
            _usuarioAtualMock.Object
        );
    }

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaNaoEncontradaENaoGrava()
    {
        // Arrange
        var jogo = new Jogo
        {
            Nome = "Super Mario Bros.",
            IdPlataforma = 999,
            IdCondicaoJogo = 1
        };

        int[] idsGeneros = { 1 };

        _plataformaRepositoryMock
            .Setup(repositorio =>
                repositorio.BuscarPorIdAsync(999))
            .ReturnsAsync((Plataforma?)null);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(jogo, idsGeneros)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA.Code,
            excecao.Erro.Code
        );

        _jogoRepositoryMock.Verify(
            repositorio =>
                repositorio.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaNaoEncontradaENaoGrava()
    {
        // Arrange
        var jogo = new Jogo
        {
            Nome = "Sonic the Hedgehog",
            IdPlataforma = 1,
            IdCondicaoJogo = 999
        };

        int[] idsGeneros = { 1 };

        var plataforma = new Plataforma
        {
            Id = 1,
            Nome = "Mega Drive",
            Fabricante = "Sega",
            AnoLancamento = 1988
        };

        _plataformaRepositoryMock
            .Setup(repositorio =>
                repositorio.BuscarPorIdAsync(1))
            .ReturnsAsync(plataforma);

        _condicaoRepositoryMock
            .Setup(repositorio =>
                repositorio.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo>());

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(jogo, idsGeneros)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.CONDICAO_NAO_ENCONTRADA.Code,
            excecao.Erro.Code
        );

        _condicaoRepositoryMock.Verify(
            repositorio => repositorio.ListarAsync(),
            Times.Once
        );

        _jogoRepositoryMock.Verify(
            repositorio =>
                repositorio.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );

        _jogoGeneroRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<int>>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Inserir_GeneroInexistente_LancaNaoEncontradoENaoGrava()
    {
        // Arrange
        var jogo = new Jogo
        {
            Nome = "The Legend of Zelda",
            IdPlataforma = 1,
            IdCondicaoJogo = 1
        };

        int[] idsGeneros = { 999 };

        var plataforma = new Plataforma
        {
            Id = 1,
            Nome = "Nintendo Entertainment System",
            Fabricante = "Nintendo",
            AnoLancamento = 1983
        };

        var condicao = new CondicaoJogo
        {
            Id = 1,
            Descricao = "Usado"
        };

        _plataformaRepositoryMock
            .Setup(repositorio =>
                repositorio.BuscarPorIdAsync(1))
            .ReturnsAsync(plataforma);

        _condicaoRepositoryMock
            .Setup(repositorio =>
                repositorio.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo> { condicao });

        _generoRepositoryMock
            .Setup(repositorio =>
                repositorio.ListarAsync())
            .ReturnsAsync(new List<Genero>());

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(jogo, idsGeneros)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.GENERO_NAO_ENCONTRADO.Code,
            excecao.Erro.Code
        );

        _generoRepositoryMock.Verify(
            repositorio => repositorio.ListarAsync(),
            Times.Once
        );

        _jogoRepositoryMock.Verify(
            repositorio =>
                repositorio.InserirAsync(It.IsAny<Jogo>()),
            Times.Never
        );

        _jogoGeneroRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyCollection<int>>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Listar_ComFiltroGenero_EnviaSomenteIdsDoGenero()
    {
        // Arrange
        int idGenero = 3;

        var vinculos = new List<JogoGenero>
        {
            new JogoGenero { IdJogo = 5, IdGenero = 3 },
            new JogoGenero { IdJogo = 8, IdGenero = 3 },
            new JogoGenero { IdJogo = 5, IdGenero = 3 },
            new JogoGenero { IdJogo = 12, IdGenero = 4 }
        };

        _jogoGeneroRepositoryMock
            .Setup(repositorio => repositorio.ListarTodosAsync())
            .ReturnsAsync(vinculos);

        _jogoRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync(
                It.IsAny<string?>(),
                It.IsAny<int?>(),
                It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            ))
            .ReturnsAsync((new List<Jogo>(), 0));

        var servico = CriarServico();

        // Act
        await servico.ListarAsync(idGenero: idGenero);

        // Assert
        _jogoGeneroRepositoryMock.Verify(
            repositorio => repositorio.ListarTodosAsync(),
            Times.Once
        );

        _jogoRepositoryMock.Verify(
            repositorio => repositorio.ListarAsync(
                null,
                null,
                It.Is<IReadOnlyCollection<int>?>(
                    ids => ids != null &&
                        ids.Count == 2 &&
                        ids.Contains(5) &&
                        ids.Contains(8)
                ),
                false,
                1,
                10
            ),
            Times.Once
        );
    }


    [Fact]
    public async Task Listar_SemFiltroGenero_NaoConsultaVinculosNemRestringeIds()
    {
        // Arrange
        _jogoRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync(
                It.IsAny<string?>(),
                It.IsAny<int?>(),
                It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            ))
            .ReturnsAsync((new List<Jogo>(), 0));

        var servico = CriarServico();

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        _jogoGeneroRepositoryMock.Verify(
            repositorio => repositorio.ListarTodosAsync(),
            Times.Never
        );

        _jogoRepositoryMock.Verify(
            repositorio => repositorio.ListarAsync(
                null,
                null,
                null,
                false,
                1,
                10
            ),
            Times.Once
        );

        Assert.Empty(resultado.Jogos);
        Assert.Equal(0, resultado.Total);
    }


    [Fact]
    public async Task Listar_ComJogos_MontaDetalhesCorretamente()
    {
        // Arrange
        var jogo = new Jogo
        {
            Id = 5,
            Nome = "Super Mario World",
            IdPlataforma = 2,
            IdCondicaoJogo = 1,
            Ativo = true
        };

        var plataforma = new Plataforma
        {
            Id = 2,
            Nome = "Super Nintendo",
            Fabricante = "Nintendo",
            AnoLancamento = 1990
        };

        var condicao = new CondicaoJogo
        {
            Id = 1,
            Descricao = "Usado"
        };

        var genero = new Genero
        {
            Id = 3,
            Nome = "Plataforma"
        };

        var vinculo = new JogoGenero
        {
            IdJogo = 5,
            IdGenero = 3
        };

        _jogoRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync(
                It.IsAny<string?>(),
                It.IsAny<int?>(),
                It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()
            ))
            .ReturnsAsync((new List<Jogo> { jogo }, 1));

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<Plataforma> { plataforma });

        _condicaoRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<CondicaoJogo> { condicao });

        _generoRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(new List<Genero> { genero });

        _jogoGeneroRepositoryMock
            .Setup(repositorio => repositorio.ListarPorJogosAsync(
                It.IsAny<IReadOnlyCollection<int>>()
            ))
            .ReturnsAsync(new List<JogoGenero> { vinculo });

        var servico = CriarServico();

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        Assert.Single(resultado.Jogos);
        Assert.Equal(1, resultado.Total);

        var detalhe = resultado.Jogos[0];

        Assert.Equal(5, detalhe.Jogo.Id);
        Assert.Equal("Super Mario World", detalhe.Jogo.Nome);

        Assert.Equal("Super Nintendo", detalhe.NomePlataforma);
        Assert.Equal("Usado", detalhe.DescricaoCondicao);

        Assert.Single(detalhe.Generos);
        Assert.Equal("Plataforma", detalhe.Generos[0].Nome);

        _jogoGeneroRepositoryMock.Verify(
            repositorio => repositorio.ListarPorJogosAsync(
                It.Is<IReadOnlyCollection<int>>(
                    ids => ids.Count == 1 && ids.Contains(5)
                )
            ),
            Times.Once
        );
    }


    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrado()
    {
        // Arrange
        int idInexistente = 999;

        _jogoRepositoryMock
            .Setup(repositorio =>
                repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Jogo?)null);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.BuscarPorIdAsync(idInexistente)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.JOGO_NAO_ENCONTRADO.Code,
            excecao.Erro.Code
        );

        _jogoRepositoryMock.Verify(
            repositorio =>
                repositorio.BuscarPorIdAsync(idInexistente),
            Times.Once
        );
    }


    [Fact]
    public async Task Desativar_IdInexistente_LancaNaoEncontradoENaoDesativa()
    {
        // Arrange
        int idInexistente = 999;

        _jogoRepositoryMock
            .Setup(repositorio =>
                repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Jogo?)null);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.DesativarAsync(idInexistente)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.JOGO_NAO_ENCONTRADO.Code,
            excecao.Erro.Code
        );

        _jogoRepositoryMock.Verify(
            repositorio =>
                repositorio.BuscarPorIdAsync(idInexistente),
            Times.Once
        );

        _jogoRepositoryMock.Verify(
            repositorio =>
                repositorio.DesativarAsync(It.IsAny<int>()),
            Times.Never
        );
    }


}
