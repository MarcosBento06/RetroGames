using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{
    public ItemPedidoRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<int> InserirAsync(ItemPedido itemPedido)
    {
        const string sql = """
            INSERT INTO tb_item_pedido
                (qt_item, vl_unitario, co_pedido, co_jogo)
            VALUES
                (@quantidade, @valorUnitario, @idPedido, @idJogo);
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@quantidade", itemPedido.Quantidade);
        AdicionarParametro(comando, "@valorUnitario", itemPedido.ValorUnitario);
        AdicionarParametro(comando, "@idPedido", itemPedido.IdPedido);
        AdicionarParametro(comando, "@idJogo", itemPedido.IdJogo);

        await ExecutarGravacaoAsync(comando);

        return Convert.ToInt32(comando.LastInsertedId);
    }

    public async Task<List<ItemPedido>> ListarPorPedidoAsync(int idPedido)
    {
        const string sql = """
            SELECT
                co_seq_item_pedido,
                qt_item,
                vl_unitario,
                co_pedido,
                co_jogo
            FROM tb_item_pedido
            WHERE co_pedido = @idPedido
            ORDER BY co_seq_item_pedido ASC;
            """;

        var itens = new List<ItemPedido>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idPedido", idPedido);

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var itemPedido = new ItemPedido
            {
                Id = leitor.GetInt32("co_seq_item_pedido"),
                Quantidade = leitor.GetInt32("qt_item"),
                ValorUnitario = leitor.GetDecimal("vl_unitario"),
                IdPedido = leitor.GetInt32("co_pedido"),
                IdJogo = leitor.GetInt32("co_jogo")
            };

            itens.Add(itemPedido);
        }

        return itens;
    }
}