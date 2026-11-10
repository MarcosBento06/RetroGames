
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class CondicaoJogoService : ICondicaoJogoService
{
    private readonly IRepositorySet _repositorio;

    public CondicaoJogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        return await _repositorio.CondicaoJogo.ListarAsync();
    }
}
