
using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);

    Task<Usuario?> BuscarPorIdAsync(int id);

    Task<bool> ExisteAdministradorAsync(int idPerfilAdministrador);

    Task<int> InserirAsync(Usuario usuario);
}
