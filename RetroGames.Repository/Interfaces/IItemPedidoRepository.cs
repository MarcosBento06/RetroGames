using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IItemPedidoRepository{
    Task<int> InserirAsync(ItemPedido itemPedido);

    Task<List<ItemPedido>> ListarPorPedidoAsync(int idPedido);
}