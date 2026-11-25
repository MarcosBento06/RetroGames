using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;

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
}