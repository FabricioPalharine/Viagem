CREATE TABLE [dbo].[Foto] (
    [ID_FOTO]           INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]         INT             NOT NULL,
    [ID_USUARIO]        INT             NULL,
    [ID_CIDADE]         INT             NULL,
    [DS_COMENTARIO]     VARCHAR(MAX)            NULL,
    [NR_LATITUDE]       NUMERIC (12, 8) NULL,
    [NR_LONGITUDE]      NUMERIC (12, 8) NULL,
    [DT_FOTO]           DATETIME        NOT NULL,
    [DS_LINK_THUMBNAIL] VARCHAR (MAX)   NOT NULL,
    [DS_LINK_FOTO]      VARCHAR (MAX)   NOT NULL,
    [CD_CODIGO_DRIVE]   VARCHAR (500)    NOT NULL,
    [FL_VIDEO]          BIT             NOT NULL,
    [CD_TIPO_ARQUIVO]   VARCHAR (20)    NOT NULL,
    [DT_ATUALIZACAO]    DATETIME        NOT NULL,
    [DT_EXCLUSAO]       DATETIME        NULL,
    CONSTRAINT [PK40] PRIMARY KEY NONCLUSTERED ([ID_FOTO] ASC),
    CONSTRAINT [RefCidade73] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefUsuario69] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefViagem68] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

