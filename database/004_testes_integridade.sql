-- RetroGames | 004_testes_integridade.sql
-- Etapa 5: testes das restricoes de integridade, conforme especificacao 4.2/4.3.

USE retrogames;

-- Verificacoes antes dos testes: a plataforma 1 existe e possui jogos;
-- a plataforma 999999 nao existe; Mega Drive ja esta cadastrada.
SELECT co_seq_plataforma, no_plataforma FROM tb_plataforma
WHERE co_seq_plataforma IN (1, 999999) OR no_plataforma = 'Mega Drive';
SELECT COUNT(*) AS jogos_da_plataforma_1 FROM tb_jogo WHERE co_plataforma = 1;

-- TESTE 1: Inserir jogo com co_plataforma inexistente.
-- Esperado: ERROR 1452 (chave estrangeira fk_jogo_plataforma).
START TRANSACTION;
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('TESTE_FK_1452', 'Registro temporario', 1990, 10.00,
     1, 1, UTC_TIMESTAMP(6), 999999, 2);
ROLLBACK;
-- Resultado observado: [PREENCHER APOS EXECUTAR: codigo e mensagem]

-- TESTE 2: Inserir plataforma com nome ja cadastrado.
-- Esperado: ERROR 1062 (restricao UNIQUE em no_plataforma).
START TRANSACTION;
INSERT INTO tb_plataforma
    (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Mega Drive', 'Sega', 1988);
ROLLBACK;
-- Resultado observado: 
-- Teste 2: Inserir plataforma com nome duplicado
-- Resultado esperado: Erro 1062 (UNIQUE)
-- Resultado observado: Erro 1062
-- Situação: Aprovado


-- TESTE 3: Excluir plataforma que possui jogos vinculados.
-- Esperado: ERROR 1451 (chave estrangeira fk_jogo_plataforma).
START TRANSACTION;
DELETE FROM tb_plataforma
WHERE co_seq_plataforma = 1;
ROLLBACK;
-- Resultado observado: 
-- Teste 3: Excluir plataforma que possui jogos vinculados.
-- Resultado esperado: Erro 1451 (FK)
-- Resultado observado: Erro 1451
-- Situação: Aprovado


-- TESTE 4: Inserir jogo com nome nulo.
-- Esperado: ERROR 1048 (coluna no_jogo com NOT NULL).
START TRANSACTION;
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (NULL, 'Registro temporario', 1990, 10.00,
     1, 1, UTC_TIMESTAMP(6), 1, 2);
ROLLBACK;
-- Resultado observado: 
-- Teste 4: Inserir jogo com nome nulo.
-- Resultado esperado: Erro 1048 (NOT NULL)
-- Resultado observado: Erro 1048
-- Situação: Aprovado


-- TESTE 5: Inserir jogo com preco igual a zero.
-- Esperado: INSERT aceito pelo banco (sem CHECK); a RN06 sera validada
-- mais tarde na camada Service da API.
START TRANSACTION;
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('TESTE_PRECO_ZERO', 'Registro temporario', 1990, 0.00,
     1, 1, UTC_TIMESTAMP(6), 1, 2);
-- Antes do ROLLBACK, confira que a linha foi inserida na transacao:
SELECT co_seq_jogo, no_jogo, vl_preco FROM tb_jogo
WHERE no_jogo = 'TESTE_PRECO_ZERO';
ROLLBACK;
-- Resultado observado:
-- Teste 5: Inserir jogo com preco igual a zero.
-- Resultado esperado: Aceitar
-- Resultado observado: Aceitou
-- Situação: Aprovado

-- VERIFICACAO FINAL: execute SOMENTE DEPOIS DE TODOS OS ROLLBACKS.
SELECT COUNT(*) AS total_plataformas FROM tb_plataforma; -- esperado: 6
SELECT COUNT(*) AS total_jogos FROM tb_jogo;            -- esperado: 20
SELECT COUNT(*) AS registros_teste_restantes FROM tb_jogo
WHERE no_jogo IN ('TESTE_FK_1452', 'TESTE_PRECO_ZERO'); -- esperado: 0
SELECT COUNT(*) AS total_usuarios FROM tb_usuario;       -- esperado: 0
