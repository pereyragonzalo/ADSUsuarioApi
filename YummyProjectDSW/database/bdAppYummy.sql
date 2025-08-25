Create database bdAppYummy
go

use bdAppYummy
go

-- Tabla: CategoriaComida
CREATE TABLE CategoriaComida (
    idCategoriaComida INT PRIMARY KEY IDENTITY(1,1),
    nombreCategoriaComida VARCHAR(100) NOT NULL,
    estado int NOT NULL
);

-- Tabla: CategoriaOrigen
CREATE TABLE CategoriaOrigen (
    idCategoriaOrigen INT PRIMARY KEY IDENTITY(1,1),
    nombreCategoriaOrigen VARCHAR(100) NOT NULL,
    estado int NOT NULL
);

-- Tabla: Producto
CREATE TABLE Producto (
    idProducto INT PRIMARY KEY IDENTITY(1,1),
    nombreProducto VARCHAR(100) NOT NULL,
    precioProd DECIMAL(10,2) NOT NULL,
	stockProd int NOT NULL,
    idCategoriaOrigen INT NOT NULL,
    idCategoriaComida INT NOT NULL,
    estado int NOT NULL,
    FOREIGN KEY (idCategoriaOrigen) REFERENCES CategoriaOrigen(idCategoriaOrigen),
    FOREIGN KEY (idCategoriaComida) REFERENCES CategoriaComida(idCategoriaComida)
);

-- Tabla: Venta
CREATE TABLE Venta (
    idVenta INT PRIMARY KEY IDENTITY(1,1),
    idUsuario NVARCHAR(450), -- mantener el tipo de dato a NVARCHAR(450) para referenciar el Id de AspNetUsers. En la version final dejarlo como NOT NULL
    fechaVenta DATETIME DEFAULT GETDATE(),
    estado int NOT NULL,
);

-- Tabla: DetalleVenta
CREATE TABLE DetalleVenta (
    idDetalleVenta INT PRIMARY KEY IDENTITY(1,1),
    idVenta INT NOT NULL,
    idProducto INT NOT NULL,
    cantidad INT NOT NULL,
    precioProd DECIMAL(10,2) NOT NULL,
    subtotal AS (cantidad * precioProd) PERSISTED,
    estado int NOT NULL,
    FOREIGN KEY (idVenta) REFERENCES Venta(idVenta),
    FOREIGN KEY (idProducto) REFERENCES Producto(idProducto)
)
go

-- CategoriaComida
ALTER TABLE CategoriaComida
ADD CONSTRAINT DF_CategoriaComida_Estado DEFAULT 1 FOR estado;
go

-- CategoriaOrigen
ALTER TABLE CategoriaOrigen
ADD CONSTRAINT DF_CategoriaOrigen_Estado DEFAULT 1 FOR estado;
go

-- Producto
ALTER TABLE Producto
ADD CONSTRAINT DF_Producto_Estado DEFAULT 1 FOR estado;
go

-- Venta
ALTER TABLE Venta
ADD CONSTRAINT DF_Venta_Estado DEFAULT 1 FOR estado;
go

-- DetalleVenta
ALTER TABLE DetalleVenta
ADD CONSTRAINT DF_DetalleVenta_Estado DEFAULT 1 FOR estado;
go

-- Insertar datos

-- Insertar datos en CategoriaOrigen
INSERT INTO CategoriaOrigen (nombreCategoriaOrigen) VALUES 
('Italiana'),
('Mexicana'),
('Japonesa')
go

-- Insertar datos en CategoriaComida
INSERT INTO CategoriaComida (nombreCategoriaComida) VALUES 
('Postres'),
('Platos Principales'),
('Bebidas')
go

INSERT INTO Producto (nombreProducto, precioProd, stockProd, idCategoriaOrigen, idCategoriaComida)
VALUES 
('Tiramisú', 18.50, 20, 1, 1),
('Tacos al Pastor', 12.00, 50, 2, 2),
('Sushi Roll', 22.00, 30, 3, 2),
('Agua Mineral', 5.00, 100, 3, 3),
('Pizza Margarita', 25.00, 15, 1, 2)
go

INSERT INTO Venta (idUsuario) VALUES
('8980f024-b071-4f82-81f8-86374d0c029a'),
('8980f024-b071-4f82-81f8-86374d0c029b'),
('8980f024-b071-4f82-81f8-86374d0c029c'),
('8980f024-b071-4f82-81f8-86374d0c029a') 
GO

INSERT INTO DetalleVenta (idVenta, idProducto, cantidad, precioProd)
VALUES 
(1, 1, 2, 18.50),
(2, 2, 3, 12.00),
(3, 3, 1, 22.00),
(4, 4, 5, 5.00),
(5, 5, 2, 25.00)
go

-- Procedimientos almacenado

CREATE or alter PROCEDURE usp_productoModel
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.idProducto,
        p.nombreProducto,
        p.precioProd,
        p.stockProd,
        co.nombreCategoriaOrigen AS nombreOrigen,
        cc.nombreCategoriaComida AS nombreComida,
        p.estado
    FROM Producto p
    INNER JOIN CategoriaOrigen co ON p.idCategoriaOrigen = co.idCategoriaOrigen
    INNER JOIN CategoriaComida cc ON p.idCategoriaComida = cc.idCategoriaComida
	where p.estado=1
END
GO


create or alter proc usp_producto
as
select * from Producto
where estado=1
go


CREATE OR ALTER PROCEDURE usp_merge_producto
    @nom VARCHAR(255),
    @prec DECIMAL(10,2),
    @stock INT,
    @id_cat_or INT,
    @id_cat_com INT
