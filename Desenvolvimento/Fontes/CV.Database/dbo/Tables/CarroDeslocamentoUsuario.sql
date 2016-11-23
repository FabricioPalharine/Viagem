CREATE TABLE [dbo].[CarroDeslocamentoUsuario]
(
	ID_CARRO_DESLOCAMENTO_USUARIO INT IDENTITY(1,1),
	ID_CARRO_DESLOCAMENTO INT NOT NULL,
	ID_USUARIO			INT NOT NULL,
	CONSTRAINT PK_CarroDeslocamentoUsuario Primary Key(ID_CARRO_DESLOCAMENTO_USUARIO),
	CONSTRAINT FK_CarroDeslocamentoUsuario1 Foreign Key (ID_CARRO_DESLOCAMENTO) References CarroDeslocamento(ID_CARRO_DESLOCAMENTO),
	CONSTRAINT FK_CarroDeslocamentoUsuario2 Foreign Key (ID_USUARIO) References Usuario(ID_USUARIO)
)
