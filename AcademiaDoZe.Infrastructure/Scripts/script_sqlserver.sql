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