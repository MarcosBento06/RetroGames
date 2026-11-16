
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

// Carrega as variáveis do arquivo .env
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Obtém a connection string do MySQL
var cadeiaConexao = builder.Configuration
    .GetConnectionString("Default");

// Verifica se a connection string foi configurada
if (string.IsNullOrWhiteSpace(cadeiaConexao))
{
    throw new InvalidOperationException(
        "Connection string não encontrada. " +
        "Configure ConnectionStrings__Default no arquivo .env.");
}

// Registra os Repositories e Services
builder.Services.AddRepositorios(cadeiaConexao);
builder.Services.AddServicosDeNegocio();

// Registra os Controllers e o OpenAPI
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseExceptionHandler();

// Disponibiliza o Swagger apenas em desenvolvimento
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

// Habilita as rotas dos Controllers
app.MapControllers();

// Inicia o servidor
app.Run();
