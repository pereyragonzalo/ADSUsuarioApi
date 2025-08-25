

USE [bdAppYummy]
GO


CREATE PROC [dbo].[ListarVentas]
AS
SELECT 
	V.*, 
	U.UserName AS Usuario

FROM Venta V 
INNER JOIN Usuario U ON V.idUsuario = U.Id
WHERE estado=1

------------------------------------------------------
CREATE PROC [dbo].[ObtenerVentaPorID]
(
    @Id INT
)
AS
BEGIN
    SELECT 
        V.idVenta, 
        V.idUsuario, 
        V.fechaVenta, 
        V.estado,
        U.UserName AS Usuario 
    FROM Venta V
    -- INNER JOIN AspNetUsers U ON V.idUsuario = U.Id  
    INNER JOIN Usuario U ON V.idUsuario = U.Id  
    WHERE V.idVenta = @Id
END




-------------------------------------------------------

CREATE PROC [dbo].[RegistrarVenta]
(
    @idUsuario VARCHAR(450),
    @fechaVenta Datetime
)
AS
BEGIN
    INSERT INTO Venta (idUsuario, fechaVenta, estado)
    VALUES (@idUsuario, @fechaVenta, 1)

    SELECT SCOPE_IDENTITY()
END

-----------------------------------------------------

CREATE PROC [dbo].[ActualizarVentas]
(
	@Id INT,
	@idUsuario VARCHAR(450),
	@fechaVenta Datetime
)
AS
UPDATE Venta
SET 
	idUsuario = @idUsuario
	--fechaVenta = @fechaVenta
WHERE idVenta = @Id


--------------------------------------------------------

CREATE PROC [dbo].[EliminarVenta]
(
	@id INT
)
AS
UPDATE Venta
SET Estado = 0
WHERE idVenta = @id


--------------------------------------------------------
----- OJOOOOOOOO ELIMINAR

-----------------------------------------------------------
-- ELIMINAR CUANDO EXISTA AspNetUsers

CREATE TABLE Usuario (
    Id NVARCHAR(450) PRIMARY KEY,
    UserName NVARCHAR(256) NOT NULL,
    Email NVARCHAR(256) NOT NULL
);


INSERT INTO Usuario (Id, UserName, Email) VALUES
('user1', 'jlopez', 'jlopez@example.com'),
('user2', 'mrojas', 'mrojas@example.com'),
('user3', 'acastro', 'acastro@example.com'),
('user4', 'pfernandez', 'pfernandez@example.com'),
('user5', 'dvalverde', 'dvalverde@example.com');


--------------------------------------------------------

CREATE PROC [dbo].[ListarUsuariosVentas]
AS
SELECT Id, UserName from Usuario

--------------------------------------------------------
--MODIFICAR Y ACTIVAR COMENTADO 

CREATE PROC [dbo].[ListarUsuariosVentas]
AS
--BEGIN
--	select
--		Id, 
--		UserName,
--	from AspNetUsers
--END
SELECT * from Usuario

--------------------------------------------------------













