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

    public async Task<int> FinalizarCompraAsync(
    IReadOnlyCollection<ItemPedido> itens)
    {
        await ValidarJogosDisponiveisAsync(itens);

        throw new NotImplementedException(
            "A finalização será implementada nas próximas tarefas da C04.");
    }

    private async Task ValidarJogosDisponiveisAsync(
    IReadOnlyCollection<ItemPedido> itens)
    {
        var idsJogos = itens
            .Select(item => item.IdJogo)
            .Distinct();

        foreach (var idJogo in idsJogos)
        {
            var jogo = await _repositorios.Jogo
                .BuscarPorIdAsync(idJogo);

            if (jogo is null || !jogo.Ativo)
            {
                throw new BusinessRuleException(
                    BusinessErrors.JOGO_INDISPONIVEL);
            }
        }
    }
}