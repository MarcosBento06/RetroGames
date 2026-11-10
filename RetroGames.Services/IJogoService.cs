
using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 10);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<Jogo> InserirAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros);

    Task AlterarAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros);

    Task DesativarAsync(int id);
}
