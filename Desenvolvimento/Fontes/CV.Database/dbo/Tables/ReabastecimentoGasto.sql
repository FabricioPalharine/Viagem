﻿CREATE TABLE [dbo].[ReabastecimentoGasto] (
    [ID_REABASTECIMENTO_GASTO]   INT IDENTITY (1, 1) NOT NULL,
    [ID_GASTO]                   INT NOT NULL,
    [ID_ALUGUEL_REABASTECIMENTO] INT NOT NULL,
    CONSTRAINT [PK28] PRIMARY KEY NONCLUSTERED ([ID_REABASTECIMENTO_GASTO] ASC),
    CONSTRAINT [RefAluguelReabastecimento46] FOREIGN KEY ([ID_ALUGUEL_REABASTECIMENTO]) REFERENCES [dbo].[AluguelReabastecimento] ([ID_ALUGUEL_REABASTECIMENTO]),
    CONSTRAINT [RefGasto45] FOREIGN KEY ([ID_GASTO]) REFERENCES [dbo].[Gasto] ([ID_GASTO])
);

