-- Opcional
CREATE DATABASE AlmacenDB;
GO
USE AlmacenDB;
GO
CREATE TABLE dbo.Categoria (
    IdCategoria INT IDENTITY(1,1) NOT NULL,
    Nombre      VARCHAR(60) NOT NULL,
    CONSTRAINT PK_Categoria PRIMARY KEY (IdCategoria),
    CONSTRAINT UQ_Categoria_Nombre UNIQUE (Nombre)
);
GO

CREATE TABLE dbo.Producto (
    IdProducto   INT IDENTITY(1,1) NOT NULL,
    IdCategoria  INT NOT NULL,
    Nombre       VARCHAR(80) NOT NULL,
    PrecioActual DECIMAL(10,2) NOT NULL,
    Activo       BIT NOT NULL CONSTRAINT DF_Producto_Activo DEFAULT (1),

    CONSTRAINT PK_Producto PRIMARY KEY (IdProducto),
    CONSTRAINT FK_Producto_Categoria FOREIGN KEY (IdCategoria) REFERENCES dbo.Categoria(IdCategoria),
    CONSTRAINT CK_Producto_PrecioActual CHECK (PrecioActual > 0)
);
GO

CREATE TABLE dbo.Cliente (
    IdCliente   INT IDENTITY(1,1) NOT NULL,
    Nombre      VARCHAR(80) NOT NULL,
    TipoCliente VARCHAR(15) NOT NULL, -- 'Final' / 'Mayorista'
    Activo      BIT NOT NULL CONSTRAINT DF_Cliente_Activo DEFAULT (1),

    CONSTRAINT PK_Cliente PRIMARY KEY (IdCliente),
    CONSTRAINT CK_Cliente_Tipo CHECK (TipoCliente IN ('Final','Mayorista'))
);
GO
CREATE TABLE dbo.Venta (
    IdVenta    INT IDENTITY(1,1) NOT NULL,
    FechaHora  DATETIME2(0) NOT NULL CONSTRAINT DF_Venta_FechaHora DEFAULT (SYSDATETIME()),
    IdCliente  INT NULL,
    Total      DECIMAL(12,2) NOT NULL CONSTRAINT DF_Venta_Total DEFAULT (0),

    CONSTRAINT PK_Venta PRIMARY KEY (IdVenta),
    CONSTRAINT FK_Venta_Cliente FOREIGN KEY (IdCliente) REFERENCES dbo.Cliente(IdCliente),
    CONSTRAINT CK_Venta_Total CHECK (Total >= 0)
);
GO

CREATE TABLE dbo.DetalleVenta (
    IdVenta       INT NOT NULL,
    NroItem       SMALLINT NOT NULL,
    IdProducto    INT NOT NULL,
    Cantidad      DECIMAL(10,2) NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal      AS (Cantidad * PrecioUnitario) PERSISTED,

    CONSTRAINT PK_DetalleVenta PRIMARY KEY (IdVenta, NroItem),
    CONSTRAINT FK_DetalleVenta_Venta FOREIGN KEY (IdVenta) REFERENCES dbo.Venta(IdVenta),
    CONSTRAINT FK_DetalleVenta_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto(IdProducto),
    CONSTRAINT CK_DetalleVenta_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_DetalleVenta_PrecioUnitario CHECK (PrecioUnitario > 0)
);
GO
CREATE TABLE dbo.StockProducto (
    IdProducto  INT NOT NULL,
    StockActual DECIMAL(12,2) NOT NULL CONSTRAINT DF_StockProducto_StockActual DEFAULT (0),
    StockMinimo DECIMAL(12,2) NOT NULL CONSTRAINT DF_StockProducto_StockMinimo DEFAULT (0),

    CONSTRAINT PK_StockProducto PRIMARY KEY (IdProducto),
    CONSTRAINT FK_StockProducto_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto(IdProducto),
    CONSTRAINT CK_StockProducto_NoNeg CHECK (StockActual >= 0 AND StockMinimo >= 0)
);
GO

