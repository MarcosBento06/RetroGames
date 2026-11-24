
using Moq;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Model.Model;

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

}
