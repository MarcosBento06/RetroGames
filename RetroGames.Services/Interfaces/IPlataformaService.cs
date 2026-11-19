
using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IPlataformaService
{
    Task<List<Plataforma>> ListarAsync();

    Task<Plataforma> BuscarPorIdAsync(int id);

    Task<Plataforma> InserirAsync(Plataforma plataforma);

    Task AlterarAsync(Plataforma plataforma);

    Task ExcluirAsync(int id);
}
