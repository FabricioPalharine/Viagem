﻿CREATE TABLE [dbo].[HotelEvento] (
    [ID_HOTEL_EVENTO] INT      IDENTITY (1, 1) NOT NULL,
    [ID_HOTEL]        INT      NOT NULL,
    [ID_USUARIO]      INT      NOT NULL,
    [DT_ENTRADA]       DATETIME NOT NULL,
    [DT_SAIDA]  DATETIME      NULL,
    [DT_ATUALIZACAO]  DATETIME NOT NULL,
    [DT_EXCLUSAO] DATETIME NULL, 
    CONSTRAINT [PK51] PRIMARY KEY CLUSTERED ([ID_HOTEL_EVENTO] ASC),
    CONSTRAINT [RefHotel99] FOREIGN KEY ([ID_HOTEL]) REFERENCES [dbo].[Hotel] ([ID_HOTEL]),
    CONSTRAINT [RefUsuario98] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO])
);

