﻿CREATE TABLE [dbo].[ParticipanteViagem] (
    [ID_PARTICIPANTE_VIAGEM] INT           IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]              INT           NOT NULL,
    [ID_USUARIO]             INT           NULL,
    [DS_EMAIL]               VARCHAR (200) NOT NULL,
    [CD_STATUS]              INT           NOT NULL,
    CONSTRAINT [PK5] PRIMARY KEY NONCLUSTERED ([ID_PARTICIPANTE_VIAGEM] ASC),
    CONSTRAINT [RefUsuario7] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefViagem6] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

