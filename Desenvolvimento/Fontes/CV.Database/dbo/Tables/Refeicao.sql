CREATE TABLE [dbo].[Refeicao] (
    [ID_REFEICAO]         INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]           INT             NOT NULL,
    [ID_CIDADE]           INT             NULL,
    [ID_ATRACAO]          INT             NULL,
    [NM_RESTAURANTE]      VARCHAR (200)   NOT NULL,
    [CD_PLACE]            VARCHAR (50)    NULL,
    [DT_REFEICAO]         DATETIME        NOT NULL,
    [NR_LATITUDE]         NUMERIC (12, 8) NULL,
    [NR_LONGITUDE]        NUMERIC (12, 8) NULL,
    [DS_TIPO_RESTAURANTE] VARCHAR (50)    NULL,
    [DT_ATUALIZACAO]      DATETIME        NOT NULL,
    [DT_EXCLUSAO]         DATETIME        NULL,
    [DT_REFEICAO_FIM] DATETIME NULL, 
    CONSTRAINT [PK22] PRIMARY KEY NONCLUSTERED ([ID_REFEICAO] ASC),
    CONSTRAINT [RefAtracao102] FOREIGN KEY ([ID_ATRACAO]) REFERENCES [dbo].[Atracao] ([ID_ATRACAO]),
    CONSTRAINT [RefCidade34] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE]),
    CONSTRAINT [RefViagem33] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

