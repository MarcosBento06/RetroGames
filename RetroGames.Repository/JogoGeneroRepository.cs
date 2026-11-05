
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;
public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = """
            SELECT
                co_jogo,
                co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
            """;

        var vinculos = new List<JogoGenero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var vinculo = new JogoGenero
            {
                IdJogo = leitor.GetInt32(
                    leitor.GetOrdinal("co_jogo")),

                IdGenero = leitor.GetInt32(
                    leitor.GetOrdinal("co_genero"))
            };

            vinculos.Add(vinculo);
        }

        return vinculos;
    }


    public async Task<List<JogoGenero>> ListarPorJogosAsync(
        IReadOnlyCollection<int> idsJogos)
    {
        var vinculos = new List<JogoGenero>();

        if (idsJogos.Count == 0)
        {
            return vinculos;
        }

        var ids = idsJogos.ToArray();

        var nomesParametros = ids
            .Select((id, indice) => $"@idJogo{indice}")
            .ToArray();

        var sql = $"""
            SELECT
                co_jogo,
                co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", nomesParametros)})
            ORDER BY co_jogo, co_genero;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        for (int i = 0; i < ids.Length; i++)
        {
            AdicionarParametro(comando, $"@idJogo{i}", ids[i]);
        }

        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var vinculo = new JogoGenero
            {
                IdJogo = leitor.GetInt32(
                    leitor.GetOrdinal("co_jogo")),

                IdGenero = leitor.GetInt32(
                    leitor.GetOrdinal("co_genero"))
            };

            vinculos.Add(vinculo);
        }

        return vinculos;
    }


    public async Task InserirAsync(
        int idJogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        var ids = idsGeneros.ToArray();

        var valores = ids
            .Select((_, indice) => $"(@idJogo, @idGenero{indice})");

        var sql = $"""
            INSERT INTO tb_jogo_genero
                (co_jogo, co_genero)
            VALUES
                {string.Join(", ", valores)};
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        for (int i = 0; i < ids.Length; i++)
        {
            AdicionarParametro(
                comando,
                $"@idGenero{i}",
                ids[i]);
        }

        await ExecutarGravacaoAsync(comando);
    }


    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        await ExecutarGravacaoAsync(comando);
    }


}
