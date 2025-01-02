USE master;
GO

IF NOT EXISTS (SELECT 1
			   FROM master.sys.databases AS D
			   WHERE D.name = 'SecretManager')
BEGIN
	CREATE DATABASE SecretManager;
END;
GO

USE SecretManager;
GO

DROP TABLE IF EXISTS dbo.CREDENCIAL_HISTORIAL;
DROP TABLE IF EXISTS dbo.CREDENCIAL;
DROP TABLE IF EXISTS dbo.USUARIO;
DROP TABLE IF EXISTS dbo.SERVIDOR;

CREATE TABLE dbo.SERVIDOR
(
	ServidorID		 INT		  IDENTITY(1, 1) CONSTRAINT PK_SERVIDOR PRIMARY KEY,
	Servidor		 VARCHAR(100) NOT NULL CONSTRAINT UQ_SERVIDOR_SERVIDOR UNIQUE,
	ServidorIP		 VARCHAR(100) NOT NULL,
	Fec_Creacion	 DATETIME	  NOT NULL CONSTRAINT DF_SERVIDOR_FEC_CREACION DEFAULT GETDATE(),
	Usr_Modificacion VARCHAR(50)  NOT NULL CONSTRAINT DF_SERVIDOR_USR_MODIFICACION DEFAULT SUSER_SNAME(),
	Fec_Modificacion DATETIME	  NOT NULL CONSTRAINT DF_SERVIDOR_FEC_MODIFICACION DEFAULT GETDATE(),
	Activo			 BIT		  NOT NULL CONSTRAINT DF_SERVIDOR_ACTIVO DEFAULT 1
);

CREATE TABLE dbo.USUARIO
(
	UsuarioID		 INT		   IDENTITY(1, 1) CONSTRAINT PK_USUARIO PRIMARY KEY,
	Usuario			 NVARCHAR(50)  NOT NULL CONSTRAINT UQ_USUARIO_USUARIO UNIQUE,
	Correo			 NVARCHAR(100) NOT NULL CONSTRAINT UQ_USUARIO_CORREO UNIQUE,
	Clave			 NVARCHAR(256) NOT NULL,
	LlaveEncripcion	 NVARCHAR(32)  NOT NULL,
	Fec_Creacion	 DATETIME	   NOT NULL CONSTRAINT DF_USUARIO_FEC_CREACION DEFAULT GETDATE(),
	Usr_Modificacion VARCHAR(50)   NOT NULL CONSTRAINT DF_USUARIO_USR_MODIFICACION DEFAULT SUSER_SNAME(),
	Fec_Modificacion DATETIME	   NOT NULL CONSTRAINT DF_USUARIO_FEC_MODIFICACION DEFAULT GETDATE(),
	Activo			 BIT		   NOT NULL CONSTRAINT DF_USUARIO_ACTIVO DEFAULT 1,
	CONSTRAINT CK_USUARIO_LLAVEENCRIPCION CHECK (LEN(LlaveEncripcion) = 32)
);

CREATE TABLE dbo.CREDENCIAL
(
	CredencialID		  INT			IDENTITY(1, 1) CONSTRAINT PK_CREDENCIAL PRIMARY KEY,
	UsuarioID			  INT			NOT NULL,
	ServidorID			  INT			NOT NULL,
	CredencialDescripcion NVARCHAR(256) NOT NULL,
	CredencialUsuario	  NVARCHAR(100) NOT NULL,
	Fec_Creacion		  DATETIME		NOT NULL CONSTRAINT DF_CREDENCIAL_FEC_CREACION DEFAULT GETDATE(),
	Usr_Modificacion	  VARCHAR(50)	NOT NULL CONSTRAINT DF_CREDENCIAL_USR_MODIFICACION DEFAULT SUSER_SNAME(),
	Fec_Modificacion	  DATETIME		NOT NULL CONSTRAINT DF_CREDENCIAL_FEC_MODIFICACION DEFAULT GETDATE(),
	Activo				  BIT			NOT NULL DEFAULT 1,
	CONSTRAINT FK_CREDENCIAL_USUARIO FOREIGN KEY (UsuarioID) REFERENCES SecretManager.dbo.USUARIO (UsuarioID) ON DELETE CASCADE,
	CONSTRAINT FK_CREDENCIAL_SERVIDOR FOREIGN KEY (ServidorID) REFERENCES SecretManager.dbo.SERVIDOR (ServidorID) ON DELETE CASCADE
);

CREATE TABLE dbo.CREDENCIAL_HISTORIAL
(
	CredencialHistorialID INT			IDENTITY(1, 1) CONSTRAINT PK_CREDENCIAL_HISTORIAL PRIMARY KEY,
	CredencialID		  INT			NOT NULL,
	CredencialClave		  NVARCHAR(256) NOT NULL,
	Usr_Modificacion	  VARCHAR(50)	NOT NULL CONSTRAINT DF_CREDENCIAL_HISTORIAL_USR_MODIFICACION DEFAULT SUSER_SNAME(),
	Fec_Creacion		  DATETIME		NOT NULL CONSTRAINT DF_CREDENCIAL_HISTORIAL_FEC_MODIFICACION DEFAULT GETDATE(),
	Fec_Encripcion		  DATETIME		NOT NULL CONSTRAINT DF_CREDENCIAL_HISTORIAL_FEC_ENCRIPCION DEFAULT GETDATE(),
	CONSTRAINT FK_CREDENCIAL_HISTORIAL_CREDENCIAL FOREIGN KEY (CredencialID) REFERENCES SecretManager.dbo.CREDENCIAL
												  (CredencialID) ON DELETE CASCADE
);
GO