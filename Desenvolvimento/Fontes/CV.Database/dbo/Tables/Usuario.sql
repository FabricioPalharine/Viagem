﻿CREATE TABLE [dbo].[Usuario] (
    [ID_USUARIO]        INT           identity(1,1),
    [DS_EMAIL]          VARCHAR (200) NOT NULL,
    [NM_USUARIO]        VARCHAR (200) NOT NULL,
    [CD_TOKEN]          VARCHAR (200) NOT NULL,
    [CD_REFRESH_TOKEN]  VARCHAR (200) NOT NULL,
    [DT_TOKEN]          DATETIME      NOT NULL,
    [NR_TOKEN_LIFETIME] INT           NOT NULL,
    [CD_USUARIO]        VARCHAR (50)  NOT NULL,
    CONSTRAINT [PK1] PRIMARY KEY NONCLUSTERED ([ID_USUARIO] ASC)
);

