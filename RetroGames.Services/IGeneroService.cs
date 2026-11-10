
using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IGeneroService
{
    Task<List<Genero>> ListarAsync();
}
