
import { Navigate, Outlet, useLocation } from 'react-router-dom'

import { useAutenticacao } from '../hooks/useAutenticacao'

function RotaProtegida({ somenteAdministrador = false }) {
  const {
    carregando,
    estaAutenticado,
    ehAdministrador
  } = useAutenticacao()

  const localizacao = useLocation()

  // Aguarda a API verificar a sessão
  if (carregando) {
    return (
      <p role="status">
        Verificando sessão...
      </p>
    )
  }

  // Visitante precisa fazer login
  if (!estaAutenticado) {
    const destino =
      localizacao.pathname +
      localizacao.search +
      localizacao.hash

    return (
      <Navigate
        to={`/login?voltarPara=${encodeURIComponent(destino)}`}
        replace
      />
    )
  }

  // Usuário logado sem permissão de administrador
  if (somenteAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  // Usuário autorizado
  return <Outlet />
}

export default RotaProtegida
