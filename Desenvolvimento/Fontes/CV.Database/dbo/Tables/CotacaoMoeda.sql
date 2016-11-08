CREATE TABLE [dbo].[CotacaoMoeda] (
    [ID_COTACAO_MOEDA] INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]        INT             NOT NULL,
    [CD_MOEDA]         INT             NOT NULL,
    [DT_COTACAO]       DATETIME        NOT NULL,
    [VL_COTACAO]       NUMERIC (18, 6) NOT NULL,
    [DT_ATUALIZACAO]   DATETIME        NOT NULL,
    [DT_EXCLUSAO]      DATETIME        NULL,
    CONSTRAINT [PK47] PRIMARY KEY NONCLUSTERED ([ID_COTACAO_MOEDA] ASC),
    CONSTRAINT [RefViagem83] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

