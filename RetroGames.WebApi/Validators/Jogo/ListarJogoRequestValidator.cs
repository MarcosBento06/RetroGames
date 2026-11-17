
using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public class ListarJogoRequestValidator
    : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage("A página deve ser maior que zero.");

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 50)
            .WithMessage("O tamanho da página deve estar entre 1 e 50.");
    }
}
