
import { Route, Routes } from 'react-router-dom'

import Layout from './components/Layout'
import RotaProtegida from './components/RotaProtegida'

import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import Login from './pages/Login/Login'
import Plataforma from './pages/Auxiliares/Plataforma'
import Jogos from './pages/Administracao/Jogos'
import FormularioJogo from './pages/Administracao/FormularioJogo'
import Cadastro from './pages/Cadastro/Cadastro'


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
        <Route path="/cadastro" element={<Cadastro />} />


        {/* Rotas administrativas */}
        <Route
          path="/admin"
          element={<RotaProtegida somenteAdministrador />}
        >
          <Route path="jogos" element={<Jogos />} />

          <Route
            path="jogos/novo"
            element={<FormularioJogo />}
          />

          <Route
            path="jogos/:id/editar"
            element={<FormularioJogo />}
          />

          <Route
              path="plataformas"
              element={<Plataforma />}
          />
        </Route>

        {/* Página não encontrada */}
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
