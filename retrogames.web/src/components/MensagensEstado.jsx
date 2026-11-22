
export function Carregando() {
  return (
    <div className="text-center my-4" role="status">
      <div className="spinner-border text-primary" aria-hidden="true" />
      <p className="mt-2">Carregando...</p>
    </div>
  )
}

export function MensagemErro({ mensagem, onTentarNovamente }) {
  return (
    <div className="alert alert-danger" role="alert">
      <p>{mensagem}</p>

      <button
        type="button"
        className="btn btn-outline-danger"
        onClick={onTentarNovamente}
      >
        Tentar novamente
      </button>
    </div>
  )
}

export function EstadoVazio({ mensagem = 'Nenhum resultado encontrado.' }) {
  return (
    <div className="alert alert-info">
      {mensagem}
    </div>
  )
}
