
using System.Net;

namespace RetroGames.Services;

public class BusinessRuleException : Exception
{
    public (
        int Code,
        string DefaultMessage,
        HttpStatusCode Status
    ) Erro { get; }

    public BusinessRuleException(
        (
            int Code,
            string DefaultMessage,
            HttpStatusCode Status
        ) erro
    ) : base(erro.DefaultMessage)
    {
        Erro = erro;
    }
}
