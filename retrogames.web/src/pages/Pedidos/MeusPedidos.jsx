import { useEffect, useState } from 'react'
import { pedidoApi } from '../../api/servicos'

function MeusPedidos() {
  const [pedidos, setPedidos] = useState([])

  useEffect(() => {
    async function carregarPedidos() {
      try {
        const dados = await pedidoApi.listarMeus()
        setPedidos(dados ?? [])
      } catch (erro) {
        console.error('Erro ao carregar pedidos:', erro)
      }
    }

    carregarPedidos()
  }, [])

  return (
    <div>
      <div className="mb-4">
        <h1 className="mb-2">Meus pedidos</h1>
        <p className="text-muted">
          Consulte seu histórico de compras e os detalhes dos
          jogos que você adquiriu.
        </p>
      </div>

      <div className="d-flex flex-column gap-3">
        {pedidos.map((pedido) => (
          <div className="card" key={pedido.id}>
            <div className="card-body">
              <h5 className="card-title">
                Pedido #{pedido.id}
              </h5>

              <div className="d-flex justify-content-between mb-3">
                <span>{pedido.situacao}</span>
                <strong>
                  {Number(pedido.valorTotal).toLocaleString('pt-BR', {
                    style: 'currency',
                    currency: 'BRL'
                  })}
                </strong>
              </div>

              <div className="text-muted mb-3">
                {new Date(pedido.dataPedido).toLocaleDateString('pt-BR')}
              </div>

              {pedido.itens?.map((item, index) => (
                <div
                  className="d-flex justify-content-between border-top py-2"
                  key={`${pedido.id}-${index}`}
                >
                  <span>
                    {item.nomeJogo} — {item.quantidade} unidade(s)
                  </span>

                  <span>
                    {Number(item.valorUnitario).toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: 'BRL'
                    })}
                  </span>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default MeusPedidos