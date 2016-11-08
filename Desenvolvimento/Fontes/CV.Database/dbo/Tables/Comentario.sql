﻿CREATE TABLE [dbo].[Comentario] (
    [ID_COMENTARIO]  INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]      INT             NOT NULL,
    [ID_CIDADE]      INT             NULL,
    [NR_LATITUDE]    NUMERIC (12, 8) NOT NULL,
    [NR_LONGITUDE]   NUMERIC (12, 8) NOT NULL,
    [DS_COMENTARIO]  TEXT            NOT NULL,
    [DT_ATUALIZACAO] DATETIME        NOT NULL,
    [DT_EXCLUSAO]    DATETIME        NULL,
    CONSTRAINT [PK13] PRIMARY KEY NONCLUSTERED ([ID_COMENTARIO] ASC),
    CONSTRAINT [RefCidade20] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefViagem19] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

