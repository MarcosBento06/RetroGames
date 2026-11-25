using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IPedidoService
{
    Task<int> FinalizarCompraAsync(IReadOnlyCollection<ItemPedido> itens);
    Task<List<Pedido>> ListarMeusPedidosAsync();
}