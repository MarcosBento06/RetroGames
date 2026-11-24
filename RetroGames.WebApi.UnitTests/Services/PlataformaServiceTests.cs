
using Moq;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformaRepositoryMock;
    private readonly Mock<IRepositorySet> _repositorySetMock;

    public PlataformaServiceTests()
    {
        _plataformaRepositoryMock = new Mock<IPlataformaRepository>();
        _repositorySetMock = new Mock<IRepositorySet>();

        _repositorySetMock
            .Setup(repositorio => repositorio.Plataforma)
            .Returns(_plataformaRepositoryMock.Object);
    }

    private PlataformaService CriarServico()
    {
        return new PlataformaService(_repositorySetMock.Object);
    }


    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange — Preparar o cenário
        int idInexistente = 999;        

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorIdAsync(idInexistente))
            .ReturnsAsync((Plataforma?)null);

        var servico = CriarServico();

        // Act — Executar o método
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.BuscarPorIdAsync(idInexistente)
        );

        // Assert — Conferir o resultado
        Assert.Equal(
            BusinessErrors.PLATAFORMA_NAO_ENCONTRADA.Code,
            excecao.Erro.Code
        );

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.BuscarPorIdAsync(idInexistente),
            Times.Once
        );
    }


    [Fact]
    public async Task Inserir_NomeJaCadastrado_LancaNomeDuplicadoENaoGrava()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "Super Nintendo",
            Fabricante = "Nintendo",
            AnoLancamento = 1990
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                "Super Nintendo", null))
            .ReturnsAsync(true);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO.Code,
            excecao.Erro.Code
        );

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<Plataforma>()),
            Times.Never
        );
    }


    [Fact]
    public async Task Inserir_RepositoryLancaDuplicado_LancaNomeDuplicado()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "PlayStation",
            Fabricante = "Sony",
            AnoLancamento = 1994
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                "PlayStation", null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.InserirAsync(
                It.IsAny<Plataforma>()))
            .ThrowsAsync(
                new RegistroDuplicadoException(
                    new Exception("Nome duplicado no banco.")
                )
            );

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.InserirAsync(plataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_NOME_DUPLICADO.Code,
            excecao.Erro.Code
        );

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<Plataforma>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Inserir_NomeLivre_RetornaPlataformaComIdGerado()
    {
        // Arrange
        var plataforma = new Plataforma
        {
            Nome = "Mega Drive",
            Fabricante = "Sega",
            AnoLancamento = 1988
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                "Mega Drive", null))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.InserirAsync(
                It.IsAny<Plataforma>()))
            .ReturnsAsync(42);

        var servico = CriarServico();

        // Act
        var resultado = await servico.InserirAsync(plataforma);

        // Assert
        Assert.Equal(42, resultado.Id);
        Assert.Equal("Mega Drive", resultado.Nome);

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.InserirAsync(
                It.IsAny<Plataforma>()),
            Times.Once
        );
    }


    [Fact]
    public async Task Alterar_MantendoProprioNome_AlteraComSucesso()
    {
        // Arrange
        var plataformaExistente = new Plataforma
        {
            Id = 7,
            Nome = "PlayStation",
            Fabricante = "Sony",
            AnoLancamento = 1994
        };

        var plataformaAlterada = new Plataforma
        {
            Id = 7,
            Nome = "PlayStation",
            Fabricante = "Sony Interactive Entertainment",
            AnoLancamento = 1994
        };

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.BuscarPorIdAsync(7))
            .ReturnsAsync(plataformaExistente);

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.ExisteComNomeAsync(
                "PlayStation", 7))
            .ReturnsAsync(false);

        _plataformaRepositoryMock
            .Setup(repositorio => repositorio.AlterarAsync(
                It.IsAny<Plataforma>()))
            .Returns(Task.CompletedTask);

        var servico = CriarServico();

        // Act
        await servico.AlterarAsync(plataformaAlterada);

        // Assert
        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.ExisteComNomeAsync(
                "PlayStation", 7),
            Times.Once
        );

        _plataformaRepositoryMock.Verify(
            repositorio => repositorio.AlterarAsync(
                It.Is<Plataforma>(plataforma =>
                    plataforma.Id == 7 &&
                    plataforma.Nome == "PlayStation" &&
                    plataforma.Fabricante == "Sony Interactive Entertainment"
                )),
            Times.Once
        );
    }


    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogosENaoExclui()
    {
        // Arrange
        int idPlataforma = 7;

        var plataforma = new Plataforma
        {
            Id = idPlataforma,
            Nome = "Super Nintendo",
            Fabricante = "Nintendo",
            AnoLancamento = 1990
        };

        _plataformaRepositoryMock
            .Setup(repositorio =>
                repositorio.BuscarPorIdAsync(idPlataforma))
            .ReturnsAsync(plataforma);

        _plataformaRepositoryMock
            .Setup(repositorio =>
                repositorio.PossuiJogosAsync(idPlataforma))
            .ReturnsAsync(true);

        var servico = CriarServico();

        // Act
        var excecao = await Assert.ThrowsAsync<BusinessRuleException>(
            () => servico.ExcluirAsync(idPlataforma)
        );

        // Assert
        Assert.Equal(
            BusinessErrors.PLATAFORMA_POSSUI_JOGOS.Code,
            excecao.Erro.Code
        );

        _plataformaRepositoryMock.Verify(
            repositorio =>
                repositorio.PossuiJogosAsync(idPlataforma),
            Times.Once
        );

        _plataformaRepositoryMock.Verify(
            repositorio =>
                repositorio.ExcluirAsync(It.IsAny<int>()),
            Times.Never
        );
    }


}