CREATE TABLE dbo.MovimientoStock (
    IdMovimiento   BIGINT IDENTITY(1,1) NOT NULL,
    FechaHora      DATETIME2(0) NOT NULL CONSTRAINT DF_MovStock_FechaHora DEFAULT (SYSDATETIME()),
    IdProducto     INT NOT NULL,
    TipoMovimiento CHAR(1) NOT NULL, -- 'E' entrada, 'S' salida, 'A' ajuste
    Cantidad       DECIMAL(12,2) NOT NULL,
    Motivo         VARCHAR(30) NOT NULL, -- 'Compra','Venta','Merma','Correccion'
    IdVenta        INT NULL,

    CONSTRAINT PK_MovimientoStock PRIMARY KEY (IdMovimiento),
    CONSTRAINT FK_MovStock_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto(IdProducto),
    CONSTRAINT FK_MovStock_Venta FOREIGN KEY (IdVenta) REFERENCES dbo.Venta(IdVenta),

    CONSTRAINT CK_MovStock_Tipo CHECK (TipoMovimiento IN ('E','S','A')),
    CONSTRAINT CK_MovStock_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_MovStock_VentaReglas CHECK (
        (TipoMovimiento = 'S' AND IdVenta IS NOT NULL)
        OR (TipoMovimiento IN ('E','A') AND IdVenta IS NULL)
    )
);
GO

CREATE INDEX IX_DetalleVenta_IdProducto ON dbo.DetalleVenta(IdProducto);
CREATE INDEX IX_Venta_FechaHora ON dbo.Venta(FechaHora);
CREATE INDEX IX_MovStock_Producto_Fecha ON dbo.MovimientoStock(IdProducto, FechaHora);
GO

CREATE OR ALTER VIEW dbo.v_VentasPorDia
AS
SELECT
    CAST(v.FechaHora AS date) AS Fecha,
    COUNT(*) AS CantidadVentas,
    SUM(v.Total) AS TotalVendido,
    AVG(v.Total) AS TicketPromedio
FROM dbo.Venta v
GROUP BY CAST(v.FechaHora AS date);
GO

CREATE OR ALTER VIEW dbo.v_VentasPorMes
AS
SELECT
    YEAR(v.FechaHora) AS Anio,
    MONTH(v.FechaHora) AS Mes,
    COUNT(*) AS CantidadVentas,
    SUM(v.Total) AS TotalVendido,
    AVG(v.Total) AS TicketPromedio
FROM dbo.Venta v
GROUP BY YEAR(v.FechaHora), MONTH(v.FechaHora);
GO

CREATE OR ALTER VIEW dbo.v_TopProductos
AS
SELECT
    p.IdProducto,
    p.Nombre AS Producto,
    c.Nombre AS Categoria,
    SUM(dv.Cantidad) AS CantidadVendida,
    SUM(dv.Cantidad * dv.PrecioUnitario) AS MontoVendido
FROM dbo.DetalleVenta dv
JOIN dbo.Producto p ON p.IdProducto = dv.IdProducto
JOIN dbo.Categoria c ON c.IdCategoria = p.IdCategoria
GROUP BY p.IdProducto, p.Nombre, c.Nombre;
GO

CREATE OR ALTER VIEW dbo.v_VentasPorCategoria
AS
SELECT
    c.IdCategoria,
    c.Nombre AS Categoria,
    SUM(dv.Cantidad * dv.PrecioUnitario) AS MontoVendido,
    SUM(dv.Cantidad) AS UnidadesVendidas
FROM dbo.DetalleVenta dv
JOIN dbo.Producto p ON p.IdProducto = dv.IdProducto
JOIN dbo.Categoria c ON c.IdCategoria = p.IdCategoria
GROUP BY c.IdCategoria, c.Nombre;
GO

CREATE OR ALTER VIEW dbo.v_StockActual
AS
SELECT
    p.IdProducto,
    p.Nombre AS Producto,
    c.Nombre AS Categoria,
    sp.StockActual,
    sp.StockMinimo,
    CASE WHEN sp.StockActual <= sp.StockMinimo THEN 1 ELSE 0 END AS BajoMinimo
