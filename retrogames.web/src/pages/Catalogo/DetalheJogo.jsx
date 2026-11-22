
import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'

import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

function DetalheJogo() {
  const { id } = useParams()

  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativas, setTentativas] = useState(0)

  useEffect(() => {
    let ativo = true

    async function carregarJogo() {
      setCarregando(true)
      setErro('')
      setJogo(null)

      try {
        const dados = await jogoApi.buscarPorId(id)

        if (ativo) {
          setJogo(dados)
        }
      } catch (erroRequisicao) {
        if (ativo) {
          setErro(obterMensagemErro(erroRequisicao))
        }
      } finally {
        if (ativo) {
          setCarregando(false)
        }
      }
    }

    carregarJogo()

    return () => {
      ativo = false
    }
  }, [id, tentativas])

  return (
    <div>
      <h1 className="mb-4">Detalhes do Jogo</h1>

      {carregando ? (
        <Carregando />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          onTentarNovamente={() =>
            setTentativas((valor) => valor + 1)
          }
        />
      ) : !jogo ? (
        <EstadoVazio mensagem="Jogo não encontrado." />
      ) : (
        <div className="card">
          <div className="card-body">
            <h2 className="card-title">
              {jogo.nome}
            </h2>

            <p>
              <strong>Plataforma:</strong> {jogo.nomePlataforma}
            </p>

            <p>
              <strong>Condição:</strong> {jogo.descricaoCondicao}
            </p>

            <p>
              <strong>Ano de lançamento:</strong> {jogo.anoLancamento}
            </p>

            <p>
              <strong>Gêneros:</strong>{' '}
              {jogo.generos?.map((genero) => genero.nome).join(', ')
                || 'Não informados'}
            </p>

            <p>
              <strong>Descrição:</strong>{' '}
              {jogo.descricao || 'Descrição não informada.'}
            </p>

            <p className="fs-4 fw-bold">
              {formatarPreco(jogo.preco)}
            </p>

            <p>
              <strong>Estoque:</strong> {jogo.estoque}
            </p>

            {jogo.estoque === 0 && (
              <span className="badge bg-danger">
                Esgotado
              </span>
            )}
          </div>
        </div>
      )}

      <Link to="/" className="btn btn-outline-primary mt-4">
        Voltar ao catálogo
      </Link>
    </div>
  )
}

export default DetalheJogo
