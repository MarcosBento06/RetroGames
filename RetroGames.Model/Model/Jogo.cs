
namespace RetroGames.Model.Model;

public class Jogo
{
    // Coluna: co_seq_jogo
    public int Id { get; set; }

    // Coluna: no_jogo
    public string Nome { get; set; } = string.Empty;

    // Coluna: ds_jogo
    public string? Descricao { get; set; }

    // Coluna: nu_ano_lancamento
    public int AnoLancamento { get; set; }

    // Coluna: vl_preco
    public decimal Preco { get; set; }

    // Coluna: qt_estoque
    public int Estoque { get; set; }

    // Coluna: st_ativo
    public bool Ativo { get; set; }

    // Coluna: dt_cadastro
    public DateTime DataCadastro { get; set; }

    // Coluna: co_plataforma
    public int IdPlataforma { get; set; }

    // Coluna: co_condicao_jogo
    public int IdCondicaoJogo { get; set; }
}
