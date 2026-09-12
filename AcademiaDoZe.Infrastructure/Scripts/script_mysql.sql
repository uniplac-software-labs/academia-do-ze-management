CREATE TABLE IF NOT EXISTS tb_logradouro (
    id_logradouro INT AUTO_INCREMENT PRIMARY KEY,
    cep VARCHAR(8) NOT NULL UNIQUE,
    nome VARCHAR(150) NOT NULL,
    bairro VARCHAR(100) NOT NULL,
    cidade VARCHAR(100) NOT NULL,
    estado VARCHAR(2) NOT NULL,
    pais VARCHAR(50) NOT NULL
) ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS tb_colaborador (
    id_colaborador INT AUTO_INCREMENT PRIMARY KEY,
    cpf VARCHAR(11) NOT NULL UNIQUE,
    nome VARCHAR(150) NOT NULL,
    nascimento DATE NOT NULL,
    telefone VARCHAR(11) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    logradouro_id INT NOT NULL,
    numero VARCHAR(10) NOT NULL,
    complemento VARCHAR(100) NULL,
    senha VARCHAR(100) NOT NULL,
    foto LONGBLOB NULL,
    admissao DATE NOT NULL,
    tipo INT NOT NULL,
    vinculo INT NOT NULL,
    CONSTRAINT FK_colaborador_logradouro FOREIGN KEY (logradouro_id) REFERENCES tb_logradouro (id_logradouro)
) ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS tb_aluno (
    id_aluno INT AUTO_INCREMENT PRIMARY KEY,
    cpf VARCHAR(11) NOT NULL UNIQUE,
    nome VARCHAR(150) NOT NULL,
    nascimento DATE NOT NULL,
    telefone VARCHAR(11) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    logradouro_id INT NOT NULL,
    numero VARCHAR(10) NOT NULL,
    complemento VARCHAR(100) NULL,
    senha VARCHAR(100) NOT NULL,
    foto LONGBLOB NULL,
    CONSTRAINT FK_aluno_logradouro FOREIGN KEY (logradouro_id) REFERENCES tb_logradouro (id_logradouro)
) ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS tb_matricula (
    id_matricula INT AUTO_INCREMENT PRIMARY KEY,
    aluno_id INT NOT NULL,
    plano INT NOT NULL,
    data_inicio DATE NOT NULL,
    data_fim DATE NOT NULL,
    objetivo VARCHAR(200) NOT NULL,
    restricao_medica INT NOT NULL,
    obs_restricao VARCHAR(300) NULL,
    laudo_medico LONGBLOB NULL,
    CONSTRAINT FK_matricula_aluno FOREIGN KEY (aluno_id) REFERENCES tb_aluno (id_aluno)
) ENGINE = InnoDB;