
import { useEffect, useState } from 'react'

import { plataformaApi } from '../../api/servicos'
import {
  obterMensagemErro,
  obterErrosDosCampos
} from '../../api/erros'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

const formularioInicial = {
  nome: '',
  fabricante: '',
  anoLancamento: ''
}

function Plataforma() {
  // Estados da listagem
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  // Estados do formulário
  const [formulario, setFormulario] = useState(formularioInicial)
  const [idEmEdicao, setIdEmEdicao] = useState(null)
  const [salvando, setSalvando] = useState(false)
  const [erroFormulario, setErroFormulario] = useState('')
  const [errosCampos, setErrosCampos] = useState({})
  const [excluindoId, setExcluindoId] = useState(null)
  const [erroExclusao, setErroExclusao] = useState('')

  useEffect(() => {
    let ativo = true

    async function carregarPlataformas() {
      try {
        const dados = await plataformaApi.listar()

        if (ativo) {
          setPlataformas(dados)
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

    carregarPlataformas()

    return () => {
      ativo = false
    }
  }, [tentativa])

  function recarregarLista() {
    setCarregando(true)
    setErro('')
    setTentativa((valorAtual) => valorAtual + 1)
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

  function limparFormulario() {
    setFormulario(formularioInicial)
    setIdEmEdicao(null)
    setErroFormulario('')
    setErrosCampos({})
  }

  function iniciarEdicao(plataforma) {
    setIdEmEdicao(plataforma.id)

    setFormulario({
      nome: plataforma.nome ?? '',
      fabricante: plataforma.fabricante ?? '',
      anoLancamento: String(plataforma.anoLancamento ?? '')
    })

    setErroFormulario('')
    setErrosCampos({})
  }

  async function salvarPlataforma(evento) {
    evento.preventDefault()

    setSalvando(true)
    setErroFormulario('')
    setErrosCampos({})

    const dados = {
      nome: formulario.nome,
      fabricante: formulario.fabricante,
      anoLancamento: Number(formulario.anoLancamento)
    }

    try {
      if (idEmEdicao !== null) {
        await plataformaApi.alterar(idEmEdicao, dados)
      } else {
        await plataformaApi.inserir(dados)
      }

      limparFormulario()
      recarregarLista()
    } catch (erroRequisicao) {
      setErroFormulario(obterMensagemErro(erroRequisicao))
      setErrosCampos(obterErrosDosCampos(erroRequisicao))
    } finally {
      setSalvando(false)
    }
  }


    async function excluirPlataforma(plataforma) {
        const confirmou = window.confirm(
            `Deseja realmente excluir a plataforma "${plataforma.nome}"?`
        )

        if (!confirmou) {
            return
        }

        setExcluindoId(plataforma.id)
        setErroExclusao('')

        try {
            await plataformaApi.excluir(plataforma.id)

            if (idEmEdicao === plataforma.id) {
            limparFormulario()
            }

            recarregarLista()
        } catch (erroRequisicao) {
            setErroExclusao(obterMensagemErro(erroRequisicao))
        } finally {
            setExcluindoId(null)
        }
    }


  return (
    <div>
      <h1 className="mb-4">
        Administração de Plataformas
      </h1>

      {/* Formulário de inclusão e alteração */}
      <div className="card mb-4">
        <div className="card-body">
          <h2 className="h5 mb-3">
            {idEmEdicao !== null
              ? 'Editar Plataforma'
              : 'Cadastrar Plataforma'}
          </h2>

          <form onSubmit={salvarPlataforma} noValidate>
            <div className="row g-3">
              <div className="col-12 col-md-4">
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

              <div className="col-12 col-md-4">
                <label htmlFor="fabricante" className="form-label">
                  Fabricante
                </label>

                <input
                  id="fabricante"
                  name="fabricante"
                  type="text"
                  className={`form-control ${
                    errosCampos.fabricante ? 'is-invalid' : ''
                  }`}
                  value={formulario.fabricante}
                  onChange={atualizarCampo}
                  disabled={salvando}
                />

                {errosCampos.fabricante && (
                  <div className="invalid-feedback">
                    {errosCampos.fabricante}
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
            </div>

            {erroFormulario && (
              <div
                className="alert alert-danger mt-3"
                role="alert"
              >
                {erroFormulario}
              </div>
            )}

            <div className="d-flex gap-2 mt-3">
              <button
                type="submit"
                className="btn btn-primary"
                disabled={salvando}
              >
                {salvando
                  ? 'Salvando...'
                  : idEmEdicao !== null
                    ? 'Salvar alterações'
                    : 'Cadastrar'}
              </button>

              {idEmEdicao !== null && (
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={limparFormulario}
                  disabled={salvando}
                >
                  Cancelar
                </button>
              )}
            </div>
          </form>
        </div>
      </div>

      {/* Listagem de plataformas */}
      <h2 className="h5 mb-3">
        Plataformas cadastradas
      </h2>

      {erroExclusao && (
        <div className="alert alert-danger" role="alert">
            {erroExclusao}
        </div>
      )}

      {carregando ? (
        <Carregando texto="Carregando plataformas..." />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={recarregarLista}
        />
      ) : plataformas.length === 0 ? (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada.">
          As plataformas cadastradas aparecerão aqui.
        </EstadoVazio>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Fabricante</th>
                <th>Ano de lançamento</th>
                <th>Ações</th>
              </tr>
            </thead>

            <tbody>
              {plataformas.map((plataforma) => (
                <tr key={plataforma.id}>
                  <td>{plataforma.nome}</td>
                  <td>{plataforma.fabricante}</td>
                  <td>{plataforma.anoLancamento}</td>
                  
                  <td>
                  <div className="d-flex gap-2">
                    <button
                    type="button"
                    className="btn btn-sm btn-outline-primary"
                    onClick={() => iniciarEdicao(plataforma)}
                    disabled={salvando || excluindoId !== null}
                    >
                    Editar
                    </button>

                    <button
                    type="button"
                    className="btn btn-sm btn-outline-danger"
                    onClick={() => excluirPlataforma(plataforma)}
                    disabled={salvando || excluindoId !== null}
                    >
                    {excluindoId === plataforma.id
                        ? 'Excluindo...'
                        : 'Excluir'}
                    </button>
                  </div>
                  </td>

                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

export default Plataforma
