
import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'

import { useAutenticacao } from '../../hooks/useAutenticacao'
import { obterMensagemErro } from '../../api/erros'

function Login() {
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState('')

  const { entrar } = useAutenticacao()

  const navigate = useNavigate()
  const [searchParams] = useSearchParams()

  const voltarPara = searchParams.get('voltarPara')

  async function realizarLogin(evento) {
    evento.preventDefault()

    setEnviando(true)
    setErro('')

    try {
      await entrar(email, senha)

      const destino =
        voltarPara?.startsWith('/') &&
        !voltarPara.startsWith('//') &&
        !voltarPara.includes('\\')
          ? voltarPara
          : '/'

      navigate(destino, { replace: true })
    } catch (erroRequisicao) {
      setErro(obterMensagemErro(erroRequisicao))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-md-6 col-lg-4">
        <h1 className="text-center mb-4">
          Entrar no RetroGames
        </h1>

        <form onSubmit={realizarLogin}>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">
              E-mail
            </label>

            <input
              id="email"
              type="email"
              className="form-control"
              value={email}
              onChange={(evento) =>
                setEmail(evento.target.value)
              }
              required
              disabled={enviando}
            />
          </div>

          <div className="mb-3">
            <label htmlFor="senha" className="form-label">
              Senha
            </label>

            <input
              id="senha"
              type="password"
              className="form-control"
              value={senha}
              onChange={(evento) =>
                setSenha(evento.target.value)
              }
              required
              disabled={enviando}
            />
          </div>

          {erro && (
            <div className="alert alert-danger" role="alert">
              {erro}
            </div>
          )}

          <button
            type="submit"
            className="btn btn-primary w-100"
            disabled={enviando}
          >
            {enviando ? 'Entrando...' : 'Entrar'}
          </button>
        </form>
      </div>
    </div>
  )
}

export default Login
