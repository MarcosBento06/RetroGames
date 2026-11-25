
import api from './api'

export const pedidoApi = {
  async finalizar(itens) {
    const resposta = await api.post('/Pedido', {
      itens
    })

    return resposta.data.data
  }
}
