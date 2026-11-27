import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

import Paginacao from '../../components/Paginacao'

function Jogos() {
  const [parametros, definirParametros] = useSearchParams()

  const busca = parametros.get('busca') ?? ''
  const pagina = Math.max(
    1,
    Number(parametros.get('pagina')) || 1
  )

  const [jogos, setJogos] = useState([])
  const [totalPaginas, setTotalPaginas] = useState(1)

  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [erroAcao, setErroAcao] = useState('')

  const [desativandoId, setDesativandoId] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      try {
        const dados = await jogoApi.listar({
          busca,
          pagina,
          tamanhoPagina: 10,
          incluirInativos: true
        })

        if (ativo) {
          setJogos(dados.itens)
          setTotalPaginas(dados.totalPaginas)
          setErro('')
        }
      } catch (erroRequisicao) {
        if (ativo) {
          setErro(obterMensagemErro(erroRequisicao))
        }
      } finally {
        if (ativo) {
          setCarregando(false)
        }
      }
    }

    carregarJogos()

    return () => {
      ativo = false
    }
  }, [busca, pagina, tentativa])

  function recarregarLista() {
    setCarregando(true)
    setErro('')
    setTentativa((anterior) => anterior + 1)
  }

  function realizarBusca(evento) {
    evento.preventDefault()

    const dadosFormulario = new FormData(evento.currentTarget)

    const textoBusca = String(
      dadosFormulario.get('busca') ?? ''
    ).trim()

    const novosParametros = new URLSearchParams()

    if (textoBusca) {
      novosParametros.set('busca', textoBusca)
    }

    definirParametros(novosParametros)
    recarregarLista()
  }

  function mudarPagina(novaPagina) {
    if (novaPagina === pagina) {
      return
    }

    const novosParametros = new URLSearchParams(parametros)

    novosParametros.set('pagina', String(novaPagina))

    definirParametros(novosParametros)
    setCarregando(true)
  }

  async function desativarJogo(jogo) {
    const confirmou = window.confirm(
      `Deseja realmente desativar o jogo "${jogo.nome}"?`
    )

    if (!confirmou) {
      return
    }

    setDesativandoId(jogo.id)
    setErroAcao('')

    try {
      await jogoApi.desativar(jogo.id)

      recarregarLista()
    } catch (erroRequisicao) {
      setErroAcao(obterMensagemErro(erroRequisicao))
    } finally {
      setDesativandoId(null)
    }
  }

  return (
    <section className="rg-admin">
      <div className="rg-admin__cabecalho">
        <h1 className="mb-0">
          Administração de Jogos
        </h1>

        <Link
          to="/admin/jogos/novo"
          className="btn btn-primary"
        >
          Novo jogo
        </Link>
      </div>

      <form onSubmit={realizarBusca} className="rg-admin__busca">
        <div className="input-group">
          <input
            key={busca}
            type="search"
            name="busca"
            className="form-control"
            placeholder="Buscar jogos..."
            defaultValue={busca}
          />

          <button type="submit" className="btn btn-primary">
            Buscar
          </button>
        </div>
      </form>

      {erroAcao && (
        <div className="alert alert-danger" role="alert">
          {erroAcao}
        </div>
      )}

      {carregando ? (
        <Carregando texto="Carregando jogos..." />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={recarregarLista}
        />
      ) : jogos.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado.">
          Não existem jogos para os filtros informados.
        </EstadoVazio>
      ) : (
        <>
          <div className="table-responsive">
            <table className="table table-striped table-hover">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Ano</th>
                  <th>Situação</th>
                  <th>Ações</th>
                </tr>
              </thead>

              <tbody>
                {jogos.map((jogo) => (
                  <tr key={jogo.id}>
                    <td>{jogo.nome}</td>
                    <td>{jogo.anoLancamento}</td>

                    <td>
                      {jogo.ativo ? (
                        <span className="badge bg-success">
                          Ativo
                        </span>
                      ) : (
                        <span className="badge bg-secondary">
                          Inativo
                        </span>
                      )}
                    </td>

                    <td>
                      <div className="d-flex gap-2">
                        <Link
                          to={`/admin/jogos/${jogo.id}/editar`}
                          className="btn btn-sm btn-outline-primary"
                        >
                          Editar
                        </Link>

                        <button
                          type="button"
                          className="btn btn-sm btn-outline-danger"
                          onClick={() => desativarJogo(jogo)}
                          disabled={
                            !jogo.ativo ||
                            desativandoId !== null
                          }
                        >
                          {desativandoId === jogo.id
                            ? 'Desativando...'
                            : 'Desativar'}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <Paginacao
            pagina={pagina}
            totalPaginas={totalPaginas}
            aoMudar={mudarPagina}
          />
        </>
      )}
    </section>
  )
}

export default Jogos