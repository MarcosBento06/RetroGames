
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services;

public class PlataformaService
{
    private readonly IRepositorySet _repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        return await _repositorio.Plataforma.ListarAsync();
    }


    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        return plataforma;
    }


    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        bool nomeExiste = await _repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome);

        if (nomeExiste)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        try
        {
            plataforma.Id = await _repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        return plataforma;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        var plataformaExistente = await _repositorio.Plataforma.BuscarPorIdAsync(plataforma.Id);

        if (plataformaExistente is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        plataforma.Nome = plataforma.Nome.Trim();

        bool nomeExiste = await _repositorio.Plataforma
            .ExisteComNomeAsync(plataforma.Nome, plataforma.Id);

        if (nomeExiste)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        try
        {
            await _repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma
            .BuscarPorIdAsync(id);

        if (plataforma is null)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        }

        bool possuiJogos = await _repositorio.Plataforma
            .PossuiJogosAsync(id);

        if (possuiJogos)
        {
            throw new BusinessRuleException(
                BusinessErrors.PLATAFORMA_POSSUI_JOGOS);
        }

        await _repositorio.Plataforma.ExcluirAsync(id);
    }

}
