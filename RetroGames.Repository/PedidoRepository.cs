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

    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        const string sql = """
            SELECT
                co_seq_pedido,
                dt_pedido,
                vl_total,
                co_usuario,
                co_situacao_pedido
            FROM tb_pedido
            WHERE co_usuario = @idUsuario
            ORDER BY
                dt_pedido DESC,
                co_seq_pedido DESC;
            """;

        var pedidos = new List<Pedido>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idUsuario", idUsuario);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var pedido = new Pedido
            {
                Id = leitor.GetInt32(
                    leitor.GetOrdinal("co_seq_pedido")),

                DataPedido = leitor.GetDateTime(
                    leitor.GetOrdinal("dt_pedido")),

                ValorTotal = leitor.GetDecimal(
                    leitor.GetOrdinal("vl_total")),

                IdUsuario = leitor.GetInt32(
                    leitor.GetOrdinal("co_usuario")),

                IdSituacaoPedido = leitor.GetInt32(
                    leitor.GetOrdinal("co_situacao_pedido"))
            };

            pedidos.Add(pedido);
        }

        return pedidos;
    }
}