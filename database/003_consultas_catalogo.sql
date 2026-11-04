-- RetroGames | 003_consultas_catalogo.sql
-- Etapa 5: consultas SQL didaticas sobre os dados das etapas 3 e 4.
-- Requer MySQL 8 e os scripts 001 e 002 previamente executados.
-- Estas consultas sao para execucao direta no Workbench.
-- Na API, o Repository deve consultar apenas uma tabela por comando,
-- conforme a regra didatica da especificacao (secoes 3.1 e 3.3).

USE retrogames;

-- 1. Catalogo: apenas jogos ativos, com plataforma e condicao.
SELECT
    j.co_seq_jogo AS id_jogo,
    j.no_jogo AS jogo,
    p.no_plataforma AS plataforma,
    c.ds_condicao_jogo AS condicao
FROM tb_jogo AS j
INNER JOIN tb_plataforma AS p
    ON p.co_seq_plataforma = j.co_plataforma
INNER JOIN tb_condicao_jogo AS c
    ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = 1
ORDER BY j.no_jogo ASC, j.co_seq_jogo ASC;

-- 2. Generos de um jogo informado.
-- Exemplo: 14 = Super Mario 64. Altere o ID para consultar outro jogo.
SET @id_jogo = 14;
SELECT
    j.no_jogo AS jogo,
    g.no_genero AS genero
FROM tb_jogo AS j
INNER JOIN tb_jogo_genero AS jg
    ON jg.co_jogo = j.co_seq_jogo
INNER JOIN tb_genero AS g
    ON g.co_seq_genero = jg.co_genero
WHERE j.co_seq_jogo = @id_jogo
ORDER BY g.no_genero ASC;

-- 3. Jogos de um genero informado.
-- Exemplo: 1 = Plataforma. Altere o ID para consultar outro genero.
-- Nao filtra por st_ativo: o exercicio nao exige apenas jogos ativos.
SET @id_genero = 1;
SELECT
    g.no_genero AS genero,
    j.co_seq_jogo AS id_jogo,
    j.no_jogo AS jogo
FROM tb_genero AS g
INNER JOIN tb_jogo_genero AS jg
    ON jg.co_genero = g.co_seq_genero
INNER JOIN tb_jogo AS j
    ON j.co_seq_jogo = jg.co_jogo
WHERE g.co_seq_genero = @id_genero
ORDER BY j.no_jogo ASC, j.co_seq_jogo ASC;

-- 4. Quantidade de jogos por plataforma, inclusive plataformas sem jogos.
-- COUNT(j.co_seq_jogo) produz zero quando nao ha jogo associado.
-- Conta tambem jogos inativos, pois o exercicio nao pede filtro por situacao.
SELECT
    p.co_seq_plataforma AS id_plataforma,
    p.no_plataforma AS plataforma,
    COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j
    ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma ASC;

-- 5. Busca por parte do nome, cinco jogos por pagina.
-- Exemplo: buscar nomes contendo 'a' (troque o termo quando quiser).
-- Pagina 1: LIMIT 5 OFFSET 0
-- Pagina 2: LIMIT 5 OFFSET 5
-- Pagina 3: LIMIT 5 OFFSET 10
-- Formula: OFFSET = (numero_da_pagina - 1) * 5.
-- A busca inclui ativos e inativos porque nao foi pedido filtro por situacao.
SET @termo_busca = 'a';
SELECT
    j.co_seq_jogo AS id_jogo,
    j.no_jogo AS jogo
FROM tb_jogo AS j
WHERE j.no_jogo LIKE CONCAT('%', @termo_busca, '%')
ORDER BY j.no_jogo ASC, j.co_seq_jogo ASC
LIMIT 5 OFFSET 0;
