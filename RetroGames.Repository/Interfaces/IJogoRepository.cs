
using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoRepository
{
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyCollection<int>? idsJogos = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 10);

    Task<Jogo?> BuscarPorIdAsync(int id);

    Task<int> InserirAsync(Jogo jogo);

    Task AlterarAsync(Jogo jogo);

    Task DesativarAsync(int id);
}
