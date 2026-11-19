
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RetroGames.Model.Constantes;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Usuario =>
        _httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado =>
        Usuario?.Identity?.IsAuthenticated == true;

    public int? IdUsuario =>
        EstaAutenticado
            ? Usuario?.ObterIdUsuario()
            : null;

    public bool EhAdministrador =>
        EstaAutenticado &&
        Usuario?.IsInRole(Perfis.Administrador) == true;
}
