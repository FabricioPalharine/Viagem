﻿CREATE TABLE [dbo].[HotelAvaliacao] (
    [ID_HOTEL_AVALIACAO] INT  IDENTITY (1, 1) NOT NULL,
    [ID_USUARIO]         INT  NOT NULL,
    [ID_HOTEL]           INT  NOT NULL,
    [NR_NOTA]            INT  NOT NULL,
    [DS_COMENTARIO]      TEXT NULL,
    CONSTRAINT [PK19] PRIMARY KEY NONCLUSTERED ([ID_HOTEL_AVALIACAO] ASC),
    CONSTRAINT [RefHotel32] FOREIGN KEY ([ID_HOTEL]) REFERENCES [dbo].[Hotel] ([ID_HOTEL]),
    CONSTRAINT [RefUsuario29] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO])
);

