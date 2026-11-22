
const formatadorPreco = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL'
})

export function formatarPreco(valor) {
  return formatadorPreco.format(valor)
}
