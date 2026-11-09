
using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface ICondicaoJogoRepository
{
    Task<List<CondicaoJogo>> ListarAsync();
}
