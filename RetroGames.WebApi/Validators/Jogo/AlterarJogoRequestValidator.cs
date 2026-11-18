
using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public class AlterarJogoRequestValidator
    : AbstractValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator()
    {
        RuleFor(jogo => jogo.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome do jogo.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(jogo => jogo.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter no máximo 1000 caracteres.");

        RuleFor(jogo => jogo.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");

        RuleFor(jogo => jogo.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, false)
            .WithMessage("O preço deve ter no máximo 2 casas decimais e 10 dígitos no total.");

        RuleFor(jogo => jogo.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");

        RuleFor(jogo => jogo.IdPlataforma)
            .GreaterThan(0)
            .WithMessage("Informe a plataforma.");

        RuleFor(jogo => jogo.IdCondicaoJogo)
            .GreaterThan(0)
            .WithMessage("Informe a condição do jogo.");

        RuleFor(jogo => jogo.IdsGeneros)
            .NotEmpty()
            .WithMessage("Informe pelo menos um gênero.");
    }
}
