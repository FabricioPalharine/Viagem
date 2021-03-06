﻿CREATE TABLE [dbo].[Cidade] (
    [ID_CIDADE] INT           IDENTITY (1, 1) NOT NULL,
    [ID_PAIS]   INT           NOT NULL,
    [NM_CIDADE] VARCHAR (100) NOT NULL,
    [NM_ESTADO] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK10] PRIMARY KEY NONCLUSTERED ([ID_CIDADE] ASC),
    CONSTRAINT [RefPais82] FOREIGN KEY ([ID_PAIS]) REFERENCES [dbo].[Pais] ([ID_PAIS])
);

