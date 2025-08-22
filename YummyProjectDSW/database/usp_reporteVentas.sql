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

