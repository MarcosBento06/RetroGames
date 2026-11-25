using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    public PedidoRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<int> InserirAsync(Pedido pedido)
    {
        const string sql = """
            INSERT INTO tb_pedido
            (
                dt_pedido,
                vl_total,
                co_usuario,
                co_situacao_pedido
            )
            VALUES
            (
                @dataPedido,
                @valorTotal,
                @idUsuario,
                @idSituacaoPedido
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@dataPedido", pedido.DataPedido);
        AdicionarParametro(comando, "@valorTotal", pedido.ValorTotal);
        AdicionarParametro(comando, "@idUsuario", pedido.IdUsuario);
        AdicionarParametro(
            comando, "@idSituacaoPedido", pedido.IdSituacaoPedido);

        await ExecutarGravacaoAsync(comando);

        return Convert.ToInt32(comando.LastInsertedId);
    }
}