import { useState } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'
import { useCarrinho } from '../hooks/useCarrinho'
import { obterMensagemErro } from '../api/erros'
import Logo from './Logo'

function IconeCarrinho() {
  return (
    <svg
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      aria-hidden="true"
      focusable="false"
    >
      <circle cx="9" cy="20" r="1.5" />
      <circle cx="18" cy="20" r="1.5" />
      <path d="M2 3h3l2.6 11.2a1 1 0 0 0 1 .8h9.2a1 1 0 0 0 1-.8L21 7H6" />
    </svg>
  )
}

// O número mostra a soma das quantidades; dados de sessão não entram no carrinho.
function LinkCarrinho({ quantidade, somenteIcone = false, aoNavegar }) {
  const descricao = `Carrinho, ${quantidade} ${quantidade === 1 ? 'item' : 'itens'}`

  return (
    <NavLink
      to="/carrinho"
      onClick={aoNavegar}
      className={`nav-link rg-link-carrinho ${somenteIcone ? 'rg-link-carrinho--icone' : ''}`}
      aria-label={descricao}
    >
      <IconeCarrinho />
      {!somenteIcone && <span>Carrinho</span>}
      <span className="rg-contador-carrinho" aria-hidden="true">
        {quantidade}
      </span>
    </NavLink>
  )
}

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    ehCliente,
    sair
  } = useAutenticacao()
  const { itens } = useCarrinho()
  const quantidadeCarrinho = itens.reduce(
    (soma, item) => soma + item.quantidade,
    0
  )

  const navigate = useNavigate()
  const [menuAberto, setMenuAberto] = useState(false)
  const [saindo, setSaindo] = useState(false)
  const [erroSaida, setErroSaida] = useState('')

  function fecharMenu() {
    setMenuAberto(false)
  }

  async function realizarLogout() {
    fecharMenu()
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
      <a href="#conteudo-principal" className="rg-pular-conteudo">
        Pular para o conteúdo
      </a>

      <nav
        className="navbar navbar-expand-lg navbar-dark rg-nav"
        aria-label="Navegação principal"
        onKeyDown={evento => {
          if (evento.key === 'Escape') fecharMenu()
        }}
      >
        <div className="container rg-nav__container">
          <Link
            className="navbar-brand rg-nav__marca"
            to="/"
            onClick={fecharMenu}
          >
            <Logo tamanho={36} />
            <span className="rg-marca-texto">RetroGames</span>
          </Link>

          <div className="rg-nav__acoes-mobile d-flex align-items-center gap-2 d-lg-none">
            {!carregando && ehCliente && (
              <LinkCarrinho
                quantidade={quantidadeCarrinho}
                somenteIcone
                aoNavegar={fecharMenu}
              />
            )}

            <button
              type="button"
              className="navbar-toggler rg-nav__alternar"
              aria-label={menuAberto ? 'Fechar menu' : 'Abrir menu'}
              aria-controls="menu-principal"
              aria-expanded={menuAberto}
              onClick={() => setMenuAberto(aberto => !aberto)}
            >
              <span className="navbar-toggler-icon" />
            </button>
          </div>

          <div
            id="menu-principal"
            className={`collapse navbar-collapse rg-nav__menu ${menuAberto ? 'show' : ''}`}
          >
            <ul className="navbar-nav rg-nav__links">
              <li className="nav-item">
                <NavLink to="/" end className="nav-link" onClick={fecharMenu}>
                  Catálogo
                </NavLink>
              </li>

              {!carregando && ehCliente && (
                <li className="nav-item">
                  <NavLink to="/meus-pedidos" className="nav-link" onClick={fecharMenu}>
                    Meus pedidos
                  </NavLink>
                </li>
              )}

              {!carregando && ehAdministrador && (
                <li className="nav-item dropdown">
                  <button
                    type="button"
                    className="nav-link dropdown-toggle bg-transparent border-0"
                    data-bs-toggle="dropdown"
                    aria-expanded="false"
                  >
                    Administração
                  </button>
                  <ul className="dropdown-menu">
                    <li>
                      <Link className="dropdown-item" to="/admin/jogos" onClick={fecharMenu}>
                        Jogos
                      </Link>
                    </li>
                    <li>
                      <Link className="dropdown-item" to="/admin/plataformas" onClick={fecharMenu}>
                        Plataformas
                      </Link>
                    </li>
                  </ul>
                </li>
              )}
            </ul>

            <ul className="navbar-nav rg-nav__conta align-items-lg-center gap-lg-3">
              {carregando && (
                <li className="nav-item">
                  <span className="navbar-text" role="status">Verificando sessão...</span>
                </li>
              )}

              {!carregando && !estaAutenticado && (
                <>
                  <li className="nav-item">
                    <NavLink to="/login" className="nav-link" onClick={fecharMenu}>
                      Entrar
                    </NavLink>
                  </li>
                  <li className="nav-item">
                    <Link to="/cadastro" className="btn btn-primary btn-sm" onClick={fecharMenu}>
                      Criar conta
                    </Link>
                  </li>
                </>
              )}

              {!carregando && estaAutenticado && (
                <>
                  {ehCliente && (
                    <li className="nav-item d-none d-lg-block">
                      <LinkCarrinho quantidade={quantidadeCarrinho} aoNavegar={fecharMenu} />
                    </li>
                  )}

                  <li className="nav-item">
                    <span className="navbar-text rg-nav__saudacao" title={usuario?.nome}>
                      Olá, {usuario?.nome}
                    </span>
                  </li>

                  <li className="nav-item">
                    <button
                      type="button"
                      className="btn btn-outline-primary btn-sm"
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
        </div>
      </nav>

      <main id="conteudo-principal" tabIndex={-1} className="container py-4 flex-grow-1">
        {erroSaida && (
          <div className="alert alert-danger" role="alert">
            {erroSaida}
          </div>
        )}
        <Outlet />
      </main>

      <footer className="rg-rodape">
        <div className="container d-flex flex-column flex-sm-row justify-content-between align-items-center gap-2">
          <span className="d-inline-flex align-items-center gap-2">
            <Logo tamanho={24} />
            <span className="rg-marca-texto">RetroGames</span>
          </span>
          <span className="rg-rodape__texto">Clássicos de ontem. Diversão de sempre.</span>
        </div>
      </footer>
    </div>
  )
}

export default Layout
