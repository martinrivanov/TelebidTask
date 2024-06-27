USE UserDB
GO

CREATE OR ALTER PROC getAllUsers
AS
	SELECT * FROM Users;
GO

CREATE OR ALTER PROC getUserById @Id UNIQUEIDENTIFIER
AS
	SELECT * FROM Users
	WHERE Id = @Id;
GO

CREATE OR ALTER PROC getUsersWithEmail @Email NVARCHAR(MAX)
AS
	SELECT * FROM Users
	WHERE Email = @Email;
GO

CREATE OR ALTER PROC getCountOfUsersWithEmail @Email NVARCHAR(MAX)
AS
	SELECT COUNT(*) AS CountOfUsersWithEmail FROM Users
	WHERE Email = @Email;
GO

CREATE OR ALTER PROC createUser @Id UNIQUEIDENTIFIER, @Name NVARCHAR(MAX), @Email NVARCHAR(MAX), @Password NVARCHAR(MAX), @Salt NVARCHAR(MAX)
AS
	INSERT INTO Users(Id, [Name], Email, [Password], Salt)
	VALUES (@Id, @Name, @Email, @Password, @Salt);
GO

CREATE OR ALTER PROC updateUserInfo @Id UNIQUEIDENTIFIER, @Name NVARCHAR(MAX), @Email NVARCHAR(MAX), @Password NVARCHAR(MAX)
AS
	UPDATE Users
	SET [Name] = @Name, Email = @Email, [Password] = @Password
	WHERE Id = @Id;
GO

EXEC getAllUsers;