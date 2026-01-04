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
    }
}