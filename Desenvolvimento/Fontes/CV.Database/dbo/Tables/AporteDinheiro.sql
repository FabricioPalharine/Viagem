CREATE TABLE [dbo].[AporteDinheiro] (
    [ID_APORTE_DINHEIRO] INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]          INT             NOT NULL,
    [ID_USUARIO]         INT             NOT NULL,
    [VL_QUANTIDADE]      NUMERIC (18, 2) NOT NULL,
    [CD_MOEDA]           INT             NULL,
    [DT_APORTE]          DATETIME        NULL,
    [VL_COTACAO]         NUMERIC (18, 6) NULL,
    [DT_ATUALIZACAO]     DATETIME        NOT NULL,
    [DT_EXCLUSAO]        DATETIME        NULL,
    CONSTRAINT [PK7] PRIMARY KEY NONCLUSTERED ([ID_APORTE_DINHEIRO] ASC),
    CONSTRAINT [RefUsuario10] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefViagem9] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

