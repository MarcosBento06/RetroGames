
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class RepositorySet : IRepositorySet
{
    public IPlataformaRepository Plataforma { get; }

    public IJogoRepository Jogo { get; }

    public IJogoGeneroRepository JogoGenero { get; }

    public IGeneroRepository Genero { get; }

    public ICondicaoJogoRepository CondicaoJogo { get; }

    public RepositorySet(RepositoryConfig configuracao)
    {
        Plataforma = new PlataformaRepository(configuracao);

        Jogo = new JogoRepository(configuracao);

        JogoGenero = new JogoGeneroRepository(configuracao);

        Genero = new GeneroRepository(configuracao);

        CondicaoJogo = new CondicaoJogoRepository(configuracao);
    }
}
