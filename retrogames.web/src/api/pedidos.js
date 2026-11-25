import api from './api'

export const pedidoApi = {
  async finalizar(itens) {
    const dadosPedido = {
      itens: itens.map(item => ({
        idJogo: item.idJogo,
        quantidade: item.quantidade
      }))
    }

    const resposta = await api.post('/Pedido', dadosPedido)

    return resposta.data.data
  }
}