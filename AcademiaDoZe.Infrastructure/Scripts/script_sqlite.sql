PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS tb_logradouro (
    id_logradouro INTEGER PRIMARY KEY AUTOINCREMENT,
    cep TEXT NOT NULL UNIQUE,
    nome TEXT NOT NULL,
    bairro TEXT NOT NULL,
    cidade TEXT NOT NULL,
    estado TEXT NOT NULL,
    pais TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS tb_colaborador (
    id_colaborador INTEGER PRIMARY KEY AUTOINCREMENT,
    cpf TEXT NOT NULL UNIQUE,
    nome TEXT NOT NULL,
    nascimento TEXT NOT NULL,
    telefone TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    logradouro_id INTEGER NOT NULL,
    numero TEXT NOT NULL,
    complemento TEXT,
    senha TEXT NOT NULL,
    foto BLOB,
    admissao TEXT NOT NULL,
    tipo INTEGER NOT NULL,
    vinculo INTEGER NOT NULL,
    FOREIGN KEY (logradouro_id) REFERENCES tb_logradouro (id_logradouro)
);

CREATE TABLE IF NOT EXISTS tb_aluno (
    id_aluno INTEGER PRIMARY KEY AUTOINCREMENT,
    cpf TEXT NOT NULL UNIQUE,
    nome TEXT NOT NULL,
    nascimento TEXT NOT NULL,
    telefone TEXT NOT NULL,
    email TEXT NOT NULL UNIQUE,
    logradouro_id INTEGER NOT NULL,
    numero TEXT NOT NULL,
    complemento TEXT,
    senha TEXT NOT NULL,
    foto BLOB,
    FOREIGN KEY (logradouro_id) REFERENCES tb_logradouro (id_logradouro)
);

CREATE TABLE IF NOT EXISTS tb_matricula (
    id_matricula INTEGER PRIMARY KEY AUTOINCREMENT,
    aluno_id INTEGER NOT NULL,
    plano INTEGER NOT NULL,
    data_inicio TEXT NOT NULL,
    data_fim TEXT NOT NULL,
    objetivo TEXT NOT NULL,
    restricao_medica INTEGER NOT NULL,
    obs_restricao TEXT,
    laudo_medico BLOB,
    FOREIGN KEY (aluno_id) REFERENCES tb_aluno (id_aluno)
);