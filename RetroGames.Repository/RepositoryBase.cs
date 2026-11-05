
using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _configuracao;

    protected RepositoryBase(RepositoryConfig configuracao)
    {
        _configuracao = configuracao;
    }

    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(
            _configuracao.CadeiaConexao);

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    protected MySqlCommand CriarComando(
        MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected void AdicionarParametro(
        MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(
            nome, valor ?? DBNull.Value);
    }

    protected async Task<int> ExecutarGravacaoAsync(
        MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException erro) when (erro.Number == 1062)
        {
            throw new RegistroDuplicadoException(erro);
        }
    }
}
