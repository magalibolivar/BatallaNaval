-- =========================================================
-- Este script se puede correr las veces que haga falta:
-- no falla si la base, las tablas o los usuarios de prueba
-- ya existen (crea/inserta solo lo que falte).
-- =========================================================

USE [master]
GO
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BATALLANAVAL')
BEGIN
	CREATE DATABASE [BATALLANAVAL]
END
GO
USE [BATALLANAVAL]
GO

-- =========================================================
-- TABLAS
-- =========================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'USUARIO')
BEGIN
	CREATE TABLE [dbo].[USUARIO](
		[ID] [int] NOT NULL,
		[Nombre] [nvarchar](50) NOT NULL,
		[Contraseña] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_USUARIO] PRIMARY KEY CLUSTERED ([ID] ASC),
	 CONSTRAINT [UQ_USUARIO_NOMBRE] UNIQUE ([Nombre])
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TIPO_LOG')
BEGIN
	CREATE TABLE [dbo].[TIPO_LOG](
		[ID_TIPO] [int] NOT NULL,
		[Tipo] [varchar](50) NOT NULL,
	 CONSTRAINT [PK_TIPO_LOG] PRIMARY KEY CLUSTERED ([ID_TIPO] ASC)
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LOG')
BEGIN
	CREATE TABLE [dbo].[LOG](
		[ID_LOG] [int] NOT NULL,
		[Descripcion] [nvarchar](100) NOT NULL,
		[ID_Usuario] [int] NOT NULL,
		[ID_TIPO] [int] NOT NULL,
		[Fecha] [datetime] NOT NULL,
	 CONSTRAINT [PK_LOG] PRIMARY KEY CLUSTERED ([ID_LOG] ASC)
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LOG_TIPO_LOG')
BEGIN
	ALTER TABLE [dbo].[LOG] WITH CHECK ADD CONSTRAINT [FK_LOG_TIPO_LOG] FOREIGN KEY([ID_TIPO]) REFERENCES [dbo].[TIPO_LOG] ([ID_TIPO])
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LOG_USUARIO')
BEGIN
	ALTER TABLE [dbo].[LOG] WITH CHECK ADD CONSTRAINT [FK_LOG_USUARIO] FOREIGN KEY([ID_Usuario]) REFERENCES [dbo].[USUARIO] ([ID])
END
GO

-- A diferencia de un juego de puntaje (ej. Generala), en Batalla Naval la partida no guarda
-- puntajes: el detalle de cada colocación/disparo vive solo en el XML (RUTA_XML). Acá solo
-- se persiste el resumen (jugadores, ganador, fechas). Como en Batalla Naval siempre hay un
-- ganador una vez que la partida termina (ver Reglas_de_Negocio_BatallaNaval.md, sección de
-- empates), el CHECK obliga a que ID_GANADOR esté resuelto en el mismo momento que FECHA_FIN.
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PARTIDA')
BEGIN
	CREATE TABLE [dbo].[PARTIDA](
		[ID] [int] NOT NULL,
		[ID_JUGADOR1] [int] NOT NULL,
		[ID_JUGADOR2] [int] NOT NULL,
		[ID_GANADOR] [int] NULL,
		[FECHA_INICIO] [datetime] NOT NULL,
		[FECHA_FIN] [datetime] NULL,
		[RUTA_XML] [nvarchar](500) NOT NULL,
	 CONSTRAINT [PK_PARTIDA] PRIMARY KEY CLUSTERED ([ID] ASC),
	 CONSTRAINT [CK_PARTIDA_GANADOR] CHECK (FECHA_FIN IS NULL OR ID_GANADOR IS NOT NULL)
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PARTIDA_JUGADOR1')
BEGIN
	ALTER TABLE [dbo].[PARTIDA] WITH CHECK ADD CONSTRAINT [FK_PARTIDA_JUGADOR1] FOREIGN KEY([ID_JUGADOR1]) REFERENCES [dbo].[USUARIO] ([ID])
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PARTIDA_JUGADOR2')
BEGIN
	ALTER TABLE [dbo].[PARTIDA] WITH CHECK ADD CONSTRAINT [FK_PARTIDA_JUGADOR2] FOREIGN KEY([ID_JUGADOR2]) REFERENCES [dbo].[USUARIO] ([ID])
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PARTIDA_GANADOR')
BEGIN
	ALTER TABLE [dbo].[PARTIDA] WITH CHECK ADD CONSTRAINT [FK_PARTIDA_GANADOR] FOREIGN KEY([ID_GANADOR]) REFERENCES [dbo].[USUARIO] ([ID])
END
GO

-- =========================================================
-- DATOS INICIALES
-- =========================================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[TIPO_LOG] WHERE ID_TIPO = 1)
	INSERT INTO [dbo].[TIPO_LOG] (ID_TIPO, Tipo) VALUES (1, 'INICIO_SESION')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[TIPO_LOG] WHERE ID_TIPO = 2)
	INSERT INTO [dbo].[TIPO_LOG] (ID_TIPO, Tipo) VALUES (2, 'CIERRE_SESION')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[TIPO_LOG] WHERE ID_TIPO = 3)
	INSERT INTO [dbo].[TIPO_LOG] (ID_TIPO, Tipo) VALUES (3, 'INICIO_PARTIDA')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[TIPO_LOG] WHERE ID_TIPO = 4)
	INSERT INTO [dbo].[TIPO_LOG] (ID_TIPO, Tipo) VALUES (4, 'FIN_PARTIDA')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[USUARIO] WHERE Nombre = 'valentina')
	INSERT INTO [dbo].[USUARIO] (ID, Nombre, Contraseña) VALUES (1, 'valentina', '1234')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[USUARIO] WHERE Nombre = 'jugador2')
	INSERT INTO [dbo].[USUARIO] (ID, Nombre, Contraseña) VALUES (2, 'jugador2', '1234')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[USUARIO] WHERE Nombre = 'jugador3')
	INSERT INTO [dbo].[USUARIO] (ID, Nombre, Contraseña) VALUES (3, 'jugador3', '1234')
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[USUARIO] WHERE Nombre = 'magali')
	INSERT INTO [dbo].[USUARIO] (ID, Nombre, Contraseña) VALUES (4, 'magali', '1234')
