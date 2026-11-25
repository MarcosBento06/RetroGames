using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;
using System.Transactions;

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

        PreencherPrecosDosItens(itens, jogos);

        var valorTotal = CalcularValorTotal(itens);

        var pedido = CriarPedido(valorTotal);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            pedido.Id = await _repositorios.Pedido
                .InserirAsync(pedido);

            foreach (var item in itens)
            {
                item.IdPedido = pedido.Id;

                await _repositorios.ItemPedido
                    .InserirAsync(item);
            }

            foreach (var grupo in itens.GroupBy(item => item.IdJogo))
            {
                var quantidade = grupo.Sum(item => item.Quantidade);

                var linhasAfetadas =
                    await _repositorios.Jogo.BaixarEstoqueAsync(
                        grupo.Key,
                        quantidade);

                if (linhasAfetadas == 0)
                {
                    throw new BusinessRuleException(
                        BusinessErrors.ESTOQUE_INSUFICIENTE);
                }
            }

            transactionScope.Complete();
        }

        return pedido.Id;
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

    private static void PreencherPrecosDosItens(
    IReadOnlyCollection<ItemPedido> itens,
    IReadOnlyCollection<Jogo> jogos)
    {
        var jogosPorId = jogos.ToDictionary(jogo => jogo.Id);

        foreach (var item in itens)
        {
            item.ValorUnitario = jogosPorId[item.IdJogo].Preco;
        }
    }

    private static decimal CalcularValorTotal(
    IReadOnlyCollection<ItemPedido> itens)
    {
        return itens.Sum(item =>
            item.Quantidade * item.ValorUnitario);
    }

    private Pedido CriarPedido(decimal valorTotal)
    {
        return new Pedido
        {
            DataPedido = DateTime.UtcNow,
            ValorTotal = valorTotal,
            IdUsuario = _usuarioAtual.IdUsuario
                ?? throw new InvalidOperationException(
                    "Não foi possível identificar o usuário autenticado."),
            IdSituacaoPedido = 1
        };
    }
}