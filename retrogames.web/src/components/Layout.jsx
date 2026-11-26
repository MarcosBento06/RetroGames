import { useState } from 'react'
import { Link, Outlet, useNavigate } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'
import { obterMensagemErro } from '../api/erros'
import Logo from './Logo'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    ehCliente,
    sair
  } = useAutenticacao()

  const navigate = useNavigate()

  const [saindo, setSaindo] = useState(false)
  const [erroSaida, setErroSaida] = useState('')

  async function realizarLogout() {
    setSaindo(true)
    setErroSaida('')

    try {
      await sair()
      navigate('/')
    } catch (erro) {
      setErroSaida(obterMensagemErro(erro))
    } finally {
      setSaindo(false)
    }
  }

  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand d-flex align-items-center gap-2" to="/">
            <Logo tamanho={36} />
            <span className="rg-marca-texto">RetroGames</span>
          </Link>

          <div className="navbar-nav">
            <Link className="nav-link" to="/">
              Catálogo
            </Link>
          </div>

          <ul className="navbar-nav ms-auto d-flex flex-row flex-wrap align-items-center gap-3">
            {carregando && (
              <li className="nav-item">
                <span className="navbar-text">Verificando sessão...</span>
              </li>
            )}

            {!carregando && !estaAutenticado && (
              <li className="nav-item">
                <Link className="nav-link" to="/login">
                  Entrar
                </Link>
              </li>
            )}

            {!carregando && estaAutenticado && (
              <>
                {ehCliente && (
                  <>
                    <li className="nav-item">
                      <Link className="nav-link" to="/carrinho">Carrinho</Link>
                    </li>
                    <li className="nav-item">
                      <Link className="nav-link" to="/meus-pedidos">Meus pedidos</Link>
                    </li>
                  </>
                )}

                {ehAdministrador && (
                  <li className="nav-item dropdown">
                    <button
                      type="button"
                      className="nav-link dropdown-toggle bg-transparent border-0"
                      data-bs-toggle="dropdown"
                      aria-expanded="false"
                    >
                      Administração
                    </button>

                    <ul className="dropdown-menu dropdown-menu-dark">
                      <li>
                        <Link className="dropdown-item" to="/admin/jogos">Jogos</Link>
                      </li>
                      <li>
                        <Link className="dropdown-item" to="/admin/plataformas">Plataformas</Link>
                      </li>
                    </ul>
                  </li>
                )}

                <li className="nav-item">
                  <span className="navbar-text">Olá, {usuario?.nome}</span>
                </li>

                <li className="nav-item">
                  <button
                    type="button"
                    className="btn btn-outline-light btn-sm"
                    onClick={realizarLogout}
                    disabled={saindo}
                  >
                    {saindo ? 'Saindo...' : 'Sair'}
                  </button>
                </li>
              </>
            )}
          </ul>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        {erroSaida && (
          <div className="alert alert-danger" role="alert">{erroSaida}</div>
        )}
        <Outlet />
      </main>

      <footer className="bg-dark text-white text-center py-3">
        <div className="container d-flex justify-content-center align-items-center gap-2">
          <Logo tamanho={24} />
          <span>RetroGames</span>
        </div>
      </footer>
    </div>
  )
}

export default Layout
