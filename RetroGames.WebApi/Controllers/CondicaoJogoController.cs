
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _servico;

    public CondicaoJogoController(ICondicaoJogoService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var condicoes = await _servico.ListarAsync();

        var dados = condicoes
            .Select(condicao => condicao.ParaResponse())
            .ToList();

        var resposta = new RetroGamesResponseApiModel<
            List<ListarCondicaoJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        };

        return Ok(resposta);
    }
}
