﻿CREATE TABLE [dbo].[GastoViagemAerea] (
    [ID_GASTO_VIAGEM_AEREA] INT      IDENTITY (1, 1) NOT NULL,
    [ID_GASTO]              INT      NOT NULL,
    [ID_VIAGEM_AEREA]       INT      NOT NULL,
    [DT_ATUALIZACAO]        DATETIME NOT NULL,
    [DT_EXCLUSAO]           DATETIME NULL,
    CONSTRAINT [PK31] PRIMARY KEY NONCLUSTERED ([ID_GASTO_VIAGEM_AEREA] ASC),
    CONSTRAINT [RefGasto49] FOREIGN KEY ([ID_GASTO]) REFERENCES [dbo].[Gasto] ([ID_GASTO]),
    CONSTRAINT [RefViagemAerea50] FOREIGN KEY ([ID_VIAGEM_AEREA]) REFERENCES [dbo].[ViagemAerea] ([ID_VIAGEM_AEREA])
);

