
namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception excecaoInterna)
        : base("O banco recusou um registro duplicado.", excecaoInterna)
    {
    }
}
