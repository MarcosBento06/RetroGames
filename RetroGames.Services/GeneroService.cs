
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class GeneroService : IGeneroService
{
    private readonly IRepositorySet _repositorio;

    public GeneroService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Genero>> ListarAsync()
    {
        return await _repositorio.Genero.ListarAsync();
    }
}
