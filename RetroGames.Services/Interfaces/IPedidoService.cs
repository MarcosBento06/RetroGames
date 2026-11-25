using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IPedidoService
{
    Task<int> FinalizarCompraAsync(IReadOnlyCollection<ItemPedido> itens);
    Task<List<PedidoMeusPedidosServiceModel>> ListarMeusPedidosAsync();
}