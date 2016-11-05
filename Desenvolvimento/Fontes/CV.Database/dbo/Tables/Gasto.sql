CREATE TABLE [dbo].[Gasto] (
    [ID_GASTO]             INT             IDENTITY (1, 1) NOT NULL,
    [ID_VIAGEM]            INT             NOT NULL,
    [ID_USUARIO]           INT             NOT NULL,
    [DS_GASTO]             VARCHAR (100)   NOT NULL,
    [DT_GASTO]             DATETIME        NOT NULL,
    [VL_GASTO]             NUMERIC (18, 2) NOT NULL,
    [FL_MOEDA_ESTRANGEIRA] BIT             NOT NULL,
    [FL_ESPECIE]           BIT             NOT NULL,
    [CD_MOEDA]             INT             NOT NULL,
    [DT_PAGAMENTO]         DATETIME        NULL,
    [FL_DIVIDIDO]          BIT             NULL,
    [FL_APENAS_BAIXA]      BIT             NULL,
    CONSTRAINT [PK8] PRIMARY KEY NONCLUSTERED ([ID_GASTO] ASC),
    CONSTRAINT [RefUsuario12] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO]),
    CONSTRAINT [RefViagem11] FOREIGN KEY ([ID_VIAGEM]) REFERENCES [dbo].[Viagem] ([ID_VIAGEM])
);

