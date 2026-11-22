
function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) {
    return null
  }

  const paginas = Array.from(
    { length: totalPaginas },
    (_, indice) => indice + 1
  )

  return (
    <nav aria-label="Paginação do catálogo">
      <ul className="pagination justify-content-center">
        <li className={`page-item ${pagina === 1 ? 'disabled' : ''}`}>
          <button
            type="button"
            className="page-link"
            disabled={pagina === 1}
            onClick={() => aoMudar(pagina - 1)}
          >
            Anterior
          </button>
        </li>

        {paginas.map((numero) => (
          <li
            key={numero}
            className={`page-item ${pagina === numero ? 'active' : ''}`}
          >
            <button
              type="button"
              className="page-link"
              aria-current={pagina === numero ? 'page' : undefined}
              onClick={() => aoMudar(numero)}
            >
              {numero}
            </button>
          </li>
        ))}

        <li
          className={`page-item ${pagina === totalPaginas ? 'disabled' : ''}`}
        >
          <button
            type="button"
            className="page-link"
            disabled={pagina === totalPaginas}
            onClick={() => aoMudar(pagina + 1)}
          >
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}

export default Paginacao
