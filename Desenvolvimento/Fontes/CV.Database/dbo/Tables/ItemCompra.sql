﻿CREATE TABLE [dbo].[ItemCompra] (
    [ID_ITEM_COMPRA]  INT            IDENTITY (1, 1) NOT NULL,
    [ID_GASTO_COMPRA] INT            NOT NULL,
    [ID_LISTA_COMPRA] INT            NULL,
    [DS_ITEM]         VARCHAR (50)   NOT NULL,
    [DS_MARCA]        VARCHAR (50)   NOT NULL,
    [VL_ITEM]         NUMERIC (9, 2) NOT NULL,
    [FL_REEMBOLSAVEL] BIT            NOT NULL,
    [NM_DESTINATARIO] VARCHAR (50)   NULL,
    CONSTRAINT [PK38] PRIMARY KEY NONCLUSTERED ([ID_ITEM_COMPRA] ASC),
    CONSTRAINT [RefGastoCompra61] FOREIGN KEY ([ID_GASTO_COMPRA]) REFERENCES [dbo].[GastoCompra] ([ID_GASTO_COMPRA]),
    CONSTRAINT [RefListaCompra62] FOREIGN KEY ([ID_LISTA_COMPRA]) REFERENCES [dbo].[ListaCompra] ([ID_LISTA_COMPRA])
);

