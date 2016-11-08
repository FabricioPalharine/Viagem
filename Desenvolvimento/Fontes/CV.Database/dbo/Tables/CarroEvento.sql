CREATE TABLE [dbo].[CarroEvento] (
    [ID_CARRO_EVENTO] INT             IDENTITY (1, 1) NOT NULL,
    [ID_CARRO]        INT             NOT NULL,
    [ID_CIDADE]       INT             NOT NULL,
    [DT_EVENTO]       DATETIME        NOT NULL,
    [CD_TIPO_EVENTO]  INT             NOT NULL,
    [VL_ODOMETRO]     INT             NULL,
    [NR_LATITUDE]     NUMERIC (12, 8) NOT NULL,
    [NR_LONGITUDE]    NUMERIC (12, 8) NOT NULL,
    [DT_ATUALIZACAO]  DATETIME        NOT NULL,
    [DT_EXCLUSAO]     DATETIME        NULL,
    CONSTRAINT [PK45] PRIMARY KEY NONCLUSTERED ([ID_CARRO_EVENTO] ASC),
    CONSTRAINT [RefCarro80] FOREIGN KEY ([ID_CARRO]) REFERENCES [dbo].[Carro] ([ID_CARRO]),
    CONSTRAINT [RefCidade81] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE])
);

