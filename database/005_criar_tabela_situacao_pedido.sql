
-- RetroGames | 005_criar_tabela_situacao_pedido.sql
-- Criação das tabelas do módulo de pedidos.

USE retrogames;

-- Situações fixas dos pedidos
CREATE TABLE tb_situacao_pedido (
    co_situacao_pedido INT NOT NULL,
    ds_situacao_pedido VARCHAR(60) NOT NULL,
    PRIMARY KEY (co_situacao_pedido)
) ENGINE=InnoDB;

INSERT INTO tb_situacao_pedido
    (co_situacao_pedido, ds_situacao_pedido)
VALUES
    (1, 'Pendente'),
    (2, 'Pago'),
    (3, 'Enviado'),
    (4, 'Cancelado');

-- Pedidos realizados pelos usuários
CREATE TABLE tb_pedido (
    co_seq_pedido INT NOT NULL AUTO_INCREMENT,
    dt_pedido DATETIME(6) NOT NULL,
    vl_total DECIMAL(10,2) NOT NULL,
    co_usuario INT NOT NULL,
    co_situacao_pedido INT NOT NULL,
    PRIMARY KEY (co_seq_pedido),
    CONSTRAINT fk_pedido_usuario
        FOREIGN KEY (co_usuario)
        REFERENCES tb_usuario (co_seq_usuario),
    CONSTRAINT fk_pedido_situacao
        FOREIGN KEY (co_situacao_pedido)
        REFERENCES tb_situacao_pedido (co_situacao_pedido)
) ENGINE=InnoDB;

-- Jogos e quantidades de cada pedido
CREATE TABLE tb_item_pedido (
    co_seq_item_pedido INT NOT NULL AUTO_INCREMENT,
    qt_item INT NOT NULL,
    vl_unitario DECIMAL(10,2) NOT NULL,
    co_pedido INT NOT NULL,
    co_jogo INT NOT NULL,
    PRIMARY KEY (co_seq_item_pedido),
    CONSTRAINT uq_item_pedido_jogo
        UNIQUE (co_pedido, co_jogo),
    CONSTRAINT fk_item_pedido_pedido
        FOREIGN KEY (co_pedido)
        REFERENCES tb_pedido (co_seq_pedido),
    CONSTRAINT fk_item_pedido_jogo
        FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo)
) ENGINE=InnoDB;