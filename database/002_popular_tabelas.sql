-- RetroGames | 002_popular_tabelas.sql
-- Dados iniciais do catalogo, conforme a etapa 4 e a especificacao, secao 4.2.
-- Executar UMA VEZ, depois de 001_criar_tabelas.sql, em um banco vazio.
-- IDs 1/2/3 de condicao e 1/2 de perfil sao fixos por requisito.
-- Os demais IDs abaixo sao identificadores de exemplo usados para deixar
-- as associacoes reproduziveis; na API, os cadastros usarao AUTO_INCREMENT.
-- Valores de preco/estoque/condicao sao ficticios para fins didaticos.
-- Nenhum usuario e criado aqui: o administrador sera criado pela API.

USE retrogames;

START TRANSACTION;

-- 1. Tabelas de dominio: codigos fixos.
INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

-- 2. Seis plataformas de videogames retro.
INSERT INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    (1, 'Nintendo Entertainment System (NES)', 'Nintendo', 1985),
    (2, 'Super Nintendo (SNES)', 'Nintendo', 1990),
    (3, 'Mega Drive', 'Sega', 1988),
    (4, 'Game Boy', 'Nintendo', 1989),
    (5, 'Nintendo 64', 'Nintendo', 1996),
    (6, 'PlayStation', 'Sony', 1994);

-- 3. Oito generos para classificar os jogos.
INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
    (1, 'Plataforma'),
    (2, 'Ação'),
    (3, 'Aventura'),
    (4, 'RPG'),
    (5, 'Corrida'),
    (6, 'Luta'),
    (7, 'Quebra-cabeça'),
    (8, 'Esporte');

-- 4. Vinte jogos reais. Ano = ano de lancamento original ou da versao indicada.
-- st_ativo: 1 = ativo; 0 = inativo. qt_estoque: 0 = esgotado.
-- dt_cadastro e gravado em UTC conforme o padrao do projeto.
INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
     qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (1,  'Super Mario Bros.', 'Jogo de plataforma para NES.', 1985, 179.90, 2, 1, UTC_TIMESTAMP(6), 1, 2),
    (2,  'The Legend of Zelda', 'Aventura original da serie Zelda.', 1986, 249.90, 1, 1, UTC_TIMESTAMP(6), 1, 2),
    (3,  'Metroid', 'Exploracao e acao no planeta Zebes.', 1986, 239.90, 0, 1, UTC_TIMESTAMP(6), 1, 2),
    (4,  'Super Mario World', 'Aventura de Mario e Yoshi no SNES.', 1990, 199.90, 4, 1, UTC_TIMESTAMP(6), 2, 2),
    (5,  'The Legend of Zelda: A Link to the Past', 'Aventura de Link em Hyrule.', 1991, 299.90, 2, 1, UTC_TIMESTAMP(6), 2, 2),
    (6,  'Chrono Trigger', 'RPG de viagens no tempo.', 1995, 849.90, 1, 1, UTC_TIMESTAMP(6), 2, 3),
    (7,  'Donkey Kong Country', 'Aventura de plataforma no SNES.', 1994, 189.90, 3, 1, UTC_TIMESTAMP(6), 2, 2),
    (8,  'Sonic the Hedgehog', 'O primeiro jogo de Sonic no Mega Drive.', 1991, 149.90, 5, 1, UTC_TIMESTAMP(6), 3, 2),
    (9,  'Streets of Rage 2', 'Acao cooperativa nas ruas da cidade.', 1992, 279.90, 1, 1, UTC_TIMESTAMP(6), 3, 2),
    (10, 'Mortal Kombat II', 'Jogo de luta na versao Mega Drive.', 1994, 169.90, 2, 1, UTC_TIMESTAMP(6), 3, 2),
    (11, 'Tetris', 'Classico jogo de encaixar pecas no Game Boy.', 1989, 129.90, 4, 1, UTC_TIMESTAMP(6), 4, 2),
    (12, 'Pokemon Red Version', 'RPG de captura e batalhas de monstros.', 1996, 459.90, 0, 1, UTC_TIMESTAMP(6), 4, 2),
    (13, 'Super Mario Land', 'Mario em sua aventura no Game Boy.', 1989, 169.90, 2, 1, UTC_TIMESTAMP(6), 4, 2),
    (14, 'Super Mario 64', 'Aventura de plataforma em tres dimensoes.', 1996, 279.90, 2, 1, UTC_TIMESTAMP(6), 5, 2),
    (15, 'Mario Kart 64', 'Corridas com personagens da Nintendo.', 1996, 349.90, 3, 1, UTC_TIMESTAMP(6), 5, 2),
    (16, 'GoldenEye 007', 'Jogo de acao para Nintendo 64.', 1997, 229.90, 1, 0, UTC_TIMESTAMP(6), 5, 2),
    (17, 'The Legend of Zelda: Ocarina of Time', 'Aventura tridimensional de Link.', 1998, 599.90, 1, 1, UTC_TIMESTAMP(6), 5, 3),
    (18, 'Crash Bandicoot', 'Jogo de plataforma para PlayStation.', 1996, 169.90, 3, 1, UTC_TIMESTAMP(6), 6, 2),
    (19, 'Gran Turismo', 'Simulador de corrida para PlayStation.', 1997, 139.90, 2, 1, UTC_TIMESTAMP(6), 6, 2),
    (20, 'FIFA 99', 'Jogo de futebol para PlayStation.', 1998, 79.90, 0, 1, UTC_TIMESTAMP(6), 6, 2);

-- 5. Associacoes entre jogos e generos: de um a tres generos por jogo.
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
    (1, 1), (1, 2),
    (2, 2), (2, 3),
    (3, 1), (3, 2),
    (4, 1), (4, 2),
    (5, 2), (5, 3),
    (6, 3), (6, 4),
    (7, 1), (7, 2),
    (8, 1), (8, 2),
    (9, 2), (9, 6),
    (10, 6),
    (11, 7),
    (12, 3), (12, 4),
    (13, 1),
    (14, 1), (14, 2), (14, 3),
    (15, 5),
    (16, 2),
    (17, 3), (17, 4),
    (18, 1), (18, 2),
    (19, 5),
    (20, 8);

COMMIT;

-- Consultas para conferir os dados apos a importacao.
SELECT 'condicoes' AS tabela, COUNT(*) AS quantidade FROM tb_condicao_jogo
UNION ALL SELECT 'perfis', COUNT(*) FROM tb_perfil
UNION ALL SELECT 'plataformas', COUNT(*) FROM tb_plataforma
UNION ALL SELECT 'generos', COUNT(*) FROM tb_genero
UNION ALL SELECT 'jogos', COUNT(*) FROM tb_jogo
UNION ALL SELECT 'jogos_generos', COUNT(*) FROM tb_jogo_genero
UNION ALL SELECT 'usuarios', COUNT(*) FROM tb_usuario;

SELECT
    COUNT(*) AS total_jogos,
    SUM(st_ativo = 0) AS jogos_inativos,
    SUM(qt_estoque = 0) AS jogos_sem_estoque
FROM tb_jogo;

SELECT co_jogo, COUNT(*) AS total_generos
FROM tb_jogo_genero
GROUP BY co_jogo
ORDER BY co_jogo;