GO

-- Usuarios invitados ("Jugar como invitado", sin usuario ni contraseña): el Jugador 1
-- anónimo usa Invitado1 y el Jugador 2 anónimo, Invitado2. Existen como filas de USUARIO
-- porque PARTIDA y LOG los referencian por clave foránea. La contraseña vacía no sirve
-- para loguearse (USUARIO_LOGIN la rechaza) y el nombre no se puede registrar (ya existe).
-- El ID se calcula con MAX+1 porque en una base ya usada los IDs bajos pueden estar ocupados.
IF NOT EXISTS (SELECT 1 FROM [dbo].[USUARIO] WHERE Nombre = 'Invitado1')
	INSERT INTO [dbo].[USUARIO] (ID, Nombre, Contraseña)
	SELECT ISNULL(MAX(ID), 0) + 1, 'Invitado1', '' FROM [dbo].[USUARIO]
GO
IF NOT EXISTS (SELECT 1 FROM [dbo].[USUARIO] WHERE Nombre = 'Invitado2')
	INSERT INTO [dbo].[USUARIO] (ID, Nombre, Contraseña)
	SELECT ISNULL(MAX(ID), 0) + 1, 'Invitado2', '' FROM [dbo].[USUARIO]
GO

-- =========================================================
-- STORED PROCEDURES
-- (CREATE OR ALTER: siempre quedan actualizados, se puede
-- volver a correr el script sin errores de "ya existe")
-- =========================================================

-- Los SP que insertan calculan el ID con MAX(ID)+1. Para que dos inserciones simultáneas
-- no saquen el mismo ID, el MAX se lee con UPDLOCK, HOLDLOCK dentro de una transacción:
-- la segunda espera a que termine la primera. XACT_ABORT ON hace que cualquier error
-- deshaga la transacción y corte el SP, sin dejarla abierta. Si el SP se llama dentro de
-- una transacción de la DAL (MP_PARTIDA), el BEGIN TRAN/COMMIT de acá queda anidado en ella.

