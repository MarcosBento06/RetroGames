
namespace RetroGames.Model.Model;

public class Plataforma
{
    // Coluna: co_seq_plataforma
    public int Id { get; set; }

    // Coluna: no_plataforma
    public string Nome { get; set; } = string.Empty;

    // Coluna: no_fabricante
    public string Fabricante { get; set; } = string.Empty;

    // Coluna: nu_ano_lancamento
    public int AnoLancamento { get; set; }
}
