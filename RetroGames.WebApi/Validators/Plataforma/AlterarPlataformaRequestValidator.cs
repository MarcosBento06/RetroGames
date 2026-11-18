
using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Plataforma;

public class AlterarPlataformaRequestValidator
    : AbstractValidator<AlterarPlataformaRequestApiModel>
{
    public AlterarPlataformaRequestValidator()
    {
        RuleFor(plataforma => plataforma.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.")
            .MaximumLength(80)
            .WithMessage("O nome deve ter no máximo 80 caracteres.");

        RuleFor(plataforma => plataforma.Fabricante)
            .NotEmpty()
            .WithMessage("Informe o fabricante.")
            .MaximumLength(80)
            .WithMessage("O fabricante deve ter no máximo 80 caracteres.");

        RuleFor(plataforma => plataforma.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");
    }
}
