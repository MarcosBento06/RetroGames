
import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true
})

api.interceptors.response.use(
  (resposta) => resposta,

  (erro) => {
    const status = erro.response?.status
    const rota = erro.config?.url?.split('?')[0]

    const rotaAutenticacaoEsperada =
      rota === '/Autenticacao/eu' ||
      rota === '/Autenticacao/login'

    if (status === 401 && !rotaAutenticacaoEsperada) {
      window.dispatchEvent(new Event('sessao:expirada'))
    }

    return Promise.reject(erro)
  }
)

export default api
