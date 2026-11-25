namespace RetroGames.Services.ServiceModels;

public class PedidoMeusPedidosServiceModel
{
    public int Id { get; set; }

    public DateTime DataPedido { get; set; }

    public decimal ValorTotal { get; set; }

    public string Situacao { get; set; } = string.Empty;

    public List<ItemPedidoMeusPedidosServiceModel> Itens { get; set; } = new();
}