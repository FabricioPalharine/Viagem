﻿CREATE TABLE [dbo].[ViagemAerea] (
    [ID_VIAGEM_AEREA]    INT          IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]          INT          NOT NULL,
    [DS_COMPANHIA_AEREA] VARCHAR (50) NOT NULL,
    [DT_PREVISTO]        DATETIME     NOT NULL,
    [DT_ATUALIZACAO]     DATETIME     NOT NULL,
    [DT_EXCLUSAO]        DATETIME     NULL,
    [CD_TIPO] INT NOT NULL, 
    [DS_DESCRICAO] VARCHAR(500) NULL, 
    CONSTRAINT [PK30] PRIMARY KEY NONCLUSTERED ([ID_VIAGEM_AEREA] ASC),
    CONSTRAINT [RefViagem48] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