FROM dbo.StockProducto sp
JOIN dbo.Producto p ON p.IdProducto = sp.IdProducto
JOIN dbo.Categoria c ON c.IdCategoria = p.IdCategoria;
GO
CREATE OR ALTER PROCEDURE dbo.sp_EntradaStock
    @IdProducto   INT,
    @Cantidad     DECIMAL(12,2),
    @StockMinimo  DECIMAL(12,2) = NULL,
    @Motivo       VARCHAR(30) = 'Compra'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Cantidad <= 0
        THROW 50001, 'La cantidad debe ser mayor a 0.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdProducto = @IdProducto AND Activo = 1)
        THROW 50002, 'Producto inexistente o inactivo.', 1;

    BEGIN TRAN;

    -- Crear fila de stock si no existe
    IF NOT EXISTS (SELECT 1 FROM dbo.StockProducto WHERE IdProducto = @IdProducto)
    BEGIN
        INSERT INTO dbo.StockProducto (IdProducto, StockActual, StockMinimo)
        VALUES (@IdProducto, 0, COALESCE(@StockMinimo, 0));
    END
    ELSE IF @StockMinimo IS NOT NULL
    BEGIN
        UPDATE dbo.StockProducto
        SET StockMinimo = @StockMinimo
        WHERE IdProducto = @IdProducto;
    END

    -- Actualizar stock
    UPDATE dbo.StockProducto
    SET StockActual = StockActual + @Cantidad
    WHERE IdProducto = @IdProducto;

    -- Registrar movimiento
    INSERT INTO dbo.MovimientoStock (IdProducto, TipoMovimiento, Cantidad, Motivo, IdVenta)
    VALUES (@IdProducto, 'E', @Cantidad, @Motivo, NULL);

    COMMIT;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarVentaJson
    @IdCliente INT = NULL,
    @ItemsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Validaciones básicas
    IF @ItemsJson IS NULL OR ISJSON(@ItemsJson) <> 1
        THROW 51001, 'ItemsJson inválido o vacío.', 1;

    IF @IdCliente IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE IdCliente = @IdCliente AND Activo = 1)
        THROW 51002, 'Cliente inexistente o inactivo.', 1;

    DECLARE @Items TABLE (
        IdProducto INT NOT NULL,
        Cantidad DECIMAL(12,2) NOT NULL,
        PrecioUnitario DECIMAL(10,2) NOT NULL
    );

    INSERT INTO @Items (IdProducto, Cantidad, PrecioUnitario)
    SELECT
        j.IdProducto,
        j.Cantidad,
        j.PrecioUnitario
    FROM OPENJSON(@ItemsJson)
    WITH (
        IdProducto INT '$.IdProducto',
        Cantidad DECIMAL(12,2) '$.Cantidad',
        PrecioUnitario DECIMAL(10,2) '$.PrecioUnitario'
    ) j;

    IF NOT EXISTS (SELECT 1 FROM @Items)
        THROW 51003, 'La venta debe tener al menos 1 ítem.', 1;

    IF EXISTS (SELECT 1 FROM @Items WHERE Cantidad <= 0 OR PrecioUnitario <= 0)
        THROW 51004, 'Cantidad y PrecioUnitario deben ser mayores a 0.', 1;

    -- Validar productos activos
    IF EXISTS (
        SELECT 1
        FROM @Items i
        LEFT JOIN dbo.Producto p ON p.IdProducto = i.IdProducto AND p.Activo = 1
        WHERE p.IdProducto IS NULL
    )
        THROW 51005, 'Hay ítems con productos inexistentes o inactivos.', 1;

    BEGIN TRAN;

    -- Asegurar que exista fila en StockProducto para todos los productos
    IF EXISTS (
        SELECT 1
        FROM @Items i
        LEFT JOIN dbo.StockProducto sp ON sp.IdProducto = i.IdProducto
        WHERE sp.IdProducto IS NULL
    )
        THROW 51006, 'Hay productos sin stock inicial cargado. Primero use sp_EntradaStock.', 1;

    -- ✅ Validar stock suficiente (agregado por producto) SIN CTE
    IF EXISTS (
        SELECT 1
        FROM (
            SELECT IdProducto, SUM(Cantidad) AS CantReq
            FROM @Items
            GROUP BY IdProducto
        ) r
        JOIN dbo.StockProducto sp WITH (UPDLOCK, HOLDLOCK)
            ON sp.IdProducto = r.IdProducto
        WHERE sp.StockActual < r.CantReq
    )
        THROW 51007, 'Stock insuficiente para uno o más productos.', 1;

    DECLARE @IdVenta INT;

    INSERT INTO dbo.Venta (IdCliente, Total)
    VALUES (@IdCliente, 0);

    SET @IdVenta = SCOPE_IDENTITY();

    -- Insertar detalles con NroItem incremental
    ;WITH x AS (
        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS NroItem,
            IdProducto, Cantidad, PrecioUnitario
        FROM @Items
    )
    INSERT INTO dbo.DetalleVenta (IdVenta, NroItem, IdProducto, Cantidad, PrecioUnitario)
    SELECT @IdVenta, NroItem, IdProducto, Cantidad, PrecioUnitario
    FROM x;

    -- Descontar stock por producto (agregado)
    UPDATE sp
    SET sp.StockActual = sp.StockActual - r.CantReq
    FROM dbo.StockProducto sp
    JOIN (
        SELECT IdProducto, SUM(Cantidad) AS CantReq
        FROM @Items
        GROUP BY IdProducto
    ) r ON r.IdProducto = sp.IdProducto;

    -- Movimientos de salida (uno por producto, agregado)
    INSERT INTO dbo.MovimientoStock (IdProducto, TipoMovimiento, Cantidad, Motivo, IdVenta)
    SELECT
        r.IdProducto,
        'S',
        r.CantReq,
        'Venta',
        @IdVenta
    FROM (
        SELECT IdProducto, SUM(Cantidad) AS CantReq
        FROM @Items
        GROUP BY IdProducto
    ) r;

    -- Calcular total desde detalle
    UPDATE v
    SET Total = x.TotalCalc
    FROM dbo.Venta v
    CROSS APPLY (
        SELECT SUM(CAST(dv.Subtotal AS DECIMAL(12,2))) AS TotalCalc
        FROM dbo.DetalleVenta dv
        WHERE dv.IdVenta = v.IdVenta
    ) x
    WHERE v.IdVenta = @IdVenta;

    COMMIT;

    SELECT @IdVenta AS IdVenta;
