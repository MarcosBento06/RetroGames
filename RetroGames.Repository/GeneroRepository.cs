
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_seq_genero,
                no_genero
            FROM tb_genero
            ORDER BY no_genero ASC;
            """;

        var generos = new List<Genero>();

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            var genero = new Genero
            {
                Id = leitor.GetInt32(
                    leitor.GetOrdinal("co_seq_genero")),

                Nome = leitor.GetString(
                    leitor.GetOrdinal("no_genero"))
            };

            generos.Add(genero);
        }

        return generos;
    }
}
