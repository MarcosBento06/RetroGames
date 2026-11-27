import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  const esgotado = jogo.estoque <= 0

  return (
    <article className="card rg-card h-100">
      <div className="card-body rg-card__corpo d-flex flex-column">
        <div className="rg-card__cabecalho">
          <span className="rg-card__plataforma" title={jogo.nomePlataforma}>
            {jogo.nomePlataforma}
          </span>

          {esgotado && (
            <span className="badge text-bg-danger">Esgotado</span>
          )}
        </div>

        <h2 className="card-title rg-card__titulo">
          {jogo.nome}
        </h2>

        <p className="rg-card__detalhes">
          <span>Ano {jogo.anoLancamento}</span>
          <span className="rg-card__separador" aria-hidden="true">·</span>
          <span>Condição: {jogo.descricaoCondicao}</span>
        </p>

        {jogo.generos?.length > 0 && (
          <div className="d-flex flex-wrap gap-2 mb-3">
            {jogo.generos.map((genero, indice) => {
              const nome = typeof genero === 'string' ? genero : genero.nome

              return (
                <span key={`${nome}-${indice}`} className="rg-chip">
                  {nome}
                </span>
              )
            })}
          </div>
        )}

        <div className="rg-card__rodape mt-auto">
          <p className="rg-preco mb-3">
            {formatarPreco(jogo.preco)}
          </p>

          <Link
            to={`/jogos/${jogo.id}`}
            className="btn btn-primary w-100"
          >
            Ver detalhes
          </Link>
        </div>
      </div>
    </article>
  )
}

export default CardJogo
