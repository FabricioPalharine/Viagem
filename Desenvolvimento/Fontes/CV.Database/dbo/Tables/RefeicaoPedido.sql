CREATE TABLE [dbo].[RefeicaoPedido] (
    [ID_REFEICAO_PEDIDO] INT           IDENTITY (1, 1) NOT NULL,
    [ID_REFEICAO]        INT           NOT NULL,
    [ID_USUARIO]         INT           NOT NULL,
    [DS_PEDIDO]          VARCHAR (200) NULL,
    [NR_NOTA]            INT           NULL,
    [DS_COMENTARIO]      TEXT          NULL,
    [DT_ATUALIZACAO]     DATETIME      NOT NULL,
    [DT_EXCLUSAO]        DATETIME      NULL,
    CONSTRAINT [PK48] PRIMARY KEY CLUSTERED ([ID_REFEICAO_PEDIDO] ASC),
    CONSTRAINT [RefRefeicao104] FOREIGN KEY ([ID_REFEICAO]) REFERENCES [dbo].[Refeicao] ([ID_REFEICAO]),
    CONSTRAINT [RefUsuario103] FOREIGN KEY ([ID_USUARIO]) REFERENCES [dbo].[Usuario] ([ID_USUARIO])
);

