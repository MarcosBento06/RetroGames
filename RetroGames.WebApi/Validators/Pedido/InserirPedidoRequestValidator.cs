using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Pedido;

public class InserirPedidoRequestValidator
    : AbstractValidator<InserirPedidoRequestApiModel>
{
    public InserirPedidoRequestValidator()
    {
        RuleFor(pedido => pedido.Itens)
            .NotEmpty()
            .WithMessage("Informe pelo menos um item no pedido.");

        RuleForEach(pedido => pedido.Itens)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.IdJogo)
                    .GreaterThan(0)
                    .WithMessage("Informe um jogo válido.");

                item.RuleFor(i => i.Quantidade)
                    .InclusiveBetween(1, 10)
                    .WithMessage(
                        "A quantidade de cada item deve estar entre 1 e 10.");
            });
    }
}