import { useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
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
    <section className="rg-autenticacao">
      <div className="rg-autenticacao__painel">
        <header className="rg-autenticacao__cabecalho">
          <span className="rg-autenticacao__etiqueta">
            BEM-VINDO DE VOLTA
          </span>

          <h1>Entrar no RetroGames</h1>

          <p>
            Acesse sua conta para acompanhar suas compras e
            aproveitar nossa coleção de jogos.
          </p>
        </header>

        <form onSubmit={realizarLogin}>
          <div className="mb-3">
            <label htmlFor="email" className="form-label">
              E-mail
            </label>

            <input
              id="email"
              type="email"
              className="form-control rg-campo"
              value={email}
              onChange={(evento) => setEmail(evento.target.value)}
              autoComplete="email"
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
              className="form-control rg-campo"
              value={senha}
              onChange={(evento) => setSenha(evento.target.value)}
              autoComplete="current-password"
              required
              disabled={enviando}
            />
          </div>

          {erro && (
            <div
              className="alert alert-danger rg-formulario__erro"
              role="alert"
              aria-live="assertive"
            >
              {erro}
            </div>
          )}

          <button
            type="submit"
            className="btn btn-primary w-100 rg-autenticacao__botao"
            disabled={enviando}
          >
            {enviando ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="rg-autenticacao__rodape">
          <span>Ainda não possui uma conta?</span>

          <Link
            to={
              voltarPara
                ? `/cadastro?voltarPara=${encodeURIComponent(voltarPara)}`
                : '/cadastro'
            }
          >
            Criar conta
          </Link>
        </div>
      </div>
    </section>
  )
}

export default Login