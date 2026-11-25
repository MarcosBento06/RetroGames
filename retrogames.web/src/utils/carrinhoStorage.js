
const CHAVE_CARRINHO = 'retrogames:carrinho'

export function carregarCarrinho() {
  try {
    const dados = localStorage.getItem(CHAVE_CARRINHO)

    if (!dados) {
      return []
    }

    const itens = JSON.parse(dados)

    return Array.isArray(itens) ? itens : []
  } catch {
    return []
  }
}

export function salvarCarrinho(itens) {
  localStorage.setItem(
    CHAVE_CARRINHO,
    JSON.stringify(itens)
  )
}
