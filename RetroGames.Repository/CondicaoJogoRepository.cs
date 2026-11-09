
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_condicao_jogo,
                ds_condicao_jogo
            FROM tb_condicao_jogo
            ORDER BY co_condicao_jogo ASC;
            """;

        var condicoes = new List<CondicaoJogo>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var condicao = new CondicaoJogo
            {
                Id = leitor.GetInt32(
                    leitor.GetOrdinal("co_condicao_jogo")),

                Descricao = leitor.GetString(
                    leitor.GetOrdinal("ds_condicao_jogo"))
            };

            condicoes.Add(condicao);
        }

        return condicoes;
    }
}