END
GO
CREATE OR ALTER PROCEDURE dbo.sp_Venta_Crear
    @IdCliente INT = NULL,
    @IdVenta INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @IdCliente IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE IdCliente = @IdCliente AND Activo = 1)
        THROW 52001, 'Cliente inexistente o inactivo.', 1;

    INSERT INTO dbo.Venta (IdCliente, Total)
    VALUES (@IdCliente, 0);

    SET @IdVenta = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Venta_AgregarItem
    @IdVenta INT,
    @IdProducto INT,
    @Cantidad DECIMAL(12,2),
    @PrecioUnitario DECIMAL(10,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Venta WHERE IdVenta = @IdVenta)
        THROW 52002, 'La venta no existe.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.Producto WHERE IdProducto = @IdProducto AND Activo = 1)
        THROW 52003, 'Producto inexistente o inactivo.', 1;

    IF @Cantidad <= 0
        THROW 52004, 'La cantidad debe ser mayor a 0.', 1;

    DECLARE @PU DECIMAL(10,2);

    SET @PU = COALESCE(@PrecioUnitario, (SELECT PrecioActual FROM dbo.Producto WHERE IdProducto = @IdProducto));

    IF @PU IS NULL OR @PU <= 0
        THROW 52005, 'Precio unitario inválido.', 1;

    DECLARE @NroItem SMALLINT;

    SELECT @NroItem = ISNULL(MAX(NroItem), 0) + 1
    FROM dbo.DetalleVenta
    WHERE IdVenta = @IdVenta;

    INSERT INTO dbo.DetalleVenta (IdVenta, NroItem, IdProducto, Cantidad, PrecioUnitario)
    VALUES (@IdVenta, @NroItem, @IdProducto, @Cantidad, @PU);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Venta_Confirmar
    @IdVenta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Venta WHERE IdVenta = @IdVenta)
        THROW 52006, 'La venta no existe.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.DetalleVenta WHERE IdVenta = @IdVenta)
        THROW 52007, 'La venta no tiene items.', 1;

    BEGIN TRAN;

    -- Validar que todos tengan fila en StockProducto
    IF EXISTS (
        SELECT 1
        FROM dbo.DetalleVenta dv
        LEFT JOIN dbo.StockProducto sp ON sp.IdProducto = dv.IdProducto
        WHERE dv.IdVenta = @IdVenta
          AND sp.IdProducto IS NULL
    )
        THROW 52008, 'Hay productos sin stock inicial cargado. Use sp_EntradaStock.', 1;

    -- Validar stock suficiente (agregado por producto)
    IF EXISTS (
        SELECT 1
        FROM (
            SELECT IdProducto, SUM(Cantidad) AS CantReq
            FROM dbo.DetalleVenta
            WHERE IdVenta = @IdVenta
            GROUP BY IdProducto
        ) r
        JOIN dbo.StockProducto sp WITH (UPDLOCK, HOLDLOCK)
            ON sp.IdProducto = r.IdProducto
        WHERE sp.StockActual < r.CantReq
    )
        THROW 52009, 'Stock insuficiente para uno o más productos.', 1;

    -- Descontar stock (agregado)
    UPDATE sp
    SET sp.StockActual = sp.StockActual - r.CantReq
    FROM dbo.StockProducto sp
    JOIN (
        SELECT IdProducto, SUM(Cantidad) AS CantReq
        FROM dbo.DetalleVenta
        WHERE IdVenta = @IdVenta
        GROUP BY IdProducto
    ) r ON r.IdProducto = sp.IdProducto;

    -- Registrar movimientos (uno por producto)
    INSERT INTO dbo.MovimientoStock (IdProducto, TipoMovimiento, Cantidad, Motivo, IdVenta)
    SELECT
        r.IdProducto, 'S', r.CantReq, 'Venta', @IdVenta
    FROM (
        SELECT IdProducto, SUM(Cantidad) AS CantReq
        FROM dbo.DetalleVenta
        WHERE IdVenta = @IdVenta
        GROUP BY IdProducto
    ) r;

    -- Calcular total
    UPDATE v
    SET Total = x.TotalCalc
    FROM dbo.Venta v
    CROSS APPLY (
        SELECT SUM(CAST(Subtotal AS DECIMAL(12,2))) AS TotalCalc
        FROM dbo.DetalleVenta
        WHERE IdVenta = @IdVenta
    ) x
    WHERE v.IdVenta = @IdVenta;

    COMMIT;

    SELECT @IdVenta AS IdVentaConfirmada;
