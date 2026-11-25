namespace RetroGames.WebApi.ApiModels;

public class InserirPedidoRequestApiModel
{
    public List<ItemPedidoRequestApiModel> Itens { get; set; } = new();
}