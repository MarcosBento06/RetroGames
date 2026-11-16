
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _servico;

    public GeneroController(IGeneroService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var generos = await _servico.ListarAsync();

        var dados = generos
            .Select(genero => genero.ParaResponse())
            .ToList();

        var resposta = new RetroGamesResponseApiModel<
            List<ListarGeneroResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        };

        return Ok(resposta);
    }
}
