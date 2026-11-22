
import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <div className="card h-100">
      <div className="card-body d-flex flex-column">
        <p className="text-muted mb-2">
          {jogo.nomePlataforma}
        </p>

        <h2 className="card-title h5">
          {jogo.nome}
        </h2>

        <p className="card-text">
          <strong>Ano:</strong> {jogo.anoLancamento}
        </p>

        <p className="card-text">
          <strong>Condição:</strong> {jogo.descricaoCondicao}
        </p>

        <div className="mb-3">
          {jogo.generos?.map((genero) => (
            <span
              key={genero}
              className="badge bg-secondary me-1"
            >
              {genero}
            </span>
          ))}
        </div>

        <p className="fs-5 fw-bold mt-auto">
          {formatarPreco(jogo.preco)}
        </p>

        {esgotado && (
          <span className="badge bg-danger mb-3 align-self-start">
            Esgotado
          </span>
        )}

        <Link
          to={`/jogos/${jogo.id}`}
          className="btn btn-primary"
        >
          Ver detalhes
        </Link>
      </div>
    </div>
  )
}

export default CardJogo
