
import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'

import {
  jogoApi,
  plataformaApi,
  generoApi
} from '../../api/servicos'

import { obterMensagemErro } from '../../api/erros'
import CardJogo from '../../components/CardJogo'
import Paginacao from '../../components/Paginacao'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

const TAMANHO_PAGINA = 12

function Catalogo() {
  const [searchParams, setSearchParams] = useSearchParams()

  // Filtros obtidos da URL
  const busca = searchParams.get('busca') ?? ''
  const idPlataforma = searchParams.get('idPlataforma') ?? ''
  const idGenero = searchParams.get('idGenero') ?? ''

  const paginaInformada = Number(searchParams.get('pagina'))
  const pagina =
    Number.isInteger(paginaInformada) && paginaInformada > 0
      ? paginaInformada
      : 1

  // Estado dos filtros
  const [textoBusca, setTextoBusca] = useState(busca)
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [carregandoFiltros, setCarregandoFiltros] = useState(true)
  const [erroFiltros, setErroFiltros] = useState('')
  const [tentativasFiltros, setTentativasFiltros] = useState(0)

  // Estado da listagem
  const [resultado, setResultado] = useState(null)
  const [carregandoJogos, setCarregandoJogos] = useState(true)
  const [erroJogos, setErroJogos] = useState('')
  const [tentativasJogos, setTentativasJogos] = useState(0)

  // Sincroniza o campo de busca com a URL
  useEffect(() => {
    setTextoBusca(busca)
  }, [busca])

  // Carrega plataformas e gêneros
  useEffect(() => {
    let ativo = true

    async function carregarFiltros() {
      setCarregandoFiltros(true)
      setErroFiltros('')

      try {
        const [dadosPlataformas, dadosGeneros] =
          await Promise.all([
            plataformaApi.listar(),
            generoApi.listar()
          ])

        if (ativo) {
          setPlataformas(dadosPlataformas)
          setGeneros(dadosGeneros)
        }
      } catch (erro) {
        if (ativo) {
          setErroFiltros(obterMensagemErro(erro))
        }
      } finally {
        if (ativo) {
          setCarregandoFiltros(false)
        }
      }
    }

    carregarFiltros()

    return () => {
      ativo = false
    }
  }, [tentativasFiltros])

  // Recarrega os jogos quando os filtros da URL mudam
  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      setCarregandoJogos(true)
      setErroJogos('')

      try {
        const dados = await jogoApi.listar({
          busca: busca || undefined,
          idPlataforma: idPlataforma || undefined,
          idGenero: idGenero || undefined,
          pagina,
          tamanhoPagina: TAMANHO_PAGINA
        })

        if (ativo) {
          setResultado(dados)
        }
      } catch (erro) {
        if (ativo) {
          setErroJogos(obterMensagemErro(erro))
        }
      } finally {
        if (ativo) {
          setCarregandoJogos(false)
        }
      }
    }

    carregarJogos()

    return () => {
      ativo = false
    }
  }, [
    busca,
    idPlataforma,
    idGenero,
    pagina,
    tentativasJogos
  ])

  // Atualiza um filtro e retorna à primeira página
  function alterarFiltro(nome, valor) {
    const parametros = new URLSearchParams(searchParams)

    if (valor) {
      parametros.set(nome, valor)
    } else {
      parametros.delete(nome)
    }

    parametros.delete('pagina')
    setSearchParams(parametros)
  }

  function pesquisar(evento) {
    evento.preventDefault()
    alterarFiltro('busca', textoBusca.trim())
  }

  function mudarPagina(novaPagina) {
    const parametros = new URLSearchParams(searchParams)

    if (novaPagina === 1) {
      parametros.delete('pagina')
    } else {
      parametros.set('pagina', String(novaPagina))
    }

    setSearchParams(parametros)
  }

  const jogos = resultado?.itens ?? []
  const totalPaginas = resultado?.totalPaginas ?? 0

  return (
    <div>
      <h1 className="mb-4">Catálogo de Jogos</h1>

      <form onSubmit={pesquisar} className="row g-3 mb-4">
        <div className="col-md-6">
          <label htmlFor="busca" className="form-label">
            Buscar jogo
          </label>

          <input
            id="busca"
            type="search"
            className="form-control"
            placeholder="Digite o nome do jogo"
            value={textoBusca}
            onChange={(evento) =>
              setTextoBusca(evento.target.value)
            }
          />
        </div>

        <div className="col-md-3">
          <label htmlFor="plataforma" className="form-label">
            Plataforma
          </label>

          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            disabled={carregandoFiltros}
            onChange={(evento) =>
              alterarFiltro('idPlataforma', evento.target.value)
            }
          >
            <option value="">Todas</option>

            {plataformas.map((plataforma) => (
              <option key={plataforma.id} value={plataforma.id}>
                {plataforma.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-md-3">
          <label htmlFor="genero" className="form-label">
            Gênero
          </label>

          <select
            id="genero"
            className="form-select"
            value={idGenero}
            disabled={carregandoFiltros}
            onChange={(evento) =>
              alterarFiltro('idGenero', evento.target.value)
            }
          >
            <option value="">Todos</option>

            {generos.map((genero) => (
              <option key={genero.id} value={genero.id}>
                {genero.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12">
          <button type="submit" className="btn btn-primary">
            Buscar
          </button>
        </div>
      </form>

      {erroFiltros && (
        <MensagemErro
          mensagem={erroFiltros}
          onTentarNovamente={() =>
            setTentativasFiltros((valor) => valor + 1)
          }
        />
      )}

      {carregandoFiltros && (
        <p className="text-muted">Carregando filtros...</p>
      )}

      {carregandoJogos ? (
        <Carregando />
      ) : erroJogos ? (
        <MensagemErro
          mensagem={erroJogos}
          onTentarNovamente={() =>
            setTentativasJogos((valor) => valor + 1)
          }
        />
      ) : jogos.length === 0 ? (
        <EstadoVazio mensagem="Nenhum jogo encontrado." />
      ) : (
        <>
          <div className="row g-4">
            {jogos.map((jogo) => (
              <div
                key={jogo.id}
                className="col-12 col-md-6 col-lg-4"
              >
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>

          <div className="mt-4">
            <Paginacao
              pagina={pagina}
              totalPaginas={totalPaginas}
              aoMudar={mudarPagina}
            />
          </div>
        </>
      )}
    </div>
  )
}

export default Catalogo
