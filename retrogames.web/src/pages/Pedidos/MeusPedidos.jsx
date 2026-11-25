import { useEffect, useState } from 'react'
import { pedidoApi } from '../../api/servicos'
import { Link } from 'react-router-dom'
import { EstadoVazio } from '../../components/MensagensEstado'

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

      {pedidos.length === 0 ? (
        <div>
          <EstadoVazio mensagem="Você ainda não realizou nenhum pedido." />

          <Link to="/" className="btn btn-primary">
            Ir para o catálogo
          </Link>
        </div>
      ) : (
        <div className="d-flex flex-column gap-3">
          {pedidos.map((pedido) => (
            // Mantenha aqui o conteúdo atual do card de cada pedido.
          ))}
        </div>
      )}
    </div>
  )
}

export default MeusPedidos