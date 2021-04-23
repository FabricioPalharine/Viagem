CREATE TABLE [dbo].[Hotel] (
    [ID_HOTEL]            INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]           INT             NOT NULL,
    [ID_CIDADE]           INT             NULL,
    [NM_HOTEL]            VARCHAR (200)   NOT NULL,
    [CD_PLACE]            VARCHAR (50)    NULL,
    [DT_ENTRADA]          DATETIME        NULL,
    [DT_SAIDA]            DATETIME        NULL,
    [NR_LONGITUDE]        NUMERIC (12, 8) NULL,
    [NR_LATITUDE]         NUMERIC (12, 8) NULL,
    [DT_ENTRADA_PREVISTA] DATETIME        NULL,
    [DT_SAIDA_PREVISTA]   DATETIME        NULL,
    [DT_ATUALIZACAO]      DATETIME        NOT NULL,
    [DT_EXCLUSAO]         DATETIME        NULL,
    [NR_RAIO]             INT             NOT NULL,
    [FL_MOVEL] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK17] PRIMARY KEY NONCLUSTERED ([ID_HOTEL] ASC),
    CONSTRAINT [RefCidade28] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefViagem27] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

