CREATE TABLE [dbo].[Carro] (
    [ID_CARRO]       INT          IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]      INT          NOT NULL,
    [DS_LOCADORA]    VARCHAR (50) NULL,
    [DS_CARRO]       VARCHAR (50) NOT NULL,
    [FL_KM]          BIT          NOT NULL,
    [FL_ALUGADO]     BIT          NOT NULL,
    [DT_ATUALIZACAO] DATETIME     NOT NULL,
    [DT_EXCLUSAO]    DATETIME     NULL,
    CONSTRAINT [PK24] PRIMARY KEY NONCLUSTERED ([ID_CARRO] ASC),
    CONSTRAINT [RefViagem38] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

