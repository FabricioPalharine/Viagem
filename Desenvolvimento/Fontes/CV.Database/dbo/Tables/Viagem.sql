CREATE TABLE [dbo].[Viagem] (
    [ID_VIAGEM]        INT            IDENTITY (1, 1) NOT NULL,
    [ID_USUARIO]       INT            NOT NULL,
    [NM_VIAGEM]        VARCHAR (200)  NOT NULL,
    [DT_INICIO]        DATETIME       NULL,
    [DT_FIM]           DATETIME       NULL,
    [FL_ABERTO]        BIT            NOT NULL,
    [FL_METRICA]       BIT            NOT NULL,
    [NR_PARTICIPANTES] INT            NOT NULL,
    [FL_PUBLICA_GASTO] BIT            NOT NULL,
    [PC_IOF]           NUMERIC (9, 4) NULL,
    [CD_MOEDA]         INT            NOT NULL,
    [DT_ATUALIZACAO]   DATETIME       NOT NULL,
    [DT_EXCLUSAO]      DATETIME       NULL,
    [CD_ALBUM] VARCHAR(500) NULL, 
    [CD_SHARE_TOKEN] VARCHAR(500) NULL, 
    CONSTRAINT [PK4] PRIMARY KEY NONCLUSTERED ([ID_VIAGEM] ASC),
    CONSTRAINT [RefUsuario5] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO])
);

