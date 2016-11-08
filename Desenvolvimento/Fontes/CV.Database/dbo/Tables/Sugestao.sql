CREATE TABLE [dbo].[Sugestao] (
    [ID_SUGESTAO]    INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]      INT             NOT NULL,
    [ID_CIDADE]      INT             NOT NULL,
    [ID_USUARIO]     INT             NOT NULL,
    [NM_LOCAL]       VARCHAR (100)   NOT NULL,
    [NR_LATITUDE]    NUMERIC (12, 8) NOT NULL,
    [NR_LONGITUDE]   NUMERIC (12, 8) NOT NULL,
    [DS_COMENTARIO]  TEXT            NULL,
    [CD_STATUS]      INT             NOT NULL,
    [DS_TIPO]        VARCHAR (50)    NULL,
    [DT_ATUALIZACAO] DATETIME        NOT NULL,
    [DT_EXCLUSAO]    DATETIME        NULL,
    CONSTRAINT [PK15] PRIMARY KEY NONCLUSTERED ([ID_SUGESTAO] ASC),
    CONSTRAINT [RefCidade24] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefUsuario23] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefViagem22] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

