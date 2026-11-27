import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useCarrinho } from '../../hooks/useCarrinho'
import { jogoApi } from '../../api/servicos'
import { formatarPreco } from '../../lib/formato'
import { pedidoApi } from '../../api/pedidos'
import { obterMensagemErro } from '../../api/erros'

function Carrinho() {
  const {
    itens,
    alterarQuantidade,
    removerJogo,
    limparCarrinho
  } = useCarrinho()

  const [jogosPorId, setJogosPorId] = useState({})
  const [carregando, setCarregando] = useState(false)
  const [finalizando, setFinalizando] = useState(false)
  const [erroCompra, setErroCompra] = useState('')
  const [compraRealizada, setCompraRealizada] = useState(false)

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

  const possuiJogoIndisponivel = itens.some(
    item => !jogosPorId[item.idJogo]
  )

  async function finalizarCompra() {
    if (
      finalizando ||
      carregando ||
      itens.length === 0 ||
      possuiJogoIndisponivel
    ) {
      return
    }

    setFinalizando(true)
    setErroCompra('')
    setCompraRealizada(false)

    try {
      await pedidoApi.finalizar(itens)

      // O carrinho só é limpo depois da confirmação de sucesso da API.
      limparCarrinho()

      setCompraRealizada(true)
    } catch (erro) {
      // Em caso de erro, os itens continuam no carrinho.
      setErroCompra(obterMensagemErro(erro))
    } finally {
      setFinalizando(false)
    }
  }

  return (
    <section className="rg-carrinho">
      <div className="rg-carrinho__cabecalho">
        <div>
          <span className="rg-carrinho__etiqueta">
            SUA COMPRA
          </span>

          <h1 className="mb-2">Meu carrinho</h1>

          <p className="text-secondary mb-0">
            Confira os jogos, as quantidades e os valores antes de finalizar.
          </p>
        </div>

        <span className="rg-carrinho__contador">
          {itens.reduce(
            (soma, item) => soma + item.quantidade,
            0
          )}{' '}
          unidade(s)
        </span>
      </div>

      {erroCompra && (
        <div
          className="alert alert-danger rg-carrinho__mensagem"
          role="alert"
          aria-live="assertive"
        >
          <strong>Não foi possível finalizar a compra.</strong>
          <p className="mb-0 mt-1">{erroCompra}</p>
          <p className="mb-0 mt-2 small">
            Seus itens continuam salvos no carrinho. Confira as
            informações e tente novamente.
          </p>
        </div>
      )}

      {compraRealizada && (
        <div
          className="alert alert-success rg-carrinho__mensagem"
          role="status"
          aria-live="polite"
        >
          <strong>Compra realizada com sucesso!</strong>

          <p className="mb-2 mt-1">
            Seu pedido foi registrado. Você já pode acompanhar
            os detalhes no histórico de compras.
          </p>

          <Link
            to="/meus-pedidos"
            className="btn btn-sm btn-outline-success"
          >
            Consultar meus pedidos
          </Link>
        </div>
      )}

      {itens.length === 0 ? (
        <div className="rg-carrinho__vazio">
          <div className="rg-carrinho__vazio-icone" aria-hidden="true">
            🛒
          </div>

          <h2>Seu carrinho está vazio</h2>

          <p className="text-secondary">
            Explore nosso catálogo e encontre seu próximo clássico.
          </p>

          <Link to="/" className="btn btn-primary">
            Ver catálogo
          </Link>
        </div>
      ) : carregando ? (
        <div className="rg-carrinho__carregando" role="status">
          <div
            className="spinner-border text-primary"
            aria-hidden="true"
          />
          <p className="mt-3 mb-0">
            Carregando os jogos do seu carrinho...
          </p>
        </div>
      ) : (
        <>
          {possuiJogoIndisponivel && (
            <div className="alert alert-warning" role="alert">
              Não foi possível consultar todos os jogos.
              Verifique sua conexão ou remova os itens indisponíveis
              antes de finalizar.
            </div>
          )}

          <div className="rg-carrinho__painel">
            <div className="rg-carrinho__painel-titulo">
              <h2>Itens da compra</h2>
              <span>{itens.length} jogo(s)</span>
            </div>

            <div className="table-responsive rg-carrinho__tabela-wrapper">
              <table className="table align-middle rg-carrinho__tabela">
                <caption className="visually-hidden">
                  Jogos, preços, quantidades e subtotais do carrinho.
                </caption>

                <thead>
                  <tr>
                    <th scope="col">Jogo</th>
                    <th scope="col">Preço unitário</th>
                    <th scope="col">Quantidade</th>
                    <th scope="col">Subtotal</th>
                    <th scope="col">Ações</th>
                  </tr>
                </thead>

                <tbody>
                  {itens.map(item => {
                    const jogo = jogosPorId[item.idJogo]

                    return (
                      <tr key={item.idJogo}>
                        <td className="rg-carrinho__nome">
                          <strong>
                            {jogo?.nome ??
                              `Jogo indisponível (ID ${item.idJogo})`}
                          </strong>
                        </td>

                        <td>
                          {jogo
                            ? formatarPreco(jogo.preco)
                            : '—'}
                        </td>

                        <td>
                          <select
                            className="form-select rg-carrinho__quantidade"
                            value={item.quantidade}
                            disabled={finalizando}
                            aria-label={`Quantidade de ${jogo?.nome ?? 'jogo'}`}
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

                        <td className="rg-carrinho__subtotal">
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
                            disabled={finalizando}
                            aria-label={`Remover ${jogo?.nome ?? 'jogo'} do carrinho`}
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
          </div>

          <div className="rg-carrinho__resumo">
            <div>
              <span className="rg-carrinho__resumo-legenda">
                Total da compra
              </span>

              <strong className="rg-carrinho__total">
                {formatarPreco(total)}
              </strong>

              <p className="text-secondary small mb-0">
                O valor definitivo será calculado pelo servidor
                ao finalizar a compra.
              </p>
            </div>

            <div className="rg-carrinho__acoes">
              <Link
                to="/"
                className="btn btn-outline-primary"
              >
                Continuar comprando
              </Link>

              <button
                type="button"
                className="btn btn-success rg-carrinho__botao-finalizar"
                onClick={finalizarCompra}
                disabled={
                  finalizando ||
                  carregando ||
                  possuiJogoIndisponivel
                }
              >
                {finalizando ? (
                  <>
                    <span
                      className="spinner-border spinner-border-sm me-2"
                      aria-hidden="true"
                    />
                    Enviando pedido...
                  </>
                ) : (
                  'Finalizar compra'
                )}
              </button>
            </div>
          </div>
        </>
      )}
    </section>
  )
}

export default Carrinho