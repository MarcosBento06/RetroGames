
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class RepositorySet : IRepositorySet
{
    public IPlataformaRepository Plataforma { get; }

    public IJogoRepository Jogo { get; }

    public IJogoGeneroRepository JogoGenero { get; }

    public IGeneroRepository Genero { get; }

    public ICondicaoJogoRepository CondicaoJogo { get; }

    public IUsuarioRepository Usuario { get; }

    public RepositorySet(
        IPlataformaRepository plataforma,
        IJogoRepository jogo,
        IJogoGeneroRepository jogoGenero,
        IGeneroRepository genero,
        ICondicaoJogoRepository condicaoJogo,
        IUsuarioRepository usuario)
    {
        Plataforma = plataforma;
        Jogo = jogo;
        JogoGenero = jogoGenero;
        Genero = genero;
        CondicaoJogo = condicaoJogo;
        Usuario = usuario;
    }
}
