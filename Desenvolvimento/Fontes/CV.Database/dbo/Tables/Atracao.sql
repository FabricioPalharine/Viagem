﻿CREATE TABLE [dbo].[Atracao] (
    [ID_ATRACAO]      INT             IDENTITY (1, 1) NOT NULL,
    [ID_ATRACAO_PAI]  INT             NULL,
    [ID_VIAGEM]       INT             NOT NULL,
    [ID_CIDADE]       INT             NULL,
    [NM_ATRACAO]      VARCHAR (200)   NOT NULL,
    [CD_PLACE]        VARCHAR (50)    NULL,
    [NR_LATITUDE]     NUMERIC (12, 8) NULL,
    [NR_LONGITUDE]    NUMERIC (12, 8) NULL,
    [DT_CHEGADA]      DATETIME        NULL,
    [DT_PARTIDA]      DATETIME        NULL,
    [DS_TIPO_ATRACAO] VARCHAR (150)    NULL,
    [DT_ATUALIZACAO]  DATETIME        NOT NULL,
    [DT_EXCLUSAO]     DATETIME        NULL,
    [VL_DISTANCIA] NUMERIC(16, 4) NULL, 
    CONSTRAINT [PK39] PRIMARY KEY NONCLUSTERED ([ID_ATRACAO] ASC),
    CONSTRAINT [RefAtracao72] FOREIGN KEY ([ID_ATRACAO_PAI]) REFERENCES [dbo].[Atracao] ([ID_ATRACAO]),
    CONSTRAINT [RefCidade66] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefViagem65] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

