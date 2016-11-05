﻿CREATE TABLE [dbo].[GastoPosicao] (
    [ID_GASTO_POSICAO] INT             IDENTITY (1, 1) NOT NULL,
    [NR_LATITUDE]      NUMERIC (12, 8) NOT NULL,
    [NR_LONGITUDE]     NUMERIC (18, 8) NOT NULL,
    [ID_GASTO]         INT             NOT NULL,
    [ID_CIDADE]        INT             NULL,
    CONSTRAINT [PK12] PRIMARY KEY NONCLUSTERED ([ID_GASTO_POSICAO] ASC),
    CONSTRAINT [RefCidade18] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefGasto17] FOREIGN KEY ([ID_GASTO]) REFERENCES [dbo].[Gasto] ([ID_GASTO])
);

