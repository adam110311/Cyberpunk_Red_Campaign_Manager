-- 001_create_users.sql
-- The Users table: one row per account (FR-01, FR-02, NFR-01).

USE CyberpunkDB;
GO

CREATE TABLE dbo.Users
(
    Id            INT IDENTITY(1,1) NOT NULL,
    Username      NVARCHAR(50)      NOT NULL,
    Email         NVARCHAR(256)     NOT NULL,
    PasswordHash  NVARCHAR(256)     NOT NULL,

    CONSTRAINT PK_Users                    PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Username           UNIQUE (Username),
    CONSTRAINT UQ_Users_Email              UNIQUE (Email),
    CONSTRAINT CK_Users_Username_NotBlank  CHECK (LEN(Username) > 0),
    CONSTRAINT CK_Users_Email_NotBlank     CHECK (LEN(Email) > 0)
);
GO
