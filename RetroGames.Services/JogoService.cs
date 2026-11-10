
using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(
        List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return new List<JogoDetalheServiceModel>();
        }

        var idsJogos = jogos
            .Select(jogo => jogo.Id)
            .ToArray();

        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();

        var vinculos = await _repositorio.JogoGenero
            .ListarPorJogosAsync(idsJogos);

        var plataformasPorId = plataformas
            .ToDictionary(plataforma => plataforma.Id);

        var condicoesPorId = condicoes
            .ToDictionary(condicao => condicao.Id);

        var generosPorId = generos
            .ToDictionary(genero => genero.Id);

        var vinculosPorJogo = vinculos
            .ToLookup(vinculo => vinculo.IdJogo);

        return jogos.Select(jogo => new JogoDetalheServiceModel
        {
            Jogo = jogo,

            NomePlataforma = plataformasPorId[jogo.IdPlataforma].Nome,

            DescricaoCondicao = condicoesPorId[jogo.IdCondicaoJogo]
                .Descricao,

            Generos = vinculosPorJogo[jogo.Id]
                .Select(vinculo => generosPorId[vinculo.IdGenero])
                .OrderBy(genero => genero.Nome)
                .ToList()
        }).ToList();
    }

    private async Task ValidarReferenciasAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        var plataforma = await _repositorio.Plataforma
            .BuscarPorIdAsync(jogo.IdPlataforma);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();

        if (!condicoes.Any(condicao =>
            condicao.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(
                BusinessErrors.CONDICAO_NAO_ENCONTRADA);
        }

        var generos = await _repositorio.Genero.ListarAsync();

        var idsGenerosExistentes = generos
            .Select(genero => genero.Id)
            .ToHashSet();

        if (idsGeneros.Count == 0 ||
            idsGeneros.Any(id => !idsGenerosExistentes.Contains(id)))
        {
            throw new BusinessRuleException(
                BusinessErrors.GENERO_NAO_ENCONTRADO);
        }
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)>
        ListarAsync(
            string? busca = null,
            int? idPlataforma = null,
            int? idGenero = null,
            bool incluirInativos = false,
            int pagina = 1,
            int tamanhoPagina = 10)
    {
        IReadOnlyCollection<int>? idsJogos = null;

        if (idGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero
                .ListarTodosAsync();

            idsJogos = vinculos
                .Where(vinculo => vinculo.IdGenero == idGenero.Value)
                .Select(vinculo => vinculo.IdJogo)
                .Distinct()
                .ToArray();
        }

        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            busca,
            idPlataforma,
            idsJogos,
            incluirInativos,
            pagina,
            tamanhoPagina);

        var detalhes = await MontarDetalhesAsync(jogos);

        return (detalhes, total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });

        return detalhes[0];
    }

    public async Task<Jogo> InserirAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;

        var idsDistintos = idsGeneros.Distinct().ToArray();

        await ValidarReferenciasAsync(jogo, idsDistintos);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);

            await _repositorio.JogoGenero.InserirAsync(
                jogo.Id,
                idsDistintos);

            transactionScope.Complete();
        }

        return jogo;
    }

    public async Task AlterarAsync(
        Jogo jogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        var jogoExistente = await _repositorio.Jogo
            .BuscarPorIdAsync(jogo.Id);

        if (jogoExistente is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        jogo.Nome = jogo.Nome.Trim();

        var idsDistintos = idsGeneros.Distinct().ToArray();

        await ValidarReferenciasAsync(jogo, idsDistintos);

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using (var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorio.Jogo.AlterarAsync(jogo);

            await _repositorio.JogoGenero
                .RemoverPorJogoAsync(jogo.Id);

            await _repositorio.JogoGenero
                .InserirAsync(jogo.Id, idsDistintos);

            transactionScope.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);

        if (jogo is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.JOGO_NAO_ENCONTRADO);
        }

        await _repositorio.Jogo.DesativarAsync(id);
    }
}
