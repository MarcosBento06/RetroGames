import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  jogoApi,
  plataformaApi,
  generoApi,
  condicaoJogoApi
} from '../../api/servicos'
import {
  obterMensagemErro,
  obterErrosDosCampos
} from '../../api/erros'
import {
  Carregando,
  MensagemErro
} from '../../components/MensagensEstado'

const formularioInicial = {
  nome: '',
  descricao: '',
  anoLancamento: '',
  preco: '',
  estoque: '',
  idPlataforma: '',
  idCondicaoJogo: '',
  idsGeneros: []
}

function FormularioJogo() {
  const { id } = useParams()
  const navigate = useNavigate()

  const emEdicao = Boolean(id)

  const [formulario, setFormulario] = useState(formularioInicial)

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [condicoes, setCondicoes] = useState([])

  const [carregando, setCarregando] = useState(true)
  const [erroCarregamento, setErroCarregamento] = useState('')
  const [tentativa, setTentativa] = useState(0)

  const [salvando, setSalvando] = useState(false)
  const [erroFormulario, setErroFormulario] = useState('')
  const [errosCampos, setErrosCampos] = useState({})

  useEffect(() => {
    let ativo = true

    async function carregarDados() {
      try {
        const [
          listaPlataformas,
          listaGeneros,
          listaCondicoes,
          jogo
        ] = await Promise.all([
          plataformaApi.listar(),
          generoApi.listar(),
          condicaoJogoApi.listar(),
          emEdicao
            ? jogoApi.buscarPorId(id)
            : Promise.resolve(null)
        ])

        if (!ativo) return

        setPlataformas(listaPlataformas ?? [])
        setGeneros(listaGeneros ?? [])
        setCondicoes(listaCondicoes ?? [])

        if (jogo) {
          setFormulario({
            nome: jogo.nome ?? '',
            descricao: jogo.descricao ?? '',
            anoLancamento: String(jogo.anoLancamento ?? ''),
            preco: String(jogo.preco ?? ''),
            estoque: String(jogo.estoque ?? ''),
            idPlataforma: String(jogo.idPlataforma ?? ''),
            idCondicaoJogo: String(jogo.idCondicaoJogo ?? ''),
            idsGeneros: (jogo.generos ?? []).map(
              (genero) => Number(genero.id)
            )
          })
        }

        setErroCarregamento('')
      } catch (erroRequisicao) {
        if (ativo) {
          setErroCarregamento(
            obterMensagemErro(erroRequisicao)
          )
        }
      } finally {
        if (ativo) {
          setCarregando(false)
        }
      }
    }

    carregarDados()

    return () => {
      ativo = false
    }
  }, [id, emEdicao, tentativa])

  function tentarNovamente() {
    setCarregando(true)
    setErroCarregamento('')
    setTentativa((anterior) => anterior + 1)
  }

  function atualizarCampo(evento) {
    const { name, value } = evento.target

    setFormulario((anterior) => ({
      ...anterior,
      [name]: value
    }))

    setErrosCampos((anterior) => ({
      ...anterior,
      [name]: ''
    }))
  }

  function alterarGenero(idGenero, marcado) {
    setFormulario((anterior) => ({
      ...anterior,
      idsGeneros: marcado
        ? [...anterior.idsGeneros, idGenero]
        : anterior.idsGeneros.filter(
            (idAtual) => idAtual !== idGenero
          )
    }))

    setErrosCampos((anterior) => ({
      ...anterior,
      idsGeneros: ''
    }))
  }

  async function salvarJogo(evento) {
    evento.preventDefault()

    setSalvando(true)
    setErroFormulario('')
    setErrosCampos({})

    const dados = {
      nome: formulario.nome,
      descricao: formulario.descricao,
      anoLancamento: Number(formulario.anoLancamento),
      preco: Number(formulario.preco),
      estoque: Number(formulario.estoque),
      idPlataforma: Number(formulario.idPlataforma),
      idCondicaoJogo: Number(formulario.idCondicaoJogo),
      idsGeneros: formulario.idsGeneros.map(Number)
    }

    try {
      if (emEdicao) {
        await jogoApi.alterar(id, dados)
      } else {
        await jogoApi.inserir(dados)
      }

      navigate('/admin/jogos')
    } catch (erroRequisicao) {
      setErroFormulario(
        obterMensagemErro(erroRequisicao)
      )

      setErrosCampos(
        obterErrosDosCampos(erroRequisicao)
      )
    } finally {
      setSalvando(false)
    }
  }

  if (carregando) {
    return <Carregando texto="Carregando formulário..." />
  }

  if (erroCarregamento) {
    return (
      <MensagemErro
        mensagem={erroCarregamento}
        aoTentarNovamente={tentarNovamente}
      />
    )
  }

  return (
    <section className="rg-admin">
      <h1 className="mb-4">
        {emEdicao ? 'Editar Jogo' : 'Cadastrar Jogo'}
      </h1>

      <form onSubmit={salvarJogo} noValidate className="rg-admin__formulario">
        <div className="row g-3">

          <div className="col-12">
            <label htmlFor="nome" className="form-label">
              Nome
            </label>

            <input
              id="nome"
              name="nome"
              type="text"
              className={`form-control ${
                errosCampos.nome ? 'is-invalid' : ''
              }`}
              value={formulario.nome}
              onChange={atualizarCampo}
              disabled={salvando}
            />

            {errosCampos.nome && (
              <div className="invalid-feedback">
                {errosCampos.nome}
              </div>
            )}
          </div>

          <div className="col-12">
            <label htmlFor="descricao" className="form-label">
              Descrição
            </label>

            <textarea
              id="descricao"
              name="descricao"
              rows={3}
              className={`form-control ${
                errosCampos.descricao ? 'is-invalid' : ''
              }`}
              value={formulario.descricao}
              onChange={atualizarCampo}
              disabled={salvando}
            />

            {errosCampos.descricao && (
              <div className="invalid-feedback">
                {errosCampos.descricao}
              </div>
            )}
          </div>

          <div className="col-12 col-md-4">
            <label
              htmlFor="anoLancamento"
              className="form-label"
            >
              Ano de lançamento
            </label>

            <input
              id="anoLancamento"
              name="anoLancamento"
              type="number"
              className={`form-control ${
                errosCampos.anoLancamento ? 'is-invalid' : ''
              }`}
              value={formulario.anoLancamento}
              onChange={atualizarCampo}
              disabled={salvando}
            />

            {errosCampos.anoLancamento && (
              <div className="invalid-feedback">
                {errosCampos.anoLancamento}
              </div>
            )}
          </div>

          <div className="col-12 col-md-4">
            <label htmlFor="preco" className="form-label">
              Preço
            </label>

            <input
              id="preco"
              name="preco"
              type="number"
              step="0.01"
              className={`form-control ${
                errosCampos.preco ? 'is-invalid' : ''
              }`}
              value={formulario.preco}
              onChange={atualizarCampo}
              disabled={salvando}
            />

            {errosCampos.preco && (
              <div className="invalid-feedback">
                {errosCampos.preco}
              </div>
            )}
          </div>

          <div className="col-12 col-md-4">
            <label htmlFor="estoque" className="form-label">
              Estoque
            </label>

            <input
              id="estoque"
              name="estoque"
              type="number"
              step="1"
              className={`form-control ${
                errosCampos.estoque ? 'is-invalid' : ''
              }`}
              value={formulario.estoque}
              onChange={atualizarCampo}
              disabled={salvando}
            />

            {errosCampos.estoque && (
              <div className="invalid-feedback">
                {errosCampos.estoque}
              </div>
            )}
          </div>

          <div className="col-12 col-md-6">
            <label
              htmlFor="idPlataforma"
              className="form-label"
            >
              Plataforma
            </label>

            <select
              id="idPlataforma"
              name="idPlataforma"
              className={`form-select ${
                errosCampos.idPlataforma ? 'is-invalid' : ''
              }`}
              value={formulario.idPlataforma}
              onChange={atualizarCampo}
              disabled={salvando}
            >
              <option value="">Selecione a plataforma</option>

              {plataformas.map((plataforma) => (
                <option
                  key={plataforma.id}
                  value={plataforma.id}
                >
                  {plataforma.nome}
                </option>
              ))}
            </select>

            {errosCampos.idPlataforma && (
              <div className="invalid-feedback">
                {errosCampos.idPlataforma}
              </div>
            )}
          </div>

          <div className="col-12 col-md-6">
            <label
              htmlFor="idCondicaoJogo"
              className="form-label"
            >
              Condição do jogo
            </label>

            <select
              id="idCondicaoJogo"
              name="idCondicaoJogo"
              className={`form-select ${
                errosCampos.idCondicaoJogo ? 'is-invalid' : ''
              }`}
              value={formulario.idCondicaoJogo}
              onChange={atualizarCampo}
              disabled={salvando}
            >
              <option value="">Selecione a condição</option>

              {condicoes.map((condicao) => (
                <option
                  key={condicao.id}
                  value={condicao.id}
                >
                  {condicao.descricao}
                </option>
              ))}
            </select>

            {errosCampos.idCondicaoJogo && (
              <div className="invalid-feedback">
                {errosCampos.idCondicaoJogo}
              </div>
            )}
          </div>

          <div className="col-12">
            <fieldset disabled={salvando}>
              <legend className="fs-6">
                Gêneros
              </legend>

              <div className="d-flex flex-wrap gap-3">
                {generos.map((genero) => (
                  <div
                    className="form-check"
                    key={genero.id}
                  >
                    <input
                      id={`genero-${genero.id}`}
                      type="checkbox"
                      className="form-check-input"
                      checked={formulario.idsGeneros.includes(
                        Number(genero.id)
                      )}
                      onChange={(evento) =>
                        alterarGenero(
                          Number(genero.id),
                          evento.target.checked
                        )
                      }
                    />

                    <label
                      htmlFor={`genero-${genero.id}`}
                      className="form-check-label"
                    >
                      {genero.nome}
                    </label>
                  </div>
                ))}
              </div>
            </fieldset>

            {errosCampos.idsGeneros && (
              <div className="text-danger small mt-2">
                {errosCampos.idsGeneros}
              </div>
            )}
          </div>

        </div>

        {erroFormulario && (
          <div
            className="alert alert-danger mt-4"
            role="alert"
          >
            {erroFormulario}
          </div>
        )}

        <div className="d-flex gap-2 mt-4">
          <button
            type="submit"
            className="btn btn-primary"
            disabled={salvando}
          >
            {salvando
              ? 'Salvando...'
              : emEdicao
                ? 'Salvar alterações'
                : 'Cadastrar jogo'}
          </button>

          <Link
            to="/admin/jogos"
            className="btn btn-secondary"
            aria-disabled={salvando}
            onClick={(evento) => {
              if (salvando) evento.preventDefault()
            }}
          >
            Cancelar
          </Link>
        </div>
      </form>
    </section>
  )
}

export default FormularioJogo