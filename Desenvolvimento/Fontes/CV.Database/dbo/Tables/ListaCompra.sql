CREATE TABLE [dbo].[ListaCompra] (
    [ID_LISTA_COMPRA]   INT            IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]         INT            NOT NULL,
    [ID_USUARIO]        INT            NOT NULL,
    [ID_USUARIO_PEDIDO] INT            NULL,
    [DS_ITEM]           VARCHAR (50)   NOT NULL,
    [DS_MARCA]          VARCHAR (50)   NOT NULL,
    [VL_MAXIMO]         NUMERIC (9, 2) NULL,
    [CD_MOEDA]          INT            NOT NULL,
    [FL_REEMBOLSAVEL]   BIT            NOT NULL,
    [FL_COMPRADO]       BIT            NOT NULL,
    [DS_DESTINATARIO]   VARCHAR (200)  NULL,
    CONSTRAINT [PK35] PRIMARY KEY NONCLUSTERED ([ID_LISTA_COMPRA] ASC),
    CONSTRAINT [RefUsuario56] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefUsuario57] FOREIGN KEY ([ID_USUARIO_PEDIDO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefViagem55] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

