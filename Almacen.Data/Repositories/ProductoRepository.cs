using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Almacen.Core.Entities;
using Almacen.Core.Interfaces;

namespace Almacen.Data.Repositories
{
    public class ProductoRepository : SqlRepository, IProductoRepository
    {
        public ProductoRepository(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            using var connection = CreateConnection();
            // Usamos tu vista v_StockActual o la tabla directa
            var sql = @"
        SELECT 
            p.IdProducto, 
            p.Nombre, 
            p.PrecioActual, 
            c.Nombre as CategoriaNombre,
            ISNULL(sp.StockActual, 0) as StockActual
        FROM dbo.Producto p 
        INNER JOIN dbo.Categoria c ON p.IdCategoria = c.IdCategoria
        LEFT JOIN dbo.StockProducto sp ON p.IdProducto = sp.IdProducto
        WHERE p.Activo = 1"; ;

            return await connection.QueryAsync<Producto>(sql);
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM dbo.Producto WHERE IdProducto = @Id";
            return await connection.QueryFirstOrDefaultAsync<Producto>(sql, new { Id = id });
        }

        public async Task<int> EntradaStockAsync(int idProducto, decimal cantidad, decimal? stockMinimo, string motivo)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IdProducto", idProducto);
            parameters.Add("@Cantidad", cantidad);
            parameters.Add("@StockMinimo", stockMinimo);
            parameters.Add("@Motivo", motivo);

            // Llamamos a tu SP sp_EntradaStock
            // ExecuteAsync devuelve el número de filas afectadas
            await connection.ExecuteAsync(
                "dbo.sp_EntradaStock",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return idProducto;
        }

        public async  Task<int> CrearProductoAsync(Producto producto)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IdCategoria", producto.IdCategoria);
            parameters.Add("@Nombre", producto.Nombre);
            parameters.Add("@PrecioActual", producto.PrecioActual);
            parameters.Add("@IdProductoGenerado", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("dbo.sp_Producto_Crear", parameters, commandType: CommandType.StoredProcedure);

            return parameters.Get<int>("@IdProductoGenerado");
        }

        public async Task ModificarProductoAsync(Producto producto)
        {
            using var connection = CreateConnection();
            // Reutilizamos el objeto producto completo
            await connection.ExecuteAsync("dbo.sp_Producto_Modificar", new
            {
                producto.IdProducto,
                producto.IdCategoria,
                producto.Nombre,
                producto.PrecioActual,
                producto.Activo
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task EliminarProductoAsync(int id)
        {
            using var connection = CreateConnection();
            // TRUCO SENIOR: No hacemos DELETE. Hacemos UPDATE Activo = 0
            // Pero ojo: necesitamos saber el resto de datos para no romperlos, 
            // o hacemos un query específico solo para desactivar.
            // Para simplificar, haré una query directa aquí (aunque lo ideal es un SP específico).

            var sql = "UPDATE dbo.Producto SET Activo = 0 WHERE IdProducto = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasAsync()
        {
            using var connection = CreateConnection();

            // NO uses 'SELECT *'. A veces trae columnas en orden raro.
            // Usa los nombres explícitos para asegurar que Dapper los encuentre.
            var sql = "SELECT IdCategoria, Nombre FROM dbo.Categoria";

            return await connection.QueryAsync<Categoria>(sql);
        }

        public async Task AgregarStockAsync(int idProducto, decimal cantidad, string motivo)
        {
            using var connection = CreateConnection();
            // Llamamos a tu SP existente
            await connection.ExecuteAsync("dbo.sp_EntradaStock", new
            {
                IdProducto = idProducto,
                Cantidad = cantidad,
                StockMinimo = 0, // Opcional si el SP lo pide
                Motivo = motivo
            }, commandType: CommandType.StoredProcedure);
        }
    }
}