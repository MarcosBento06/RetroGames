namespace RetroGames.Model.Model;

public class ItemPedido
{
    // Coluna: co_seq_item_pedido
    public int Id { get; set; }

    // Coluna: qt_item
    public int Quantidade { get; set; }

    // Coluna: vl_unitario
    public decimal ValorUnitario { get; set; }

    // Coluna: co_pedido
    public int IdPedido { get; set; }

    // Coluna: co_jogo
    public int IdJogo { get; set; }
}