using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorySet _repositorios;
    private readonly IUsuarioAtualService _usuarioAtual;

    public PedidoService(
        IRepositorySet repositorios,
        IUsuarioAtualService usuarioAtual)
    {
        _repositorios = repositorios;
        _usuarioAtual = usuarioAtual;
    }

    public Task<int> FinalizarCompraAsync(
        IReadOnlyCollection<ItemPedido> itens)
    {
        throw new NotImplementedException(
            "A finalização será implementada nas próximas tarefas da C04.");
    }
}