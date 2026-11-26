import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import './styles/tema.css'

import AutenticacaoProvider from './contexts/AutenticacaoProvider'
import CarrinhoProvider from './contexts/CarrinhoProvider'
import App from './App.jsx'

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <AutenticacaoProvider>
        <CarrinhoProvider>
          <App />
        </CarrinhoProvider>
      </AutenticacaoProvider>
    </BrowserRouter>
  </StrictMode>
)