END
GO


CREATE TABLE dbo.VentaImport (
    RowId         BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ImportadoEn   DATETIME2(0) NOT NULL CONSTRAINT DF_VentaImport_ImportadoEn DEFAULT (SYSDATETIME()),
    Procesado     BIT NOT NULL CONSTRAINT DF_VentaImport_Procesado DEFAULT (0),
    ErrorMsg      NVARCHAR(300) NULL,

    FechaHora      DATETIME2(0) NOT NULL,
    TicketNro      INT NOT NULL,
    Producto       VARCHAR(80) NOT NULL,
    Cantidad       DECIMAL(12,2) NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL
);
GO

-- Ajuste de precisión para precios unitarios
ALTER TABLE dbo.Producto
ALTER COLUMN PrecioActual DECIMAL(18,4) NOT NULL;

ALTER TABLE dbo.DetalleVenta
ALTER COLUMN PrecioUnitario DECIMAL(18,4) NOT NULL;

-- El subtotal y total pueden quedarse en 2 decimales si es moneda final,
-- pero internamente 4 decimales da menos errores de redondeo.

-- 1. Borramos la columna calculada que causa el conflicto
ALTER TABLE dbo.DetalleVenta
DROP COLUMN Subtotal;
GO

-- 2. Ahora sí, modificamos las columnas "padre" a la nueva precisión
ALTER TABLE dbo.Producto
ALTER COLUMN PrecioActual DECIMAL(18,4) NOT NULL;

