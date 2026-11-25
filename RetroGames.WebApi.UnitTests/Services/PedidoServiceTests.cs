using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    [Fact]
    public async Task FinalizarCompra_ComDadosValidos_CriaPedidoEItensEBaixaEstoque()
    {
        // Arrange
        var jogoRepository = new Mock<IJogoRepository>();
        var pedidoRepository = new Mock<IPedidoRepository>();
        var itemPedidoRepository = new Mock<IItemPedidoRepository>();
        var repositorySet = new Mock<IRepositorySet>();
        var usuarioAtual = new Mock<IUsuarioAtualService>();

        repositorySet
            .Setup(r => r.Jogo)
            .Returns(jogoRepository.Object);

        repositorySet
            .Setup(r => r.Pedido)
            .Returns(pedidoRepository.Object);

        repositorySet
            .Setup(r => r.ItemPedido)
            .Returns(itemPedidoRepository.Object);

        usuarioAtual
            .Setup(u => u.IdUsuario)
            .Returns(10);

        var jogo = new Jogo
        {
            Id = 1,
            Nome = "Super Mario Bros.",
            Preco = 100m,
            Estoque = 5,
            Ativo = true
        };

        jogoRepository
            .Setup(r => r.BuscarPorIdAsync(1))
            .ReturnsAsync(jogo);

        pedidoRepository
            .Setup(r => r.InserirAsync(It.IsAny<Pedido>()))
            .ReturnsAsync(50);

        itemPedidoRepository
            .Setup(r => r.InserirAsync(It.IsAny<ItemPedido>()))
            .ReturnsAsync(1);

        jogoRepository
            .Setup(r => r.BaixarEstoqueAsync(1, 2))
            .ReturnsAsync(1);

        var itens = new List<ItemPedido>
        {
            new()
            {
                IdJogo = 1,
                Quantidade = 2
            }
        };

        var servico = new PedidoService(
            repositorySet.Object,
            usuarioAtual.Object);

        // Act
        var idPedido = await servico.FinalizarCompraAsync(itens);

        // Assert
        Assert.Equal(50, idPedido);

        pedidoRepository.Verify(
            r => r.InserirAsync(It.Is<Pedido>(p =>
                p.IdUsuario == 10 &&
                p.IdSituacaoPedido == 1 &&
                p.ValorTotal == 200m &&
                p.DataPedido.Kind == DateTimeKind.Utc)),
            Times.Once);

        itemPedidoRepository.Verify(
            r => r.InserirAsync(It.Is<ItemPedido>(i =>
                i.IdPedido == 50 &&
                i.IdJogo == 1 &&
                i.Quantidade == 2 &&
                i.ValorUnitario == 100m)),
            Times.Once);

        jogoRepository.Verify(
            r => r.BaixarEstoqueAsync(1, 2),
            Times.Once);
    }

    [Fact]
    public async Task FinalizarCompra_SemItens_LancaExcecaoENaoGravaPedido()
    {
        // Arrange
        var jogoRepository = new Mock<IJogoRepository>();
        var pedidoRepository = new Mock<IPedidoRepository>();
        var itemPedidoRepository = new Mock<IItemPedidoRepository>();
        var repositorySet = new Mock<IRepositorySet>();
        var usuarioAtual = new Mock<IUsuarioAtualService>();

        repositorySet
            .Setup(r => r.Jogo)
            .Returns(jogoRepository.Object);

        repositorySet
            .Setup(r => r.Pedido)
            .Returns(pedidoRepository.Object);

        repositorySet
            .Setup(r => r.ItemPedido)
            .Returns(itemPedidoRepository.Object);

        var servico = new PedidoService(
            repositorySet.Object,
            usuarioAtual.Object);

        var itens = new List<ItemPedido>();

        // Act
        var excecao = await Assert.ThrowsAsync<ArgumentException>(
            () => servico.FinalizarCompraAsync(itens));

        // Assert
        Assert.Contains("item", excecao.Message.ToLowerInvariant());

        pedidoRepository.Verify(
            r => r.InserirAsync(It.IsAny<Pedido>()),
            Times.Never);

        itemPedidoRepository.Verify(
            r => r.InserirAsync(It.IsAny<ItemPedido>()),
            Times.Never);

        jogoRepository.Verify(
            r => r.BaixarEstoqueAsync(
                It.IsAny<int>(),
                It.IsAny<int>()),
            Times.Never);
    }

}