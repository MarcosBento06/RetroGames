import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { pedidoApi } from '../../api/servicos'
import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'
import { formatarPreco } from '../../lib/formato'

function MeusPedidos() {
  const [pedidos, setPedidos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  async function carregarPedidos() {
    setCarregando(true)
    setErro('')

    try {
      const dados = await pedidoApi.listarMeus()
      setPedidos(dados ?? [])
    } catch (erro) {
      console.error('Erro ao carregar pedidos:', erro)
      setErro('Não foi possível carregar seus pedidos.')
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarPedidos()
  }, [])

  if (carregando) {
    return <Carregando />
  }

  if (erro) {
    return (
      <MensagemErro
        mensagem={erro}
        onTentarNovamente={carregarPedidos}
      />
    )
  }

  return (
    <section className="rg-pedidos">
      <header className="rg-pedidos__cabecalho">
        <span className="rg-pedidos__etiqueta">
          SUA CONTA
        </span>

        <h1 className="mb-2">Meus pedidos</h1>

        <p className="text-secondary mb-0">
          Acompanhe suas compras, consulte os itens e confira
          os valores de cada pedido.
        </p>
      </header>

      {pedidos.length === 0 ? (
        <div className="rg-pedidos__vazio">
          <EstadoVazio
            mensagem="Você ainda não realizou nenhum pedido."
          />

          <Link to="/" className="btn btn-primary">
            Ir para o catálogo
          </Link>
        </div>
      ) : (
        <div className="rg-pedidos__lista">
          {pedidos.map(pedido => (
            <article
              className="card rg-pedido"
              key={pedido.id}
            >
              <div className="card-body rg-pedido__corpo">
                <header className="rg-pedido__cabecalho">
                  <div>
                    <span className="rg-pedido__identificacao">
                      PEDIDO
                    </span>

                    <h2 className="rg-pedido__titulo">
                      #{pedido.id}
                    </h2>
                  </div>

                  <div className="rg-pedido__situacao">
                    <span className="rg-pedido__situacao-legenda">
                      Situação
                    </span>

                    <span className="rg-pedido__situacao-valor">
                      {pedido.situacao || 'Não informada'}
                    </span>
                  </div>
                </header>

                <div className="rg-pedido__data">
                  <span>Data da compra</span>

                  <strong>
                    {new Date(pedido.dataPedido).toLocaleDateString(
                      'pt-BR'
                    )}
                  </strong>
                </div>

                <div className="rg-pedido__itens">
                  <h3>Itens do pedido</h3>

                  {pedido.itens?.length > 0 ? (
                    <div className="rg-pedido__lista-itens">
                      {pedido.itens.map((item, index) => {
                        const subtotal =
                          Number(item.valorUnitario) *
                          Number(item.quantidade)

                        return (
                          <div
                            className="rg-pedido__item"
                            key={`${pedido.id}-${index}`}
                          >
                            <div className="rg-pedido__item-info">
                              <strong className="rg-pedido__item-nome">
                                {item.nomeJogo}
                              </strong>

                              <span>
                                {item.quantidade} unidade(s)
                                {' · '}
                                {formatarPreco(item.valorUnitario)}
                                {' por unidade'}
                              </span>
                            </div>

                            <strong className="rg-pedido__item-subtotal">
                              {formatarPreco(subtotal)}
                            </strong>
                          </div>
                        )
                      })}
                    </div>
                  ) : (
                    <p className="text-secondary mb-0">
                      Não há itens disponíveis para exibição.
                    </p>
                  )}
                </div>

                <footer className="rg-pedido__rodape">
                  <span>Total do pedido</span>

                  <strong>
                    {formatarPreco(pedido.valorTotal)}
                  </strong>
                </footer>
              </div>
            </article>
          ))}
        </div>
      )}
    </section>
  )
}

export default MeusPedidos