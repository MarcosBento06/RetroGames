
import { useEffect, useState } from 'react'
import { autenticacaoApi } from '../api/servicos'
import { AutenticacaoContext } from './autenticacaoContext'

function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    let ativo = true

    function sessaoExpirada() {
      if (ativo) {
        setUsuario(null)
      }
    }

    window.addEventListener('sessao:expirada', sessaoExpirada)

    async function verificarSessao() {
      try {
        const dados = await autenticacaoApi.usuarioAtual()

        if (ativo) {
          setUsuario(dados)
        }
      } catch (erro) {
        if (ativo) {
          setUsuario(null)

          if (erro.response?.status !== 401) {
            console.error(
              'Não foi possível verificar a sessão.',
              erro
            )
          }
        }
      } finally {
        if (ativo) {
          setCarregando(false)
        }
      }
    }

    verificarSessao()

    return () => {
      ativo = false
      window.removeEventListener('sessao:expirada', sessaoExpirada)
    }
  }, [])

  async function entrar(email, senha) {
    const dados = await autenticacaoApi.entrar(email, senha)

    setUsuario(dados)

    return dados
  }

  async function sair() {
    await autenticacaoApi.sair()

    setUsuario(null)
  }

  const estaAutenticado = usuario !== null

  const ehAdministrador =
    usuario?.perfil === 'Administrador'

  return (
    <AutenticacaoContext.Provider
      value={{
        usuario,
        carregando,
        estaAutenticado,
        ehAdministrador,
        entrar,
        sair
      }}
    >
      {children}
    </AutenticacaoContext.Provider>
  )
}

export default AutenticacaoProvider
