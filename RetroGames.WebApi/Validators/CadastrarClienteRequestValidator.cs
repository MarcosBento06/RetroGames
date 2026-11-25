
using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class CadastrarClienteRequestValidator
    : AbstractValidator<CadastrarClienteRequestApiModel>
{
    public CadastrarClienteRequestValidator()
    {
        RuleFor(cliente => cliente.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome.");

        RuleFor(cliente => cliente.Email)
            .NotEmpty()
            .WithMessage("Informe o e-mail.")
            .EmailAddress()
            .WithMessage("Informe um e-mail válido.");

        RuleFor(cliente => cliente.Senha)
            .NotEmpty()
            .WithMessage("Informe a senha.");
    }
}
