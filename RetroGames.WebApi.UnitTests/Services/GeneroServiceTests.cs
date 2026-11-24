
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    [Fact]
    public async Task Listar_ComGeneros_RetornaGenerosDoRepository()
    {
        // Arrange
        var generos = new List<Genero>
        {
            new Genero
            {
                Id = 1,
                Nome = "Ação"
            },
            new Genero
            {
                Id = 2,
                Nome = "Aventura"
            }
        };

        var generoRepositoryMock = new Mock<IGeneroRepository>();
        var repositorySetMock = new Mock<IRepositorySet>();

        generoRepositoryMock
            .Setup(repositorio => repositorio.ListarAsync())
            .ReturnsAsync(generos);

        repositorySetMock
            .Setup(repositorio => repositorio.Genero)
            .Returns(generoRepositoryMock.Object);

        var servico = new GeneroService(repositorySetMock.Object);

        // Act
        var resultado = await servico.ListarAsync();

        // Assert
        Assert.Equal(2, resultado.Count);

        Assert.Equal("Ação", resultado[0].Nome);
        Assert.Equal("Aventura", resultado[1].Nome);

        generoRepositoryMock.Verify(
            repositorio => repositorio.ListarAsync(),
            Times.Once
        );
    }
}
