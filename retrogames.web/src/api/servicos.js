
import api from './api'

export const jogoApi = {
  async listar(filtros = {}) {
    const resposta = await api.get('/Jogo', {
      params: filtros
    })

    return resposta.data.data
  },

  async buscarPorId(id) {
    const resposta = await api.get(`/Jogo/${id}`)

    return resposta.data.data
  },

  async inserir(dados) {
    const resposta = await api.post('/Jogo', dados)

    return resposta.data.data
  },

  async alterar(id, dados) {
    const resposta = await api.put(`/Jogo/${id}`, dados)

    return resposta.data.data
  },

  async desativar(id) {
    const resposta = await api.delete(`/Jogo/${id}`)

    return resposta.data.data
  }
}

export const plataformaApi = {
  async listar() {
    const resposta = await api.get('/Plataforma')

    return resposta.data.data
  },

  async inserir(dados) {
    const resposta = await api.post('/Plataforma', dados)

    return resposta.data.data
  },

  async alterar(id, dados) {
    const resposta = await api.put(`/Plataforma/${id}`, dados)

    return resposta.data.data
  },

  async excluir(id) {
    const resposta = await api.delete(`/Plataforma/${id}`)

    return resposta.data.data
  }
}

export const generoApi = {
  async listar() {
    const resposta = await api.get('/Genero')

    return resposta.data.data
  }
}

export const condicaoJogoApi = {
  async listar() {
    const resposta = await api.get('/TabelaTipo/condicoes-jogo')

    return resposta.data.data
  }
}

export const autenticacaoApi = {
  async entrar(email, senha) {
    const resposta = await api.post('/Autenticacao/login', {
      email,
      senha
    })

    return resposta.data.data
  },

  async sair() {
    const resposta = await api.post('/Autenticacao/logout')

    return resposta.data.data
  },

  async usuarioAtual() {
    const resposta = await api.get('/Autenticacao/eu')

    return resposta.data.data
  }
}
