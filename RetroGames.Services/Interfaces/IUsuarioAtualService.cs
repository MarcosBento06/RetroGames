
namespace RetroGames.Services.Interfaces;

public interface IUsuarioAtualService
{
    bool EstaAutenticado { get; }

    int? IdUsuario { get; }

    bool EhAdministrador { get; }
}
