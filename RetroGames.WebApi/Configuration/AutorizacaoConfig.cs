
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(
        this IServiceCollection services)
    {
        services.AddAuthorization(opcoes =>
        {
            opcoes.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            opcoes.AddPolicy(
                Politicas.Administrador,
                politica => politica
                    .RequireAuthenticatedUser()
                    .RequireRole(Perfis.Administrador)
            );
        });

        return services;
    }
}
