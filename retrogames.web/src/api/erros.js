export function obterMensagemErro(erro) {
  if (!erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  const resposta = erro.response.data

  switch (resposta?.responseCode) {
    case 110:
      return 'Um ou mais jogos não estão disponíveis para venda. Remova os jogos indisponíveis do carrinho e tente novamente.'

    case 111:
      return 'Estoque insuficiente para um ou mais jogos. Altere as quantidades no carrinho e tente novamente.'

    default:
      return resposta?.errorMessage
        || 'Ocorreu um erro ao processar a solicitação.'
  }
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
