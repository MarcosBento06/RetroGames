
import { Route, Routes } from 'react-router-dom'

import Layout from './components/Layout'
import RotaProtegida from './components/RotaProtegida'

import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import Login from './pages/Login/Login'

// Telas provisórias da administração
function PaginaProvisoria({ titulo }) {
  return <h1>{titulo}</h1>
}

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Rotas públicas */}
        <Route path="/" element={<Catalogo />} />

        <Route
          path="/jogos/:id"
          element={<DetalheJogo />}
        />

        <Route path="/login" element={<Login />} />

        {/* Rotas administrativas */}
        <Route
          path="/admin"
          element={<RotaProtegida somenteAdministrador />}
        >
          <Route
            path="jogos"
            element={
              <PaginaProvisoria titulo="Administração de Jogos" />
            }
          />

          <Route
            path="jogos/novo"
            element={
              <PaginaProvisoria titulo="Cadastrar Jogo" />
            }
          />

          <Route
            path="jogos/:id/editar"
            element={
              <PaginaProvisoria titulo="Editar Jogo" />
            }
          />

          <Route
            path="plataformas"
            element={
              <PaginaProvisoria titulo="Administração de Plataformas" />
            }
          />
        </Route>

        {/* Página não encontrada */}
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
