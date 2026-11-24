
export function obterMensagemErro(erro) {
  if (!erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  return erro.response.data?.errorMessage
    || 'Ocorreu um erro ao processar a solicitação.'
}

export function obterErrosDosCampos(erro) {
  const resposta = erro.response?.data

  if (resposta?.responseCode !== 3) {
    return {}
  }

  const dados = resposta.data

  if (!dados || typeof dados !== 'object' || Array.isArray(dados)) {
    return {}
  }

  const erros = {}

  for (const [campo, mensagens] of Object.entries(dados)) {
    erros[campo] = Array.isArray(mensagens)
      ? mensagens.join(' ')
      : String(mensagens ?? '')
  }

  return erros
}
