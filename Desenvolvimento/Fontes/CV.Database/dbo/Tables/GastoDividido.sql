CREATE TABLE [dbo].[GastoDividido]
(
	[ID_GASTO_DIVIDIDO] INT IDENTITY(1,1),
	[ID_GASTO]	INT	NOT NULL,
	ID_USUARIO	INT NOT NULL,
	CONSTRAINT PK_GastoDividido Primary Key(ID_GASTO_DIVIDIDO),
	CONSTRAINT FK_GastoDividido1 Foreign Key (ID_GASTO) references Gasto(ID_GASTO),
	CONSTRAINT FK_GastoDividido2 Foreign Key (ID_USUARIO) references Usuario(ID_USUARIO)
)
