USE SecretManager;
GO

DROP TABLE IF EXISTS dbo.CREDENCIAL_HISTORIAL;
DROP TABLE IF EXISTS dbo.CREDENCIAL;
DROP TABLE IF EXISTS dbo.USUARIO;
DROP TABLE IF EXISTS dbo.SERVIDOR;

CREATE TABLE dbo.SERVIDOR
(
	ServidorID	 INT		  IDENTITY(1, 1) CONSTRAINT PK_SERVIDOR PRIMARY KEY,
	Servidor	 VARCHAR(100) NOT NULL CONSTRAINT UQ_SERVIDOR_SERVIDOR UNIQUE,
	ServidorIP	 VARCHAR(100) NOT NULL CONSTRAINT UQ_SERVIDOR_SERVIDORIP UNIQUE,
	Fec_Creacion DATETIME	  CONSTRAINT DF_SERVIDOR_FEC_CREACION DEFAULT GETDATE(), -- Account creation date
	Activo		 BIT		  CONSTRAINT DF_SERVIDOR_ACTIVO DEFAULT 1				 -- Flag to indicate if the account is active
);

CREATE TABLE dbo.USUARIO
(
	UsuarioID		INT			  IDENTITY(1, 1) CONSTRAINT PK_USUARIO PRIMARY KEY,		-- Unique user ID
	Usuario			NVARCHAR(50)  NOT NULL CONSTRAINT UQ_USUARIO_USUARIO UNIQUE,		-- Username (must be unique)
	Correo			NVARCHAR(100) NOT NULL CONSTRAINT UQ_USUARIO_CORREO UNIQUE,			-- Email address (must be unique)
	Clave			NVARCHAR(256) NOT NULL,												-- Store hashed passwords
	LlaveEncripcion NVARCHAR(32)  NOT NULL,
	Fec_Creacion	DATETIME	  CONSTRAINT DF_USUARIO_FEC_CREACION DEFAULT GETDATE(), -- Account creation date
	Activo			BIT			  CONSTRAINT DF_USUARIO_ACTIVO DEFAULT 1				-- Flag to indicate if the account is active
);

CREATE TABLE dbo.CREDENCIAL
(
	CredencialID	 INT		   IDENTITY(1, 1) CONSTRAINT PK_CREDENCIAL PRIMARY KEY, -- Unique credential ID
	UsuarioID		 INT		   NOT NULL,											-- Foreign key to Users
	ServidorID		 INT		   NOT NULL,
	CredencialNombre NVARCHAR(100) NOT NULL,											-- Name/description of the credential (e.g., "Gmail Account")
	Usuario			 NVARCHAR(100) NOT NULL,											-- Username for the credential
	Fec_Creacion	 DATETIME	   DEFAULT GETDATE(),									-- When the credential was created
	Activo			 BIT		   DEFAULT 1,											-- Flag to indicate if the credential is active
	CONSTRAINT FK_CREDENCIAL_USUARIO FOREIGN KEY (UsuarioID) REFERENCES SecretManager.dbo.USUARIO (UsuarioID) ON DELETE CASCADE,
	CONSTRAINT FK_CREDENCIAL_SERVIDOR FOREIGN KEY (ServidorID) REFERENCES SecretManager.dbo.SERVIDOR (ServidorID) ON DELETE CASCADE
);

CREATE TABLE dbo.CREDENCIAL_HISTORIAL
(
	CredencialHistorialID INT			IDENTITY(1, 1) CONSTRAINT PK_CREDENCIAL_HISTORIAL PRIMARY KEY,		  -- Unique ID for each history record
	CredencialID		  INT			NOT NULL,															  -- Foreign key to Credentials
	CredencialClave		  NVARCHAR(256) NOT NULL,															  -- Hashed password
	Fec_Transaccion		  DATETIME		CONSTRAINT DF_CREDENCIAL_HISTORIAL_FEC_TRANSACCION DEFAULT GETDATE(), -- Date and time of the password change
	CONSTRAINT FK_CREDENCIAL_HISTORIAL_CREDENCIAL FOREIGN KEY (CredencialID) REFERENCES SecretManager.dbo.CREDENCIAL
												  (CredencialID) ON DELETE CASCADE
);
GO