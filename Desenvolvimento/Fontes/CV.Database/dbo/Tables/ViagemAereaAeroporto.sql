﻿CREATE TABLE [dbo].[ViagemAereaAeroporto] (
    [ID_VIAGEM_AEREA_AEROPORTO] INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM_AEREA]           INT             NOT NULL,
    [ID_CIDADE]                 INT             NOT NULL,
    [DS_AEROPORTO]              VARCHAR (200)   NOT NULL,
    [NR_LATITUDE]               NUMERIC (12, 8) NOT NULL,
    [NR_LONGITUDE]              NUMERIC (12, 8) NOT NULL,
    [CD_TIPO_PONTO]             INT             NOT NULL,
    [DT_CHEGADA]                DATETIME        NULL,
    [DT_SAIDA]                  DATETIME        NULL,
    [CD_PLACE]                  VARCHAR (50)    NOT NULL,
    CONSTRAINT [PK32] PRIMARY KEY NONCLUSTERED ([ID_VIAGEM_AEREA_AEROPORTO] ASC),
    CONSTRAINT [RefCidade52] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefViagemAerea51] FOREIGN KEY ([ID_VIAGEM_AEREA]) REFERENCES [dbo].[ViagemAerea] ([ID_VIAGEM_AEREA])
);

