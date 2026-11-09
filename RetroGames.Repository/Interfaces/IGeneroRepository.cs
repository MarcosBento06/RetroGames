
using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarAsync();
}
