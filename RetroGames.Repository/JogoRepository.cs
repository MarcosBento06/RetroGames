
using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyCollection<int>? idsJogos = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        var ids = idsJogos?.ToArray();

        string filtroIds = "";

        if (ids is not null)
        {
            if (ids.Length == 0)
            {
                filtroIds = "AND 1 = 0";
            }
            else
            {
                var parametrosIds = ids
                    .Select((id, indice) => $"@idJogo{indice}");

                filtroIds = $"""
                    AND co_seq_jogo IN ({string.Join(", ", parametrosIds)})
                    """;
            }
        }

        var filtros = $"""
            WHERE (@busca IS NULL
                   OR no_jogo LIKE CONCAT('%', @busca, '%'))
              AND (@idPlataforma IS NULL
                   OR co_plataforma = @idPlataforma)
              AND (@incluirInativos = 1 OR st_ativo = 1)
              {filtroIds}
            """;

        var sqlListagem = $"""
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            {filtros}
            ORDER BY no_jogo ASC, co_seq_jogo ASC
            LIMIT @limite OFFSET @deslocamento;
            """;

        var sqlTotal = $"""
            SELECT COUNT(*)
            FROM tb_jogo
            {filtros};
            """;

        void AdicionarFiltros(MySqlCommand comando)
        {
            AdicionarParametro(comando, "@busca", busca);
            AdicionarParametro(comando, "@idPlataforma", idPlataforma);
            AdicionarParametro(comando, "@incluirInativos", incluirInativos);

            if (ids is not null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    AdicionarParametro(comando, $"@idJogo{i}", ids[i]);
                }
            }
        }

        var jogos = new List<Jogo>();

        await using var conexao = await AbrirConexaoAsync();

        await using (var comando = CriarComando(conexao, sqlListagem))
        {
            AdicionarFiltros(comando);

            AdicionarParametro(comando, "@limite", tamanhoPagina);
            AdicionarParametro(
                comando, "@deslocamento", (pagina - 1) * tamanhoPagina);

            await using var leitor = await comando.ExecuteReaderAsync();

            while (await leitor.ReadAsync())
            {
                var jogo = new Jogo
                {
                    Id = leitor.GetInt32(
                        leitor.GetOrdinal("co_seq_jogo")),

                    Nome = leitor.GetString(
                        leitor.GetOrdinal("no_jogo")),

                    Descricao = leitor.IsDBNull(
                        leitor.GetOrdinal("ds_jogo"))
                        ? null
                        : leitor.GetString(
                            leitor.GetOrdinal("ds_jogo")),

                    AnoLancamento = leitor.GetInt32(
                        leitor.GetOrdinal("nu_ano_lancamento")),

                    Preco = leitor.GetDecimal(
                        leitor.GetOrdinal("vl_preco")),

                    Estoque = leitor.GetInt32(
                        leitor.GetOrdinal("qt_estoque")),

                    Ativo = leitor.GetBoolean(
                        leitor.GetOrdinal("st_ativo")),

                    DataCadastro = DateTime.SpecifyKind(
                        leitor.GetDateTime(
                            leitor.GetOrdinal("dt_cadastro")),
                        DateTimeKind.Utc),

                    IdPlataforma = leitor.GetInt32(
                        leitor.GetOrdinal("co_plataforma")),

                    IdCondicaoJogo = leitor.GetInt32(
                        leitor.GetOrdinal("co_condicao_jogo"))
                };

                jogos.Add(jogo);
            }
        }

        await using var comandoTotal = CriarComando(conexao, sqlTotal);

        AdicionarFiltros(comandoTotal);

        var resultadoTotal = await comandoTotal.ExecuteScalarAsync();
        int total = Convert.ToInt32(resultadoTotal);

        return (jogos, total);
    }


    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_jogo,
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
        {
            return null;
        }

        return new Jogo
        {
            Id = leitor.GetInt32(
                leitor.GetOrdinal("co_seq_jogo")),

            Nome = leitor.GetString(
                leitor.GetOrdinal("no_jogo")),

            Descricao = leitor.IsDBNull(
                leitor.GetOrdinal("ds_jogo"))
                ? null
                : leitor.GetString(
                    leitor.GetOrdinal("ds_jogo")),

            AnoLancamento = leitor.GetInt32(
                leitor.GetOrdinal("nu_ano_lancamento")),

            Preco = leitor.GetDecimal(
                leitor.GetOrdinal("vl_preco")),

            Estoque = leitor.GetInt32(
                leitor.GetOrdinal("qt_estoque")),

            Ativo = leitor.GetBoolean(
                leitor.GetOrdinal("st_ativo")),

            DataCadastro = DateTime.SpecifyKind(
                leitor.GetDateTime(
                    leitor.GetOrdinal("dt_cadastro")),
                DateTimeKind.Utc),

            IdPlataforma = leitor.GetInt32(
                leitor.GetOrdinal("co_plataforma")),

            IdCondicaoJogo = leitor.GetInt32(
                leitor.GetOrdinal("co_condicao_jogo"))
        };
    }


    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
            (
                no_jogo,
                ds_jogo,
                nu_ano_lancamento,
                vl_preco,
                qt_estoque,
                st_ativo,
                dt_cadastro,
                co_plataforma,
                co_condicao_jogo
            )
            VALUES
            (
                @nome,
                @descricao,
                @anoLancamento,
                @preco,
                @estoque,
                @ativo,
                UTC_TIMESTAMP(),
                @idPlataforma,
                @idCondicaoJogo
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);

        return Convert.ToInt32(comando.LastInsertedId);
    }


    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET
                no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicaoJogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }


    public async Task DesativarAsync(int id)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = 0
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }


}
