
using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface ICondicaoJogoService
{
    Task<List<CondicaoJogo>> ListarAsync();
}
