using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class SituacaoPedidoRepository : RepositoryBase, ISituacaoPedidoRepository
{
    public SituacaoPedidoRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<List<SituacaoPedido>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_situacao_pedido,
                ds_situacao_pedido
            FROM tb_situacao_pedido
            ORDER BY co_situacao_pedido ASC;
            """;

        var situacoes = new List<SituacaoPedido>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var situacao = new SituacaoPedido
            {
                Id = leitor.GetInt32(
                    leitor.GetOrdinal("co_situacao_pedido")),

                Descricao = leitor.GetString(
                    leitor.GetOrdinal("ds_situacao_pedido"))
            };

            situacoes.Add(situacao);
        }

        return situacoes;
    }
}