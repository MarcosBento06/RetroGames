
import { Link } from 'react-router-dom'

function NaoEncontrada() {
  return (
    <div className="text-center py-5">
      <h1>404 — Página não encontrada</h1>

      <p className="text-muted">
        A página que você está procurando não existe.
      </p>

      <Link to="/" className="btn btn-primary">
        Voltar ao catálogo
      </Link>
    </div>
  )
}

export default NaoEncontrada