ALTER TABLE dbo.DetalleVenta
ALTER COLUMN PrecioUnitario DECIMAL(18,4) NOT NULL;
GO

-- 3. Volvemos a crear la columna calculada (ahora usará la nueva precisión)
ALTER TABLE dbo.DetalleVenta
ADD Subtotal AS (Cantidad * PrecioUnitario) PERSISTED;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Venta_Confirmar
    @IdVenta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; -- Si hay error, deshace todo automáticamente

    -- 1. Validaciones básicas
    IF NOT EXISTS (SELECT 1 FROM dbo.Venta WHERE IdVenta = @IdVenta)
        THROW 52006, 'La venta no existe.', 1;

    -- Usamos una variable de tabla para guardar lo que necesitamos descontar
    -- Esto hace el código más limpio y rápido que hacer subconsultas repetidas
    DECLARE @Requerimientos TABLE (IdProducto INT, CantReq DECIMAL(12,2));

    INSERT INTO @Requerimientos (IdProducto, CantReq)
    SELECT IdProducto, SUM(Cantidad)
    FROM dbo.DetalleVenta
    WHERE IdVenta = @IdVenta
    GROUP BY IdProducto;

    IF NOT EXISTS (SELECT 1 FROM @Requerimientos)
        THROW 52007, 'La venta no tiene items.', 1;

    BEGIN TRAN;

    -- 2. Validar que existan en StockProducto (Integridad)
    IF EXISTS (
        SELECT 1
        FROM @Requerimientos r
        LEFT JOIN dbo.StockProducto sp ON sp.IdProducto = r.IdProducto
        WHERE sp.IdProducto IS NULL
    )
        THROW 52008, 'Hay productos sin stock inicial cargado. Use sp_EntradaStock.', 1;

    -- 3. EL CAMBIO CLAVE: Actualización Atómica
    -- Intentamos actualizar el stock DIRECTAMENTE, pero solo donde alcance.
    -- No usamos SELECT previo, ni HOLDLOCK.
    
    UPDATE sp
    SET sp.StockActual = sp.StockActual - r.CantReq
    FROM dbo.StockProducto sp
    INNER JOIN @Requerimientos r ON r.IdProducto = sp.IdProducto
    WHERE sp.StockActual >= r.CantReq; -- <--- ESTA CONDICIÓN ES EL GUARDIÁN

    -- 4. Verificación Post-Mortem
    -- Si teníamos 3 productos para vender, el UPDATE debió afectar 3 filas.
    -- Si afectó 2, significa que 1 no tenía stock suficiente.
    DECLARE @ItemsRequeridos INT = (SELECT COUNT(*) FROM @Requerimientos);
    DECLARE @ItemsActualizados INT = @@ROWCOUNT;

    IF @ItemsActualizados < @ItemsRequeridos
    BEGIN
        -- Falló alguno. Como XACT_ABORT está ON, el THROW hará rollback automático.
        THROW 52009, 'Stock insuficiente para uno o más productos. La operación fue cancelada.', 1;
    END

    -- 5. Registrar movimientos (Histórico)
    INSERT INTO dbo.MovimientoStock (IdProducto, TipoMovimiento, Cantidad, Motivo, IdVenta)
    SELECT
        r.IdProducto, 'S', r.CantReq, 'Venta', @IdVenta
    FROM @Requerimientos r;

    -- 6. Calcular total final (Aseguramos que el encabezado quede perfecto)
    UPDATE v
    SET Total = x.TotalCalc,
        Estado = 'Confirmada' -- Asumiendo que agregaste la columna Estado
    FROM dbo.Venta v
    CROSS APPLY (
        SELECT SUM(Subtotal) AS TotalCalc
        FROM dbo.DetalleVenta
        WHERE IdVenta = @IdVenta
    ) x
    WHERE v.IdVenta = @IdVenta;

    COMMIT;

    SELECT @IdVenta AS IdVentaConfirmada;
END
GO