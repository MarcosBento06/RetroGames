
import { Route, Routes } from 'react-router-dom'

import Layout from './components/Layout'
import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'
import Login from './pages/Login/Login'

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Catalogo />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route path="/login" element={<Login />} />
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
