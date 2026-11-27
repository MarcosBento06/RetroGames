import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'
import { useCarrinho } from '../../hooks/useCarrinho'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

function DetalheJogo() {
  const { id } = useParams()

  const { itens, adicionarJogo } = useCarrinho()
  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativas, setTentativas] = useState(0)

  const quantidadeNoCarrinho =
    itens.find(item => item.idJogo === jogo?.id)?.quantidade ?? 0

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
    <section className="rg-detalhe">
      <header className="rg-detalhe__cabecalho">
        <span className="rg-detalhe__etiqueta">
          CATÁLOGO RETROGAMES
        </span>

        <h1>Detalhes do jogo</h1>
      </header>

      {carregando ? (
        <Carregando texto="Carregando detalhes do jogo..." />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          onTentarNovamente={() =>
            setTentativas(valor => valor + 1)
          }
        />
      ) : !jogo ? (
        <EstadoVazio mensagem="Jogo não encontrado." />
      ) : (
        <article className="card rg-detalhe__card">
          <div className="card-body rg-detalhe__corpo">
            <div className="rg-detalhe__informacoes">
              <span className="rg-detalhe__plataforma">
                {jogo.nomePlataforma}
              </span>

              <h2 className="rg-detalhe__titulo">
                {jogo.nome}
              </h2>

              <p className="rg-detalhe__descricao">
                {jogo.descricao || 'Descrição não informada.'}
              </p>
            </div>

            <div className="rg-detalhe__dados">
              <div className="rg-detalhe__dado">
                <span>Condição</span>
                <strong>{jogo.descricaoCondicao}</strong>
              </div>

              <div className="rg-detalhe__dado">
                <span>Ano de lançamento</span>
                <strong>{jogo.anoLancamento}</strong>
              </div>

              <div className="rg-detalhe__dado">
                <span>Gêneros</span>
                <strong>
                  {jogo.generos?.map(genero => genero.nome).join(', ')
                    || 'Não informados'}
                </strong>
              </div>

              <div className="rg-detalhe__dado">
                <span>Estoque disponível</span>
                <strong>{jogo.estoque}</strong>
              </div>
            </div>

            <div className="rg-detalhe__compra">
              <div>
                <span className="rg-detalhe__preco-legenda">
                  Preço
                </span>

                <strong className="rg-detalhe__preco">
                  {formatarPreco(jogo.preco)}
                </strong>

                {jogo.estoque === 0 && (
                  <span className="rg-detalhe__esgotado">
                    Esgotado
                  </span>
                )}
              </div>

              <div className="rg-detalhe__acoes">
                <button
                  type="button"
                  className="btn btn-primary"
                  onClick={() => adicionarJogo(jogo.id)}
                  disabled={
                    jogo.estoque <= 0 ||
                    quantidadeNoCarrinho >= 10
                  }
                >
                  {jogo.estoque <= 0
                    ? 'Esgotado'
                    : quantidadeNoCarrinho >= 10
                      ? 'Limite de 10 unidades'
                      : 'Adicionar ao carrinho'}
                </button>

                {quantidadeNoCarrinho > 0 && (
                  <p className="text-success mb-0">
                    No carrinho: {quantidadeNoCarrinho} unidade(s).
                  </p>
                )}
              </div>
            </div>
          </div>
        </article>
      )}

      <Link to="/" className="btn btn-outline-primary mt-4">
        Voltar ao catálogo
      </Link>
    </section>
  )
}

export default DetalheJogo