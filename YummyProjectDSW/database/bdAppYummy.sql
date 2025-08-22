Create database bdAppYummy
go

use bdAppYummy
go

-- Se eliminan las tablas de usuario y roles personalizadas para dar paso a ASP.NET Core Identity

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
    idUsuario NVARCHAR(450) NOT NULL, -- Cambiado a NVARCHAR(450) para referenciar el Id de AspNetUsers
    fechaVenta DATETIME DEFAULT GETDATE(),
    estado int NOT NULL,
    -- La clave foránea se agregará después de que Identity cree la tabla AspNetUsers
    -- ALTER TABLE Venta ADD CONSTRAINT FK_Venta_Usuario FOREIGN KEY (idUsuario) REFERENCES AspNetUsers(Id);
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

-- Se eliminan las tablas de usuario y roles, por ende, sus valores por defecto
-- y las inserciones ya no son necesarias

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

---------------------------------------------------PROC ALEJANDRO-------------------------------------------------------------------------------

create or alter proc usp_producto
as
select * from Producto
go

/*create or alter proc usp_merge_producto
@nom varchar(255),
@prec decimal(10,2),
@stock int,
@id_cat_or int,
@id_cat_com int
as
insert into Producto (nombreProducto,precioProd,stockProd,idCategoriaOrigen,idCategoriaComida)
values (@nom,@prec, @stock, @id_cat_or, @id_cat_com)
go*/

CREATE OR ALTER PROCEDURE usp_merge_producto
    @nom VARCHAR(255),
    @prec DECIMAL(10,2),
    @stock INT,
    @id_cat_or INT,
    @id_cat_com INT
AS
BEGIN
    SET NOCOUNT ON;
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