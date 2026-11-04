-- RetroGames | 001_criar_tabelas.sql
-- MySQL 8 | Especificacao tecnica, secoes 4.1 a 4.3
-- Os tamanhos VARCHAR e a precisao do DATETIME sao escolhas de implementacao:
-- o dicionario de dados informa nomes, obrigatoriedade e relacionamentos,
-- mas nao fixa os limites de caracteres nem a precisao de data/hora.
-- Banco chamado 'retrogames' por convencao; pode ser ajustado se necessario.
-- Datas em dt_cadastro devem ser gravadas em UTC pela aplicacao.

CREATE DATABASE IF NOT EXISTS retrogames
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE retrogames;

-- Tabelas de dominio (codigos fixos, sem AUTO_INCREMENT)
CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo INT NOT NULL,
    ds_condicao_jogo VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_condicao_jogo)
) ENGINE=InnoDB;

CREATE TABLE tb_perfil (
    co_perfil INT NOT NULL,
    ds_perfil VARCHAR(60) NOT NULL,
    PRIMARY KEY (co_perfil)
) ENGINE=InnoDB;

-- Tabelas de cadastro
CREATE TABLE tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(120) NOT NULL,
    no_fabricante VARCHAR(120) NOT NULL,
    nu_ano_lancamento INT NOT NULL,
    PRIMARY KEY (co_seq_plataforma),
    CONSTRAINT uq_plataforma_nome UNIQUE (no_plataforma)
) ENGINE=InnoDB;

CREATE TABLE tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(100) NOT NULL,
    PRIMARY KEY (co_seq_genero)
) ENGINE=InnoDB;

-- Jogos e relacionamentos
CREATE TABLE tb_jogo (
    co_seq_jogo INT NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(200) NOT NULL,
    ds_jogo TEXT NULL,
    nu_ano_lancamento INT NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT NOT NULL,
    st_ativo BOOLEAN NOT NULL,
    dt_cadastro DATETIME(6) NOT NULL,
    co_plataforma INT NOT NULL,
    co_condicao_jogo INT NOT NULL,
    PRIMARY KEY (co_seq_jogo),
    CONSTRAINT fk_jogo_plataforma
        FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao
        FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_condicao_jogo)
) ENGINE=InnoDB;

CREATE TABLE tb_jogo_genero (
    co_jogo INT NOT NULL,
    co_genero INT NOT NULL,
    PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo
        FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero
        FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
) ENGINE=InnoDB;

CREATE TABLE tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(150) NOT NULL,
    ds_email VARCHAR(254) NOT NULL,
    ds_senha_hash VARCHAR(512) NOT NULL,
    st_ativo BOOLEAN NOT NULL,
    dt_cadastro DATETIME(6) NOT NULL,
    co_perfil INT NOT NULL,
    PRIMARY KEY (co_seq_usuario),
    CONSTRAINT uq_usuario_email UNIQUE (ds_email),
    CONSTRAINT fk_usuario_perfil
        FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_perfil)
) ENGINE=InnoDB;
