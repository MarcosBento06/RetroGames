
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var cadeiaConexao = builder.Configuration
    .GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(cadeiaConexao))
{
    throw new InvalidOperationException(
        "Connection string não encontrada. Configure ConnectionStrings__Default no arquivo .env.");
}

builder.Services.AddRepositorios(cadeiaConexao);
builder.Services.AddServicosDeNegocio();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers(opcoes =>
{
    opcoes.Filters.Add<FluentValidationFilter>();
})
.ConfigureApiBehaviorOptions(opcoes =>
{
    opcoes.InvalidModelStateResponseFactory = contexto =>
    {
        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
            ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage
        };

        return new BadRequestObjectResult(resposta);
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

// Garantir a existência do administrador inicial
var emailAdministrador = app.Configuration["AdminInicial:Email"];
var senhaAdministrador = app.Configuration["AdminInicial:Senha"];

if (string.IsNullOrWhiteSpace(emailAdministrador) ||
    string.IsNullOrWhiteSpace(senhaAdministrador))
{
    throw new InvalidOperationException(
        "Credenciais do administrador inicial não configuradas no .env.");
}

using (var escopo = app.Services.CreateScope())
{
    var autenticacaoService = escopo.ServiceProvider
        .GetRequiredService<IAutenticacaoService>();

    await autenticacaoService.GarantirAdministradorInicialAsync(
        emailAdministrador,
        senhaAdministrador
    );
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint(
            "/openapi/v1.json",
            "RetroGames API v1");
    });
}

app.MapControllers();

app.Run();