AS
BEGIN
    SET NOCOUNT OFF;
    IF EXISTS (SELECT 1 FROM Producto WHERE nombreProducto = @nom)
    BEGIN
        UPDATE Producto
        SET
            precioProd = @prec,
            stockProd = @stock,
            idCategoriaOrigen = @id_cat_or,
            idCategoriaComida = @id_cat_com
        WHERE nombreProducto = @nom;
    END
    ELSE
    BEGIN
        INSERT INTO Producto (nombreProducto, precioProd, stockProd, idCategoriaOrigen, idCategoriaComida)
        VALUES (@nom, @prec, @stock, @id_cat_or, @id_cat_com);
    END
END
GO

CREATE or alter PROCEDURE usp_desactivar_producto
    @idProducto INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Verifica si el producto existe y está activo
    IF EXISTS (
        SELECT 1
        FROM Producto
        WHERE idProducto = @idProducto AND estado = 1
    )
    BEGIN
        -- Desactiva el producto de forma lógica
        UPDATE Producto
        SET estado = 0
        WHERE idProducto = @idProducto;
    END
END
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_reporteVentas]
    @Desde DATETIME = NULL,
    @Hasta DATETIME = NULL
AS
BEGIN
    IF @Desde IS NULL
        SET @Desde = DATEFROMPARTS(YEAR(GETDATE()), 1, 1);
        
    IF @Hasta IS NULL
        SET @Hasta = GETDATE();
        
    SET @Hasta = DATEADD(DAY, 1, @Hasta) - 0.0000001;
        
    SELECT 
        v.idVenta AS IdVenta,
        v.idVenta AS NFactura,
        v.fechaVenta AS Fecha,
        ISNULL(u.UserName, 'Usuario General') AS Cliente,
        'System' AS Vendedor,
        COUNT(dv.idDetalleVenta) AS Lineas,
        SUM(dv.cantidad) AS Cantidad,
        SUM(dv.precioProd * dv.cantidad) AS Total
    FROM [dbo].[Venta] v
    LEFT JOIN [dbo].[AspNetUsers] u ON v.idUsuario = u.Id
    INNER JOIN [dbo].[DetalleVenta] dv ON v.idVenta = dv.idVenta
    WHERE v.fechaVenta >= @Desde 
        AND v.fechaVenta <= @Hasta
    GROUP BY 
        v.idVenta,
        v.fechaVenta,
        u.UserName
    ORDER BY v.fechaVenta DESC, v.idVenta DESC;
END
GO


create or alter proc usp_catcomida
as
select * from CategoriaComida
where estado = 1
go

create or alter proc usp_catorigen
as
select * from CategoriaOrigen
where estado = 1
go

--Proc Gonzalo
create or alter proc ListarUsuarios
as
BEGIN
	select Id, Email from AspNetUsers
	order by Email;
end;
go

create or alter proc registrar_venta
@precio decimal(10,2)
as
begin
	select*from venta;
end;
go

create or alter proc alterar_producto
@id int,
@cantidad int
as
begin
	select*from venta;
end;
go

create or alter proc registrar_detalles_venta
@id int,
@cantidad int,
@precio decimal(10,2),
@subtotal decimal(10,2)
as
begin
	insert into DetalleVenta (idProducto, cantidad, precioProd, subtotal)
	values(@id,@cantidad,@precio, @subtotal)
end;
go


-----------------------------------------

-- proc Jacobo



USE [bdAppYummy]
GO


CREATE or alter PROC [dbo].[ListarVentas]
AS
SELECT 
	V.*, 
	A.UserName AS Usuario

FROM Venta V 
INNER JOIN AspNetUsers A ON V.idUsuario = A.Id
WHERE estado=1
go



------------------------------------------------------
CREATE or alter PROC [dbo].[ObtenerVentaPorID]
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
        A.UserName AS Usuario 
    FROM Venta V
    INNER JOIN AspNetUsers A ON V.idUsuario = A.Id  
    WHERE V.idVenta = @Id
END
go



-------------------------------------------------------

CREATE or alter PROC [dbo].[RegistrarVenta]
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
go
-----------------------------------------------------

CREATE or alter PROC [dbo].[ActualizarVentas]
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
go

--------------------------------------------------------

CREATE or alter PROC [dbo].[EliminarVenta]
(
	@id INT
)
AS
UPDATE Venta
SET Estado = 0
WHERE idVenta = @id
go

--------------------------------------------------------
----- OJOOOOOOOO ELIMINAR

-----------------------------------------------------------
-- ELIMINAR CUANDO EXISTA AspNetUsers

--CREATE TABLE Usuario (
--    Id NVARCHAR(450) PRIMARY KEY,
--    UserName NVARCHAR(256) NOT NULL,
--    Email NVARCHAR(256) NOT NULL
--);


--INSERT INTO Usuario (Id, UserName, Email) VALUES
--('user1', 'jlopez', 'jlopez@example.com'),
--('user2', 'mrojas', 'mrojas@example.com'),
--('user3', 'acastro', 'acastro@example.com'),
--('user4', 'pfernandez', 'pfernandez@example.com'),
--('user5', 'dvalverde', 'dvalverde@example.com');


CREATE or alter PROC [dbo].[ListarUsuariosVentas]
AS
BEGIN
	select
		Id, 
		UserName
	from AspNetUsers
END
go


create or ALTER PROCEDURE [dbo].[ObtenerUsuarioPorID]
(
    @Id NVARCHAR(450)
)
AS
BEGIN
    SELECT Id, UserName, Email
    FROM AspNetUsers
    WHERE Id = @Id
END
go