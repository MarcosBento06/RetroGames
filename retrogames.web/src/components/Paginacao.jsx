// A paginação mantém as URLs de filtro e evita uma longa lista de números.
function gerarItens(atual, total) {
  const numeros = [...new Set([1, total, atual - 1, atual, atual + 1])]
    .filter(numero => numero >= 1 && numero <= total)
    .sort((a, b) => a - b)

  const itens = []

  numeros.forEach((numero, indice) => {
    if (indice > 0 && numero - numeros[indice - 1] > 1) {
      itens.push({ tipo: 'reticencias', chave: `r-${numero}` })
    }

    itens.push({ tipo: 'pagina', numero, chave: `p-${numero}` })
  })

  return itens
}

function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) {
    return null
  }

  const itens = gerarItens(pagina, totalPaginas)

  return (
    <nav className="rg-paginacao" aria-label="Paginação do catálogo">
      <ul className="pagination justify-content-center align-items-center gap-1 flex-nowrap">
        <li className={`page-item ${pagina === 1 ? 'disabled' : ''}`}>
          <button
            type="button"
            className="page-link"
            aria-label="Página anterior"
            disabled={pagina === 1}
            onClick={() => aoMudar(pagina - 1)}
          >
            <span className="d-none d-sm-inline">Anterior</span>
            <span className="d-sm-none" aria-hidden="true">‹</span>
          </button>
        </li>

        {itens.map(item => {
          if (item.tipo === 'reticencias') {
            return (
              <li key={item.chave} className="page-item disabled d-none d-sm-block" aria-hidden="true">
                <span className="page-link">…</span>
              </li>
            )
          }

          const proximaDaAtual = Math.abs(item.numero - pagina) <= 1

          return (
            <li
              key={item.chave}
              className={`page-item ${pagina === item.numero ? 'active' : ''} ${
                proximaDaAtual ? '' : 'd-none d-sm-block'
              }`}
            >
              <button
                type="button"
                className="page-link"
                aria-label={`Página ${item.numero}`}
                aria-current={pagina === item.numero ? 'page' : undefined}
                onClick={() => aoMudar(item.numero)}
              >
                {item.numero}
              </button>
            </li>
          )
        })}

        <li className={`page-item ${pagina === totalPaginas ? 'disabled' : ''}`}>
          <button
            type="button"
            className="page-link"
            aria-label="Próxima página"
            disabled={pagina === totalPaginas}
            onClick={() => aoMudar(pagina + 1)}
          >
            <span className="d-none d-sm-inline">Próxima</span>
            <span className="d-sm-none" aria-hidden="true">›</span>
          </button>
        </li>
      </ul>
      <p className="rg-paginacao__resumo text-center mb-0 mt-2">
        Página {pagina} de {totalPaginas}
      </p>
    </nav>
  )
}

export default Paginacao
