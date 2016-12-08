﻿CREATE TABLE [dbo].[RequisicaoAmizade] (
    [ID_REQUISICAO_AMIZADE]  INT           IDENTITY (1, 1) NOT NULL,
    [ID_USUARIO]             INT           NOT NULL,
    [ID_USUARIO_REQUISITADO] INT           NULL,
    [DS_EMAIL]               VARCHAR (200) NOT NULL,
    [CD_STATUS]              INT           NOT NULL,
    CONSTRAINT [PK2] PRIMARY KEY NONCLUSTERED ([ID_REQUISICAO_AMIZADE] ASC),
    CONSTRAINT [RefUsuario1] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefUsuario2] FOREIGN KEY ([ID_USUARIO_REQUISITADO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO])
);

