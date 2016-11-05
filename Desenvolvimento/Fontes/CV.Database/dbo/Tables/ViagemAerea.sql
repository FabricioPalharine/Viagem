﻿CREATE TABLE [dbo].[ViagemAerea] (
    [ID_VIAGEM_AEREA]    INT          IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]          INT          NOT NULL,
    [DS_COMPANHIA_AEREA] VARCHAR (50) NOT NULL,
    [DT_PREVISTO]        DATETIME     NOT NULL,
    [DT_INICIO]          DATETIME     NULL,
    [DT_FIM]             DATETIME     NULL,
    CONSTRAINT [PK30] PRIMARY KEY NONCLUSTERED ([ID_VIAGEM_AEREA] ASC),
    CONSTRAINT [RefViagem48] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

