
using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static (
        string? Busca,
        int? IdPlataforma,
        int? IdGenero,
        int Pagina,
        int TamanhoPagina
    ) ParaFiltro(this ListarJogoRequestApiModel request)
    {
        return (
            request.Busca,
            request.IdPlataforma,
            request.IdGenero,
            request.Pagina,
            request.TamanhoPagina
        );
    }

    public static ListarJogoResponseApiModel ParaResponseListagem(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.Nome)
                .ToList()
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaResponseDetalhe(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarPorIdJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Descricao = detalhe.Jogo.Descricao ?? string.Empty,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            IdPlataforma = detalhe.Jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.ParaResponse())
                .ToList()
        };
    }

    public static Jogo ParaModel(
        this InserirJogoRequestApiModel request)
    {
        return new Jogo
        {
            Nome = request.Nome,
            Descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? null
                : request.Descricao,
            AnoLancamento = request.AnoLancamento,
            Preco = request.Preco,
            Estoque = request.Estoque,
            IdPlataforma = request.IdPlataforma,
            IdCondicaoJogo = request.IdCondicaoJogo,
            Ativo = true
        };
    }

    public static Jogo ParaModel(
        this AlterarJogoRequestApiModel request,
        int id)
    {
        return new Jogo
        {
            Id = id,
            Nome = request.Nome,
            Descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? null
                : request.Descricao,
            AnoLancamento = request.AnoLancamento,
            Preco = request.Preco,
            Estoque = request.Estoque,
            IdPlataforma = request.IdPlataforma,
            IdCondicaoJogo = request.IdCondicaoJogo,
            Ativo = request.Ativo
        };
    }
}
