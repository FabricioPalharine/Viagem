﻿CREATE TABLE [dbo].[Carro] (
    [ID_CARRO]       INT          IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]      INT          NOT NULL,
    [DS_LOCADORA]    VARCHAR (50) NULL,
    [DS_CARRO]       VARCHAR (50) NOT NULL,
    [FL_KM]          BIT          NOT NULL,
    [FL_ALUGADO]     BIT          NOT NULL,
    [DT_ATUALIZACAO] DATETIME     NOT NULL,
    [DT_EXCLUSAO]    DATETIME     NULL,
    [DT_RETIRADA] DATETIME NULL, 
    [DT_DEVOLUCAO] DATETIME NULL, 
    [DS_DESCRICAO] VARCHAR(100) NOT NULL, 
    [ID_CARRO_EVENTO_RETIRADA] INT NULL, 
    [ID_CARRO_EVENTO_DEVOLUCAO] INT NULL, 
    CONSTRAINT [PK24] PRIMARY KEY NONCLUSTERED ([ID_CARRO] ASC),
    CONSTRAINT [RefViagem38] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM]),
	CONSTRAINT FK_Carro1 FOREIGN KEY (ID_CARRO_EVENTO_RETIRADA) REFERENCES dbo.CarroEvento(ID_CARRO_EVENTO),
	CONSTRAINT FK_Carro2 FOREIGN KEY (ID_CARRO_EVENTO_DEVOLUCAO) REFERENCES dbo.CarroEvento(ID_CARRO_EVENTO),

);

