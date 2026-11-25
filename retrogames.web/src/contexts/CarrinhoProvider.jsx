
import { useEffect, useState } from 'react'

import { CarrinhoContext } from './carrinhoContext'
import {
  carregarCarrinho,
  salvarCarrinho
} from '../utils/carrinhoStorage'

function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(carregarCarrinho)

  useEffect(() => {
    salvarCarrinho(itens)
  }, [itens])

  function adicionarJogo(idJogo) {
    setItens(itensAtuais => {
      const itemExistente = itensAtuais.find(
        item => item.idJogo === idJogo
      )

      if (itemExistente) {
        return itensAtuais.map(item =>
          item.idJogo === idJogo
            ? {
                ...item,
                quantidade: Math.min(item.quantidade + 1, 10)
              }
            : item
        )
      }

      return [
        ...itensAtuais,
        { idJogo, quantidade: 1 }
      ]
    })
  }

  function alterarQuantidade(idJogo, quantidade) {
    if (
      !Number.isInteger(quantidade) ||
      quantidade < 1 ||
      quantidade > 10
    ) {
      return
    }

    setItens(itensAtuais =>
      itensAtuais.map(item =>
        item.idJogo === idJogo
          ? { ...item, quantidade }
          : item
      )
    )
  }

  function removerJogo(idJogo) {
    setItens(itensAtuais =>
      itensAtuais.filter(
        item => item.idJogo !== idJogo
      )
    )
  }

  function limparCarrinho() {
    setItens([])
  }

  return (
    <CarrinhoContext.Provider
      value={{
        itens,
        adicionarJogo,
        alterarQuantidade,
        removerJogo,
        limparCarrinho
      }}
    >
      {children}
    </CarrinhoContext.Provider>
  )
}

export default CarrinhoProvider
