
using System.Net;

namespace RetroGames.Services;

public static class BusinessErrors
{
    public static readonly (
        int Code,
        string DefaultMessage,
        HttpStatusCode Status
    ) PLATAFORMA_NAO_ENCONTRADA =
        (100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly (
        int Code,
        string DefaultMessage,
        HttpStatusCode Status
    ) PLATAFORMA_NOME_DUPLICADO =
        (101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly (
        int Code,
        string DefaultMessage,
        HttpStatusCode Status
    ) PLATAFORMA_POSSUI_JOGOS =
        (102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly (
        int Code,
        string DefaultMessage,
        HttpStatusCode Status
    ) JOGO_NAO_ENCONTRADO =
        (103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly (
        int Code,
        string DefaultMessage,
        HttpStatusCode Status
    ) CONDICAO_NAO_ENCONTRADA =
        (104, "Condição de jogo não encontrada.", HttpStatusCode.NotFound);

    public static readonly (
        int Code,
        string DefaultMessage,
        HttpStatusCode Status
    ) GENERO_NAO_ENCONTRADO =
        (105, "Gênero não encontrado.", HttpStatusCode.NotFound);
}
