﻿CREATE TABLE [dbo].[AvaliacaoLoja] (
    [ID_AVALIACAO_LOJA] INT      IDENTITY (1, 1) NOT NULL,
    [ID_LOJA]           INT      NOT NULL,
    [ID_USUARIO]        INT      NOT NULL,
    [NR_NOTA]           INT      NULL,
    [DS_COMENTARIO]     VARCHAR(MAX)     NULL,
    [DT_ATUALIZACAO]    DATETIME NOT NULL,
    [DT_EXCLUSAO]       DATETIME NULL,
    CONSTRAINT [PK49] PRIMARY KEY CLUSTERED ([ID_AVALIACAO_LOJA] ASC),
    CONSTRAINT [RefLoja97] FOREIGN KEY ([ID_LOJA]) REFERENCES [dbo].[Loja] ([ID_LOJA]),
    CONSTRAINT [RefUsuario96] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO])
);

