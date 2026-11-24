
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_ComCondicoes_RetornaCondicoesDoRepository()
    {
        // Arrange
        var condicoes = new List<CondicaoJogo>
        {
            new CondicaoJogo
            {
                Id = 1,
                Descricao = "Novo"
            },
            new CondicaoJogo
            {
                Id = 2,
                Descricao = "Usado"
            },
            new CondicaoJogo
            {
                Id = 3,
                Descricao = "Recondicionado"
            }
        };

        var condicaoRepositoryMock =
            new Mock<ICondicaoJogoRepository>();

        var repositorySetMock = new Mock<IRepositorySet>();

        condicaoRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(condicoes);

        repositorySetMock
            .Setup(repositorio => repositorio.CondicaoJogo)
            .Returns(condicaoRepositoryMock.Object);

        var servico = new CondicaoJogoService(
            repositorySetMock.Object
        );

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        Assert.Equal(3, resultado.Count);

        Assert.Equal("Novo", resultado[0].Descricao);
        Assert.Equal("Usado", resultado[1].Descricao);
        Assert.Equal("Recondicionado", resultado[2].Descricao);

        condicaoRepositoryMock.Verify(
            repositorio => repositorio.ListarAsync(),
            Times.Once
        );
    }
}
