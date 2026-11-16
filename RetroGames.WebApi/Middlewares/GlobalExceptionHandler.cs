
using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var resposta = new RetroGamesResponseApiModel();

        if (exception is BusinessRuleException erroNegocio)
        {
            httpContext.Response.StatusCode =
                (int)erroNegocio.Erro.Status;

            resposta.ResponseCode = erroNegocio.Erro.Code;
            resposta.ErrorMessage = erroNegocio.Message;
        }
        else
        {
            _logger.LogError(
                exception,
                "Erro inesperado ao processar a requisição.");

            httpContext.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            resposta.ResponseCode = ResponseCodes.ERRO;

            resposta.ErrorMessage =
                "Ocorreu um erro interno no servidor.";
        }

        await httpContext.Response.WriteAsJsonAsync(
            resposta,
            cancellationToken);

        return true;
    }
}
