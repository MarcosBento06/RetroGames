# AGENTS.md — Diretrizes para assistentes de IA no RetroGames

Este arquivo orienta qualquer assistente de IA usado neste repositório. O RetroGames é o projeto de um curso de desenvolvimento web: o estudante escreve os prompts e é responsável pelo código que aceita. O assistente produz código quando pedido, mas sempre de forma que o estudante consiga entender, testar e explicar o resultado.

## 1. Papel do assistente

- Implementar o que o estudante pedir, dentro das regras deste arquivo.
- Explicar o código que gerar: o que faz, por que foi escrito assim e onde está a parte importante.
- Apontar quando um pedido contraria a especificação, em vez de atendê-lo em silêncio.
- Ajudar a investigar erros a partir da causa, não substituindo blocos de código às cegas.
- Responder em português, com nomes de classes, métodos e variáveis em português.

## 2. Referências, em ordem de prioridade

1. Especificação técnica **RetroGames — Arquitetura e Stack** (documento fornecido pelo professor).
2. Guia da aula atual (seção "Requisitos" da etapa em andamento).
3. `docs/MER.md` e `docs/USER_STORIES.md` deste repositório.
4. Convenções do código existente.
5. Boas práticas das tecnologias da stack.

Se duas referências se contradizem, aponte o conflito e pergunte ao estudante qual seguir. Não escolha uma interpretação sem avisar.

## 3. Regras do projeto

### Stack

.NET 10 (ASP.NET Core com controllers), ADO.NET com MySqlConnector, MySQL 8, FluentValidation, xUnit + Moq, React + Vite + Bootstrap + Axios. Não adicione Entity Framework, Dapper, AutoMapper, JWT, TypeScript, Tailwind nem outro pacote sem que o estudante peça e o professor tenha aprovado.

### Camadas

| Camada | Faz | Não faz |
| --- | --- | --- |
| Controller | Recebe a requisição, chama o Service, devolve o envelope com o status HTTP | Regra de negócio, SQL |
| Validator | Valida formato (obrigatório, tamanho, faixa) | Consultar o banco |
| Service | Regras de negócio, transações, composição de dados com LINQ, `BusinessRuleException` | Conhecer HTTP ou SQL |
| Repository | SQL parametrizado e mapeamento coluna → propriedade | Decidir regras, abrir transações |

### Banco de dados

- O banco guarda só a estrutura: PK, FK com comportamento padrão, tipos, `NOT NULL` e `UNIQUE`.
- Não use trigger, function, `CHECK`, `ON DELETE CASCADE` ou `ON DELETE SET NULL`.
- Nomes seguem a convenção `tb_`, `co_seq_`, `co_`, `no_`, `ds_`, `nu_`, `vl_`, `qt_`, `dt_`, `st_`.
- Mudanças de estrutura entram em um novo script numerado em `database/`.

### Repository

- Todo valor entra como parâmetro (`@nome`). Nunca concatene valores no SQL.
- Cada consulta lê uma única tabela: sem `JOIN` e sem subconsulta em outra tabela.
- Filtros, ordenação e paginação (`LIMIT`/`OFFSET`) ficam no SQL.
- INSERT e UPDATE em tabela com `UNIQUE` usam `ExecutarGravacaoAsync`, que converte o erro 1062 do MySQL em `RegistroDuplicadoException`.

### Service

- Toda regra de negócio fica aqui, inclusive as que o banco também garante.
- Dados de várias tabelas são combinados com LINQ e devolvidos em um ServiceModel (exemplo: `JogoDetalheServiceModel`). Filtro que depende de outra tabela também é resolvido aqui, com LINQ (exemplo: filtro por gênero no `JogoService`).
- Erros de negócio: `throw new BusinessRuleException(BusinessErrors.X)`.
- `RegistroDuplicadoException` vinda do Repository vira o erro de negócio correspondente (exemplo: `PlataformaNomeDuplicado`).
- Transação só quando a operação grava mais de um comando, sempre neste formato:

```csharp
var transactionOptions = new TransactionOptions
{
    IsolationLevel = IsolationLevel.ReadCommitted,
    Timeout = TransactionManager.DefaultTimeout,
};
using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
           transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
{
    // chamadas aos repositórios
    transactionScope.Complete();
}
```

### Modelos

- Uma classe por tabela em `RetroGames.Model/Model`, inclusive `JogoGenero`.
- Apenas propriedades, cada uma comentada com a coluna correspondente.
- `decimal` para valores, `DateTime` para datas, `bool` para `st_`, tipo anulável para coluna opcional.

### API

- Toda resposta usa o envelope `{ "responseCode", "data", "errorMessage" }`.
- Códigos em `ResponseCodes.cs`, sequenciais: `SUCESSO = 0`, `ERRO = 1` e, a partir de 2, tuplas `(Code, DefaultMessage)`. Código novo recebe o próximo número livre.
- Status HTTP: 200, 201, 204, 400, 401, 403, 404, 409, 500, conforme a especificação.

### Segurança

- Connection string e senha do administrador inicial ficam em `dotnet user-secrets`. Nunca escreva credenciais em arquivo versionado, nem em exemplos.
- Senhas são guardadas apenas como hash (`PasswordHasher`).
- Sessão por cookie com proteção CSRF; não guarde token no `localStorage`.
- `bin/`, `obj/`, `node_modules/` e `dist/` não são versionados.

## 4. Como trabalhar com o estudante

### Ao gerar código

- Faça a menor mudança que atende ao pedido. Não reorganize arquivos que não fazem parte da tarefa.
- Siga os nomes e o estilo dos arquivos vizinhos.
- Depois do código, diga em poucas linhas o que ele faz e qual parte merece atenção.
- Indique como verificar: comando, consulta SQL, chamada no Swagger ou teste unitário.
- Se o pedido for grande, proponha dividir em partes que possam ser testadas uma a uma.

### Ao investigar um erro

1. Leia a mensagem e identifique onde ocorreu.
2. Explique o que a mensagem significa.
3. Liste as causas prováveis.
4. Indique como confirmar cada uma.
5. Corrija a causa e explique por que a correção resolve.

### Escolhas didáticas

Algumas regras existem para fins de ensino e não são a única forma correta. Quando o estudante perguntar, explique a escolha e a alternativa usada no mercado:

- **Uma tabela por consulta, com composição em LINQ:** separa o acesso a dados da montagem do resultado. No mercado, o comum é `JOIN` no SQL ou um ORM.
- **Nenhuma regra no banco:** concentra as regras em um só lugar, onde são testadas. No mercado, `CHECK` e restrições no banco são comuns como segunda proteção.

### Honestidade

- Não invente requisitos, rotas, colunas ou códigos de resposta. Se faltar informação, diga o que falta.
- Não diga que executou, compilou ou testou algo que não executou.
- Separe hipótese de fato confirmado.
- Deixe claro quando um trecho é só exemplo e não deve ir para o projeto.
