CREATE TABLE [dbo].[AluguelReabastecimento] (
    [ID_ALUGUEL_REABASTECIMENTO] INT             IDENTITY (1, 1) NOT NULL,
    [ID_CARRO]                   INT             NOT NULL,
    [ID_CIDADE]                  INT             NOT NULL,
    [NR_LATITUDE]                NUMERIC (12, 8) NOT NULL,
    [NR_LONGITUDE]               NUMERIC (12, 8) NOT NULL,
    [FL_LITRO]                   BIT             NOT NULL,
    [VL_REABASTECIDO]            NUMERIC (6, 2)  NULL,
    CONSTRAINT [PK27] PRIMARY KEY NONCLUSTERED ([ID_ALUGUEL_REABASTECIMENTO] ASC),
    CONSTRAINT [RefCarro43] FOREIGN KEY ([ID_CARRO]) REFERENCES [dbo].[Carro] ([ID_CARRO]),
    CONSTRAINT [RefCidade47] FOREIGN KEY ([ID_CIDADE]) REFERENCES [dbo].[Cidade] ([ID_CIDADE])
);

