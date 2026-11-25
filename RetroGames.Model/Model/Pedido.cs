namespace RetroGames.Model.Model;

public class Pedido
{
    // Coluna: co_seq_pedido
    public int Id { get; set; }

    // Coluna: dt_pedido
    public DateTime DataPedido { get; set; }

    // Coluna: vl_total
    public decimal ValorTotal { get; set; }

    // Coluna: co_usuario
    public int IdUsuario { get; set; }

    // Coluna: co_situacao_pedido
    public int IdSituacaoPedido { get; set; }
}