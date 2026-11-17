
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter, IOrderedFilter
{
    public int Order => -2100;

    public async Task OnActionExecutionAsync(
        ActionExecutingContext contexto,
        ActionExecutionDelegate proximo)
    {
        var parametroCorpo = contexto.ActionDescriptor.Parameters
            .FirstOrDefault(parametro =>
                parametro.BindingInfo?.BindingSource?
                    .CanAcceptDataFrom(BindingSource.Body) == true);

        var corpoAusente = contexto.HttpContext.Features
            .Get<IHttpRequestBodyDetectionFeature>()?.CanHaveBody == false;

        if (parametroCorpo is not null && corpoAusente)
        {
            contexto.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel
                {
                    ResponseCode = ResponseCodes.MODEL_NULL.Code,
                    ErrorMessage = ResponseCodes.MODEL_NULL.DefaultMessage
                });

            return;
        }

        if (!contexto.ModelState.IsValid)
        {
            await proximo();
            return;
        }

        var erros = new Dictionary<string, List<string>>();

        foreach (var argumento in contexto.ActionArguments.Values)
        {
            if (argumento is null)
                continue;

            var tipoValidador = typeof(IValidator<>)
                .MakeGenericType(argumento.GetType());

            var validador = contexto.HttpContext.RequestServices
                .GetService(tipoValidador) as IValidator;

            if (validador is null)
                continue;

            var resultado = await validador.ValidateAsync(
                new ValidationContext<object>(argumento),
                contexto.HttpContext.RequestAborted);

            foreach (var falha in resultado.Errors)
            {
                var nomeCampo = string.Join(".",
                    falha.PropertyName
                        .Split('.')
                        .Select(nome =>
                            JsonNamingPolicy.CamelCase.ConvertName(nome)));

                if (!erros.TryGetValue(nomeCampo, out var mensagens))
                {
                    mensagens = new List<string>();
                    erros[nomeCampo] = mensagens;
                }

                mensagens.Add(falha.ErrorMessage);
            }
        }

        if (erros.Count > 0)
        {
            contexto.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel<
                    Dictionary<string, List<string>>>
                {
                    ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                    ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                    Data = erros
                });

            return;
        }

        await proximo();
    }
}
