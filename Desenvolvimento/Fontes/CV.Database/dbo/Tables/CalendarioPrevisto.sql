CREATE TABLE [dbo].[CalendarioPrevisto] (
    [ID_CALENDARIO_PREVISTO] INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]              INT             NOT NULL,
    [DT_CALENDARIO]          DATETIME        NOT NULL,
    [HR_INICIO]              CHAR (5)        NULL,
    [HR_FIM]                 CHAR (5)        NULL,
    [NM_LOCAL]               VARCHAR (100)   NOT NULL,
    [NR_LATITUDE]            NUMERIC (12, 8) NULL,
    [NR_LONGITUDE]           NUMERIC (12, 8) NULL,
    [CD_PLACE]               VARCHAR (50)    NULL,
    [DS_TIPO]                VARCHAR (50)    NULL,
    [CD_PRIORIDADE]          INT             NOT NULL,
    CONSTRAINT [PK14] PRIMARY KEY NONCLUSTERED ([ID_CALENDARIO_PREVISTO] ASC),
    CONSTRAINT [RefViagem21] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

