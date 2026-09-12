IF NOT EXISTS (SELECT *
               FROM   sys.objects
               WHERE  object_id = OBJECT_ID(N'[dbo].[tb_logradouro]')
                      AND type IN (N'U'))
    BEGIN
        CREATE TABLE [dbo].[tb_logradouro] (
            [id_logradouro] INT           IDENTITY (1, 1) NOT NULL PRIMARY KEY,
            [cep]           VARCHAR (8)   NOT NULL UNIQUE,
            [nome]          VARCHAR (150) NOT NULL,
            [bairro]        VARCHAR (100) NOT NULL,
            [cidade]        VARCHAR (100) NOT NULL,
            [estado]        VARCHAR (2)   NOT NULL,
            [pais]          VARCHAR (50)  NOT NULL
        );
    END

IF NOT EXISTS (SELECT *
               FROM   sys.objects
               WHERE  object_id = OBJECT_ID(N'[dbo].[tb_colaborador]')
                      AND type IN (N'U'))
    BEGIN
        CREATE TABLE [dbo].[tb_colaborador] (
            [id_colaborador] INT             IDENTITY (1, 1) NOT NULL PRIMARY KEY,
            [cpf]            VARCHAR (11)    NOT NULL UNIQUE,
            [nome]           VARCHAR (150)   NOT NULL,
            [nascimento]     DATE            NOT NULL,
            [telefone]       VARCHAR (11)    NOT NULL,
            [email]          VARCHAR (150)   NOT NULL UNIQUE,
            [logradouro_id]  INT             NOT NULL,
            [numero]         VARCHAR (10)    NOT NULL,
            [complemento]    VARCHAR (100)   NULL,
            [senha]          VARCHAR (100)   NOT NULL,
            [foto]           VARBINARY (MAX) NULL,
            [admissao]       DATE            NOT NULL,
            [tipo]           INT             NOT NULL,
            [vinculo]        INT             NOT NULL,
            CONSTRAINT FK_colaborador_logradouro FOREIGN KEY (logradouro_id) REFERENCES [dbo].[tb_logradouro] ([id_logradouro])
        );
    END

IF NOT EXISTS (SELECT *
               FROM   sys.objects
               WHERE  object_id = OBJECT_ID(N'[dbo].[tb_aluno]')
                      AND type IN (N'U'))
    BEGIN
        CREATE TABLE [dbo].[tb_aluno] (
            [id_aluno]      INT             IDENTITY (1, 1) NOT NULL PRIMARY KEY,
            [cpf]           VARCHAR (11)    NOT NULL UNIQUE,
            [nome]          VARCHAR (150)   NOT NULL,
            [nascimento]    DATE            NOT NULL,
            [telefone]      VARCHAR (11)    NOT NULL,
            [email]         VARCHAR (150)   NOT NULL UNIQUE,
            [logradouro_id] INT             NOT NULL,
            [numero]        VARCHAR (10)    NOT NULL,
            [complemento]   VARCHAR (100)   NULL,
            [senha]         VARCHAR (100)   NOT NULL,
            [foto]          VARBINARY (MAX) NULL,
            CONSTRAINT FK_aluno_logradouro FOREIGN KEY (logradouro_id) REFERENCES [dbo].[tb_logradouro] ([id_logradouro])
        );
    END

IF NOT EXISTS (SELECT *
               FROM   sys.objects
               WHERE  object_id = OBJECT_ID(N'[dbo].[tb_matricula]')
                      AND type IN (N'U'))
    BEGIN
        CREATE TABLE [dbo].[tb_matricula] (
            [id_matricula]     INT             IDENTITY (1, 1) NOT NULL PRIMARY KEY,
            [aluno_id]         INT             NOT NULL,
            [plano]            INT             NOT NULL,
            [data_inicio]      DATE            NOT NULL,
            [data_fim]         DATE            NOT NULL,
            [objetivo]         VARCHAR (200)   NOT NULL,
            [restricao_medica] INT             NOT NULL,
            [obs_restricao]    VARCHAR (300)   NULL,
            [laudo_medico]     VARBINARY (MAX) NULL,
            CONSTRAINT FK_matricula_aluno FOREIGN KEY (aluno_id) REFERENCES [dbo].[tb_aluno] ([id_aluno])
        );
    END