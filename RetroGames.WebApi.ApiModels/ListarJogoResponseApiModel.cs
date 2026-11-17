
namespace RetroGames.WebApi.ApiModels;

public class ListarJogoResponseApiModel
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int AnoLancamento { get; set; }

    public decimal Preco { get; set; }

    public int Estoque { get; set; }

    public bool Ativo { get; set; }

    public string NomePlataforma { get; set; } = string.Empty;

    public string DescricaoCondicao { get; set; } = string.Empty;

    public List<string> Generos { get; set; } = new();
}
