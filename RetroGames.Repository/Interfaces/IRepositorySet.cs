
namespace RetroGames.Repository.Interfaces;

public interface IRepositorySet
{
    IPlataformaRepository Plataforma { get; }

    IJogoRepository Jogo { get; }

    IJogoGeneroRepository JogoGenero { get; }

    IGeneroRepository Genero { get; }

    ICondicaoJogoRepository CondicaoJogo { get; }
}
