
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(
        IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestApiModel requisicao)
    {
        var usuario = await _autenticacaoService.AutenticarAsync(
            requisicao.Email,
            requisicao.Senha
        );

        if (usuario is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode =
                    ResponseCodes.CREDENCIAIS_INVALIDAS.Code,
                ErrorMessage =
                    ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage
            });
        }

        var principal = usuario.CriarPrincipal();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        var resposta = new UsuarioLogadoResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = principal.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty
        };

        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = resposta
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Ok(new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        });
    }

    [HttpGet("eu")]
    [Authorize]
    public IActionResult Eu()
    {
        var idUsuario = User.ObterIdUsuario();

        var nome = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var perfil = User.FindFirst(ClaimTypes.Role)?.Value;

        if (idUsuario is null ||
            string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(perfil))
        {
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode =
                    ResponseCodes.NAO_AUTENTICADO.Code,
                ErrorMessage =
                    ResponseCodes.NAO_AUTENTICADO.DefaultMessage
            });
        }

        var resposta = new UsuarioLogadoResponseApiModel
        {
            Id = idUsuario.Value,
            Nome = nome,
            Email = email,
            Perfil = perfil
        };

        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = resposta
        });
    }
}
