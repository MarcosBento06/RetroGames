
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(
        this IServiceCollection services,
        string cadeiaConexao)
    {
        services.AddSingleton(new RepositoryConfig
        {
            CadeiaConexao = cadeiaConexao
        });

        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(
        this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();

        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();    

        return services;
    }
}
