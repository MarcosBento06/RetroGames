export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div
      className="rg-estado rg-estado--carregando"
      role="status"
      aria-live="polite"
      aria-busy="true"
    >
      <div
        className="spinner-border text-primary rg-estado__spinner"
        aria-hidden="true"
      />

      <p className="rg-estado__texto">
        {texto}
      </p>
    </div>
  )
}

export function MensagemErro({
  mensagem,
  aoTentarNovamente,
  onTentarNovamente
}) {
  const tentarNovamente =
    aoTentarNovamente ?? onTentarNovamente

  return (
    <section
      className="rg-estado rg-estado--erro"
      role="alert"
      aria-live="assertive"
    >
      <span className="rg-estado__icone" aria-hidden="true">
        !
      </span>

      <div className="rg-estado__conteudo">
        <h2 className="rg-estado__titulo">
          Não foi possível concluir a operação
        </h2>

        <p className="rg-estado__texto">
          {mensagem}
        </p>

        {tentarNovamente && (
          <button
            type="button"
            className="btn btn-outline-danger"
            onClick={tentarNovamente}
          >
            Tentar novamente
          </button>
        )}
      </div>
    </section>
  )
}

export function EstadoVazio({
  titulo,
  mensagem,
  children
}) {
  const texto = mensagem ?? children

  return (
    <section className="rg-estado rg-estado--vazio">
      <span className="rg-estado__icone" aria-hidden="true">
        —
      </span>

      <div className="rg-estado__conteudo">
        <h2 className="rg-estado__titulo">
          {titulo ?? mensagem ?? 'Nenhum resultado encontrado.'}
        </h2>

        {titulo && mensagem && (
          <p className="rg-estado__texto">
            {mensagem}
          </p>
        )}

        {titulo && children && (
          <div className="rg-estado__texto">
            {children}
          </div>
        )}
      </div>
    </section>
  )
}