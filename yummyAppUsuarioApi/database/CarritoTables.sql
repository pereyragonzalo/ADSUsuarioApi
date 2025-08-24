USE bdAppYummy
GO

CREATE TABLE Carrito (
    idCarrito INT PRIMARY KEY IDENTITY(1,1),
    idUsuario NVARCHAR(450) NOT NULL,
    fechaCreacion DATETIME DEFAULT GETDATE(),
    fechaActualizacion DATETIME DEFAULT GETDATE(),
    estado INT NOT NULL DEFAULT 1,

    INDEX IX_Carrito_Usuario (idUsuario),
    INDEX IX_Carrito_Estado (estado)
);
GO

CREATE TABLE CarritoItem (
    idCarritoItem INT PRIMARY KEY IDENTITY(1,1),
    idCarrito INT NOT NULL,
    idProducto INT NOT NULL,
    cantidad INT NOT NULL,
    precioUnitario DECIMAL(10,2) NOT NULL,
    fechaAgregado DATETIME DEFAULT GETDATE(),
    estado INT NOT NULL DEFAULT 1,

    FOREIGN KEY (idCarrito) REFERENCES Carrito(idCarrito) ON DELETE CASCADE,
    FOREIGN KEY (idProducto) REFERENCES Producto(idProducto),

    INDEX IX_CarritoItem_Carrito (idCarrito),
    INDEX IX_CarritoItem_Producto (idProducto),
    INDEX IX_CarritoItem_Estado (estado)
);
GO

ALTER TABLE CarritoItem
ADD CONSTRAINT CK_CarritoItem_Cantidad CHECK (cantidad > 0);
GO

ALTER TABLE CarritoItem
ADD CONSTRAINT CK_CarritoItem_Precio CHECK (precioUnitario > 0);
GO

CREATE OR ALTER PROCEDURE usp_ObtenerCarritoUsuario
    @IdUsuario NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ci.idCarritoItem,
        ci.idProducto,
        p.nombreProducto,
        ci.cantidad,
        ci.precioUnitario,
        (ci.cantidad * ci.precioUnitario) AS subtotal,
        ci.fechaAgregado,
        co.nombreCategoriaOrigen,
        cc.nombreCategoriaComida
    FROM Carrito c
    INNER JOIN CarritoItem ci ON c.idCarrito = ci.idCarrito
    INNER JOIN Producto p ON ci.idProducto = p.idProducto
    INNER JOIN CategoriaOrigen co ON p.idCategoriaOrigen = co.idCategoriaOrigen
    INNER JOIN CategoriaComida cc ON p.idCategoriaComida = cc.idCategoriaComida
    WHERE c.idUsuario = @IdUsuario 
        AND c.estado = 1 
        AND ci.estado = 1
    ORDER BY ci.fechaAgregado DESC;
END
GO

