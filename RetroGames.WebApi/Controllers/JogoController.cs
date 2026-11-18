
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _servico;

    public JogoController(IJogoService servico)
    {
        _servico = servico;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();

        var (jogos, total) = await _servico.ListarAsync(
            busca: filtro.Busca,
            idPlataforma: filtro.IdPlataforma,
            idGenero: filtro.IdGenero,
            incluirInativos: false,
            pagina: filtro.Pagina,
            tamanhoPagina: filtro.TamanhoPagina
        );

        var itens = jogos
            .Select(jogo => jogo.ParaResponseListagem())
            .ToList();

        var pagina = new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = itens,
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling(
                (double)total / filtro.TamanhoPagina
            )
        };

        var resposta = new RetroGamesResponseApiModel<
            PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = pagina
        };

        return Ok(resposta);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var jogo = await _servico.BuscarPorIdAsync(id);

        var dados = jogo.ParaResponseDetalhe();

        var resposta = new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        };

        return Ok(resposta);
    }


    [HttpPost]
    public async Task<IActionResult> Inserir(
        [FromBody] InserirJogoRequestApiModel request)
    {
        var jogo = request.ParaModel();

        var criado = await _servico.InserirAsync(
            jogo,
            request.IdsGeneros
        );

        var detalhe = await _servico.BuscarPorIdAsync(criado.Id);

        var resposta = new RetroGamesResponseApiModel<
            ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaResponseDetalhe()
        };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = criado.Id },
            resposta
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Alterar(
        int id,
        [FromBody] AlterarJogoRequestApiModel request)
    {
        var jogo = request.ParaModel(id);

        await _servico.AlterarAsync(
            jogo,
            request.IdsGeneros
        );

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _servico.DesativarAsync(id);

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }


}
