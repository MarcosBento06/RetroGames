using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;


namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _servico;

    public PlataformaController(IPlataformaService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _servico.ListarAsync();

        var dados = plataformas
            .Select(plataforma => plataforma.ParaResponse())
            .ToList();

        var resposta = new RetroGamesResponseApiModel<
            List<ListarPlataformaResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        };

        return Ok(resposta);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma = await _servico.BuscarPorIdAsync(id);

        var resposta = new RetroGamesResponseApiModel<
            ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = plataforma.ParaResponse()
        };

        return Ok(resposta);
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Inserir(
        [FromBody] InserirPlataformaRequestApiModel request)
    {
        var plataforma = request.ParaModel();

        var criada = await _servico.InserirAsync(plataforma);

        var resposta = new RetroGamesResponseApiModel<
            ListarPlataformaResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = criada.ParaResponse()
        };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = criada.Id },
            resposta
        );
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Alterar(
        int id,
        [FromBody] AlterarPlataformaRequestApiModel request)
    {
        var plataforma = request.ParaModel(id);

        await _servico.AlterarAsync(plataforma);

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Excluir(int id)
    {
        await _servico.ExcluirAsync(id);

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }
}
