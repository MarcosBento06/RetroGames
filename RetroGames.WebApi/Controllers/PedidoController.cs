using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using Microsoft.AspNetCore.Authorization;
using RetroGames.Services.ServiceModels;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _servico;

    public PedidoController(IPedidoService servico)
    {
        _servico = servico;
    }

    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Inserir(
        [FromBody] InserirPedidoRequestApiModel request)
    {
        var itens = request.Itens
            .Select(item => new ItemPedido
            {
                IdJogo = item.IdJogo,
                Quantidade = item.Quantidade
            })
            .ToList();

        var idPedido = await _servico.FinalizarCompraAsync(itens);

        var resposta = new RetroGamesResponseApiModel<int>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = idPedido
        };

        return StatusCode(
            StatusCodes.Status201Created,
            resposta);
    }
     
    [Authorize]
    [HttpGet("meus")]
    public async Task<IActionResult> ListarMeus()
    {
        var pedidos = await _servico.ListarMeusPedidosAsync();

        var resposta = new RetroGamesResponseApiModel<List<PedidoMeusPedidosServiceModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = pedidos
        };

        return Ok(resposta);
    }
}