CREATE OR ALTER PROC [dbo].[USUARIO_INSERTAR]
@usu nvarchar(50), @pass nvarchar(50)
AS
BEGIN
	SET XACT_ABORT ON
	BEGIN TRAN

	-- Nombre repetido: devuelve 0 (y no -1, que la app usa para "error de base de datos").
	IF EXISTS (SELECT 1 FROM USUARIO WITH (UPDLOCK, HOLDLOCK) WHERE Nombre = @usu)
	BEGIN
		COMMIT
		SELECT 0 AS ID
		RETURN
	END

	DECLARE @ID int
	SET @ID = (SELECT ISNULL(MAX(ID),0)+1 FROM USUARIO WITH (UPDLOCK, HOLDLOCK))

	INSERT INTO USUARIO (ID, Nombre, Contraseña)
	VALUES (@ID, @usu, @pass)

	COMMIT
	SELECT @ID AS ID
END
GO

CREATE OR ALTER PROC [dbo].[USUARIO_LOGIN]
@usu nvarchar(50), @pass nvarchar(50)
AS
BEGIN
	-- La collation por defecto de la base es case-insensitive ("Hola" = "hola"), así que la
	-- contraseña se compara con una collation CS (case-sensitive). El "=" de SQL Server además
	-- ignora los espacios al final, por eso también se compara el largo con DATALENGTH.
	SELECT ID, Nombre, Contraseña FROM USUARIO
	WHERE Nombre = @usu
	  AND Contraseña = @pass COLLATE Latin1_General_CS_AS
	  AND DATALENGTH(Contraseña) = DATALENGTH(@pass)
	  -- Los invitados tienen contraseña vacía: con usuario y contraseña nunca se puede
	  -- entrar como invitado (se entra con "Jugar como invitado").
	  AND DATALENGTH(Contraseña) > 0
END
GO

CREATE OR ALTER PROC [dbo].[USUARIO_BUSCAR_POR_NOMBRE]
@usu nvarchar(50)
AS
BEGIN
	SELECT ID, Nombre, Contraseña FROM USUARIO WHERE Nombre = @usu
END
GO

CREATE OR ALTER PROC [dbo].[USUARIO_BUSCAR_POR_ID]
@id int
AS
BEGIN
	SELECT ID, Nombre, Contraseña FROM USUARIO WHERE ID = @id
END
GO

CREATE OR ALTER PROC [dbo].[LOG_INSERTAR]
@descripcion nvarchar(100), @idUsuario int, @idTipo int
AS
BEGIN
	SET XACT_ABORT ON
	BEGIN TRAN

	DECLARE @ID int
	SET @ID = (SELECT ISNULL(MAX(ID_LOG),0)+1 FROM LOG WITH (UPDLOCK, HOLDLOCK))

	INSERT INTO LOG (ID_LOG, Descripcion, ID_Usuario, ID_TIPO, Fecha)
	VALUES (@ID, @descripcion, @idUsuario, @idTipo, GETDATE())

	COMMIT
END
GO

CREATE OR ALTER PROC [dbo].[PARTIDA_INSERTAR]
@idJugador1 int, @idJugador2 int, @rutaXml nvarchar(500)
AS
BEGIN
	SET XACT_ABORT ON
	BEGIN TRAN

	DECLARE @ID int
	SET @ID = (SELECT ISNULL(MAX(ID),0)+1 FROM PARTIDA WITH (UPDLOCK, HOLDLOCK))

	INSERT INTO PARTIDA (ID, ID_JUGADOR1, ID_JUGADOR2, FECHA_INICIO, RUTA_XML)
	VALUES (@ID, @idJugador1, @idJugador2, GETDATE(), @rutaXml)

	COMMIT
	SELECT @ID AS ID
END
GO

CREATE OR ALTER PROC [dbo].[PARTIDA_FINALIZAR]
@id int, @idGanador int
AS
BEGIN
	UPDATE PARTIDA
	SET ID_GANADOR = @idGanador,
	    FECHA_FIN = GETDATE()
	-- Una partida ya cerrada no se vuelve a tocar: así no se le puede cambiar el resultado.
	WHERE ID = @id AND FECHA_FIN IS NULL
END
GO

CREATE OR ALTER PROC [dbo].[PARTIDA_LISTAR_POR_USUARIO]
@idUsuario int
AS
BEGIN
	SELECT ID, ID_JUGADOR1, ID_JUGADOR2, ID_GANADOR, FECHA_INICIO, FECHA_FIN, RUTA_XML
	FROM PARTIDA
	WHERE (ID_JUGADOR1 = @idUsuario OR ID_JUGADOR2 = @idUsuario) AND FECHA_FIN IS NOT NULL
	ORDER BY FECHA_INICIO DESC
END
GO
