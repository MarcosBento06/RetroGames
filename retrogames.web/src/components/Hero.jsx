import { Link } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'

function Hero() {
  const { carregando, estaAutenticado } = useAutenticacao()
  const mostrarCadastro = !carregando && !estaAutenticado

  function irParaCatalogo() {
    const alvo = document.getElementById('jogos')
    if (!alvo) return

    const reduzirMovimento = window.matchMedia(
      '(prefers-reduced-motion: reduce)'
    ).matches

    alvo.scrollIntoView({
      behavior: reduzirMovimento ? 'auto' : 'smooth',
      block: 'start'
    })
    alvo.focus({ preventScroll: true })
  }

  return (
    <section className="rg-hero" aria-labelledby="titulo-hero">
      <div className="rg-hero__conteudo">
        <p className="rg-hero__etiqueta">A sua loja de clássicos</p>
        <h1 id="titulo-hero" className="rg-hero__titulo">
          Jogos retrô, diversão de sempre.
        </h1>
        <p className="rg-hero__desc">
          Encontre jogos para seus consoles favoritos, compare condições
          e monte seu carrinho com facilidade.
        </p>

        <div className="rg-hero__acoes">
          <button
            type="button"
            className="btn btn-primary"
            onClick={irParaCatalogo}
          >
            Ver catálogo
          </button>

          {mostrarCadastro && (
            <Link to="/cadastro" className="btn btn-outline-primary">
              Criar conta
            </Link>
          )}
        </div>
      </div>
    </section>
  )
}

export default Hero
