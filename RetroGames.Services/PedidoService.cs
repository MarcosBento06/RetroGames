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
        var jogos = await ObterJogosDisponiveisAsync(itens);

        ValidarEstoque(itens, jogos);

        throw new NotImplementedException(
            "A finalização será implementada nas próximas tarefas da C04.");
    }

    private async Task<List<Jogo>> ObterJogosDisponiveisAsync(
    IReadOnlyCollection<ItemPedido> itens)
    {
        var jogos = new List<Jogo>();

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

            jogos.Add(jogo);
        }

        return jogos;
    }

    private void ValidarEstoque(
    IReadOnlyCollection<ItemPedido> itens,
    IReadOnlyCollection<Jogo> jogos)
    {
        var jogosPorId = jogos.ToDictionary(jogo => jogo.Id);

        var quantidadesPorJogo = itens
            .GroupBy(item => item.IdJogo)
            .Select(grupo => new
            {
                IdJogo = grupo.Key,
                QuantidadeSolicitada = grupo.Sum(item => item.Quantidade)
            });

        foreach (var item in quantidadesPorJogo)
        {
            var jogo = jogosPorId[item.IdJogo];

            if (jogo.Estoque < item.QuantidadeSolicitada)
            {
                throw new BusinessRuleException(
                    BusinessErrors.ESTOQUE_INSUFICIENTE);
            }
        }
    }
}