CREATE OR ALTER PROCEDURE usp_AgregarAlCarrito
    @IdUsuario NVARCHAR(450),
    @IdProducto INT,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdCarrito INT;
    DECLARE @PrecioProducto DECIMAL(10,2);
    DECLARE @StockDisponible INT;
    
    
    SELECT @PrecioProducto = precioProd, @StockDisponible = stockProd
    FROM Producto 
    WHERE idProducto = @IdProducto AND estado = 1;
    
    IF @PrecioProducto IS NULL
    BEGIN
        RAISERROR('Producto no encontrado', 16, 1);
        RETURN;
    END
    
    IF @StockDisponible < @Cantidad
    BEGIN
        RAISERROR('Stock no disponible: %d', 16, 1, @StockDisponible);
        RETURN;
    END
    
    SELECT @IdCarrito = idCarrito 
    FROM Carrito 
    WHERE idUsuario = @IdUsuario AND estado = 1;
    
    IF @IdCarrito IS NULL
    BEGIN
        INSERT INTO Carrito (idUsuario, fechaCreacion, fechaActualizacion, estado)
        VALUES (@IdUsuario, GETDATE(), GETDATE(), 1);
        
        SET @IdCarrito = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE Carrito 
        SET fechaActualizacion = GETDATE()
        WHERE idCarrito = @IdCarrito;
    END
    
    DECLARE @CantidadExistente INT;
    SELECT @CantidadExistente = cantidad
    FROM CarritoItem 
    WHERE idCarrito = @IdCarrito AND idProducto = @IdProducto AND estado = 1;
    
    IF @CantidadExistente IS NOT NULL
    BEGIN
        UPDATE CarritoItem 
        SET cantidad = cantidad + @Cantidad,
            fechaAgregado = GETDATE()
        WHERE idCarrito = @IdCarrito AND idProducto = @IdProducto AND estado = 1;
    END
    ELSE
    BEGIN
        INSERT INTO CarritoItem (idCarrito, idProducto, cantidad, precioUnitario, fechaAgregado, estado)
        VALUES (@IdCarrito, @IdProducto, @Cantidad, @PrecioProducto, GETDATE(), 1);
    END
    
    EXEC usp_ObtenerCarritoUsuario @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE usp_ActualizarCantidadCarrito
    @IdCarritoItem INT,
    @NuevaCantidad INT,
    @IdUsuario NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdProducto INT;
    DECLARE @StockDisponible INT;
    
    SELECT @IdProducto = ci.idProducto
    FROM CarritoItem ci
    INNER JOIN Carrito c ON ci.idCarrito = c.idCarrito
    WHERE ci.idCarritoItem = @IdCarritoItem 
        AND c.idUsuario = @IdUsuario 
        AND ci.estado = 1;
    
    IF @IdProducto IS NULL
    BEGIN
        RAISERROR('Item del carrito no encontrado', 16, 1);
        RETURN;
    END
    
    SELECT @StockDisponible = stockProd
    FROM Producto 
    WHERE idProducto = @IdProducto AND estado = 1;
    
    IF @StockDisponible < @NuevaCantidad
    BEGIN
        RAISERROR('Stock insuficiente. Disponible: %d', 16, 1, @StockDisponible);
        RETURN;
    END
    
    UPDATE CarritoItem 
    SET cantidad = @NuevaCantidad,
        fechaAgregado = GETDATE()
    WHERE idCarritoItem = @IdCarritoItem;
    
    UPDATE Carrito 
    SET fechaActualizacion = GETDATE()
    WHERE idCarrito = (SELECT idCarrito FROM CarritoItem WHERE idCarritoItem = @IdCarritoItem);
    
    EXEC usp_ObtenerCarritoUsuario @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE usp_EliminarDelCarrito
    @IdCarritoItem INT,
    @IdUsuario NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdCarrito INT;
    SELECT @IdCarrito = ci.idCarrito
    FROM CarritoItem ci
    INNER JOIN Carrito c ON ci.idCarrito = c.idCarrito
    WHERE ci.idCarritoItem = @IdCarritoItem 
        AND c.idUsuario = @IdUsuario;
    
    IF @IdCarrito IS NULL
    BEGIN
        RAISERROR('Producto no encontrado en el carrito', 16, 1);
        RETURN;
    END
    
    UPDATE CarritoItem 
    SET estado = 0
    WHERE idCarritoItem = @IdCarritoItem;
    
    UPDATE Carrito 
    SET fechaActualizacion = GETDATE()
    WHERE idCarrito = @IdCarrito;
    
    EXEC usp_ObtenerCarritoUsuario @IdUsuario;
END
GO

CREATE OR ALTER PROCEDURE usp_LimpiarCarrito
    @IdUsuario NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdCarrito INT;
    
    SELECT @IdCarrito = idCarrito 
    FROM Carrito 
    WHERE idUsuario = @IdUsuario AND estado = 1;
    
    IF @IdCarrito IS NOT NULL
    BEGIN
        UPDATE CarritoItem 
        SET estado = 0
        WHERE idCarrito = @IdCarrito;
        
        UPDATE Carrito 
        SET fechaActualizacion = GETDATE()
        WHERE idCarrito = @IdCarrito;
    END
    
    SELECT 'Carrito limpiado correctamente' AS mensaje;
END
GO

CREATE OR ALTER PROCEDURE usp_ResumenCarrito
    @IdUsuario NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(ci.idCarritoItem) AS totalItems,
        SUM(ci.cantidad) AS totalCantidad,
        SUM(ci.cantidad * ci.precioUnitario) AS totalPrecio,
        c.fechaCreacion,
        c.fechaActualizacion
    FROM Carrito c
    LEFT JOIN CarritoItem ci ON c.idCarrito = ci.idCarrito AND ci.estado = 1
    WHERE c.idUsuario = @IdUsuario AND c.estado = 1
    GROUP BY c.idCarrito, c.fechaCreacion, c.fechaActualizacion;
END
GO
