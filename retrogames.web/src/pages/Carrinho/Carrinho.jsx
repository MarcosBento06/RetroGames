
import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

import { useCarrinho } from '../../hooks/useCarrinho'
import { jogoApi } from '../../api/servicos'
import { formatarPreco } from '../../lib/formato'

function Carrinho() {
  const { itens, alterarQuantidade, removerJogo } = useCarrinho()

  const [jogosPorId, setJogosPorId] = useState({})
  const [carregando, setCarregando] = useState(false)

  const idsJogos = itens.map(item => item.idJogo).join(',')

  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      if (!idsJogos) {
        setJogosPorId({})
        setCarregando(false)
        return
      }

      setCarregando(true)

      const ids = idsJogos.split(',').map(Number)

      const resultados = await Promise.allSettled(
        ids.map(idJogo => jogoApi.buscarPorId(idJogo))
      )

      if (ativo) {
        const detalhes = {}

        resultados.forEach((resultado, indice) => {
          detalhes[ids[indice]] =
            resultado.status === 'fulfilled'
              ? resultado.value
              : null
        })

        setJogosPorId(detalhes)
        setCarregando(false)
      }
    }

    carregarJogos()

    return () => {
      ativo = false
    }
  }, [idsJogos])

  const total = itens.reduce((soma, item) => {
    const jogo = jogosPorId[item.idJogo]

    if (!jogo) {
      return soma
    }

    return soma + Number(jogo.preco) * item.quantidade
  }, 0)

  return (
    <div>
      <h1 className="mb-4">Meu Carrinho</h1>

      {itens.length === 0 ? (
        <div>
          <p>Seu carrinho está vazio.</p>

          <Link to="/" className="btn btn-primary">
            Ver catálogo
          </Link>
        </div>
      ) : carregando ? (
        <p>Carregando jogos do carrinho...</p>
      ) : (
        <>
          <div className="table-responsive">
            <table className="table align-middle">
              <thead>
                <tr>
                  <th>Jogo</th>
                  <th>Preço unitário</th>
                  <th>Quantidade</th>
                  <th>Subtotal</th>
                  <th>Ações</th>
                </tr>
              </thead>

              <tbody>
                {itens.map(item => {
                  const jogo = jogosPorId[item.idJogo]

                  return (
                    <tr key={item.idJogo}>
                      <td>
                        {jogo?.nome ??
                          `Jogo indisponível (ID ${item.idJogo})`}
                      </td>

                      <td>
                        {jogo
                          ? formatarPreco(jogo.preco)
                          : '—'}
                      </td>

                      <td>
                        <select
                          className="form-select"
                          style={{ maxWidth: '100px' }}
                          value={item.quantidade}
                          aria-label={`Quantidade do jogo ${item.idJogo}`}
                          onChange={evento =>
                            alterarQuantidade(
                              item.idJogo,
                              Number(evento.target.value)
                            )
                          }
                        >
                          {Array.from(
                            { length: 10 },
                            (_, indice) => indice + 1
                          ).map(quantidade => (
                            <option
                              key={quantidade}
                              value={quantidade}
                            >
                              {quantidade}
                            </option>
                          ))}
                        </select>
                      </td>

                      <td>
                        {jogo
                          ? formatarPreco(
                              Number(jogo.preco) * item.quantidade
                            )
                          : '—'}
                      </td>

                      <td>
                        <button
                          type="button"
                          className="btn btn-outline-danger btn-sm"
                          onClick={() => removerJogo(item.idJogo)}
                        >
                          Remover
                        </button>
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>

          <div className="text-end mt-4">
            <h2 className="fs-4">
              Total: {formatarPreco(total)}
            </h2>

            <p className="text-muted small">
              O valor definitivo será calculado pelo servidor
              ao finalizar a compra.
            </p>
          </div>

          <Link to="/" className="btn btn-outline-primary">
            Continuar comprando
          </Link>
        </>
      )}
    </div>
  )
}

export default Carrinho
