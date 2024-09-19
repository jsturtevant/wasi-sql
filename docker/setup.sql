/*

Enter custom T-SQL here that would run after SQL Server has started up. 

*/

CREATE DATABASE HelloWorld;
GO
CREATE TABLE HelloWorld.dbo.HelloWorldTable (id INT, name NVARCHAR(50));
GO
INSERT INTO HelloWorld.dbo.HelloWorldTable VALUES (1, 'Hello, World!');
GO
INSERT INTO HelloWorld.dbo.HelloWorldTable VALUES (2, 'Hello, Docker!');