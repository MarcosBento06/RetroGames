
import { useContext } from 'react'
import { CarrinhoContext } from '../contexts/carrinhoContext'

export function useCarrinho() {
  const contexto = useContext(CarrinhoContext)

  if (contexto === null) {
    throw new Error(
      'useCarrinho deve ser utilizado dentro de CarrinhoProvider.'
    )
  }

  return contexto
}
