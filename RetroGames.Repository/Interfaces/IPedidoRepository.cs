using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPedidoRepository
{
    Task<int> InserirAsync(Pedido pedido);

    Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario);
}