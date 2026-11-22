
export function obterMensagemErro(erro) {
  if (!erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  return erro.response.data?.errorMessage
    || 'Ocorreu um erro ao processar a solicitação.'
}
