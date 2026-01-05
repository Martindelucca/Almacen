using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Almacen.Core.Dtos;
using Almacen.Core.Interfaces;
using Dapper;

namespace Almacen.Data.Repositories
{
    public class VentaRepository : SqlRepository, IVentaRepository
    {
        public VentaRepository(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<DetalleVentaDto>> ObtenerDetalleDeVentaAsync(int idVenta)
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT
                  p.Nombre as Producto,
                  dv.Cantidad,
                  dv.PrecioUnitario
                FROM dbo.DetalleVenta dv
                LEFT JOIN dbo.Producto p ON dv.IdProducto = p.IdProducto
                WHERE dv.IdVenta = @IdVenta";
            return await connection.QueryAsync<DetalleVentaDto>(sql, new { IdVenta = idVenta });
        }

        public async Task<IEnumerable<VentaResumenDto>> ObtenerVentasRecientesAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
        SELECT TOP 50
            v.IdVenta,
            v.FechaHora,
            v.Total,
            ISNULL(c.Nombre, 'Consumidor Final') as ClienteNombre,
            (SELECT COUNT(*) FROM dbo.DetalleVenta dv WHERE dv.IdVenta = v.IdVenta) as CantidadItems
        FROM dbo.Venta v  
        LEFT JOIN dbo.Cliente c ON v.IdCliente = c.IdCliente
        ORDER BY v.FechaHora DESC"; // <--- Asegúrate de que esta línea esté presente

            return await connection.QueryAsync<VentaResumenDto>(sql);
        }

        public async Task<int> RegistrarVentaAsync(int? idCliente, string itemsJson)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IdCliente", idCliente);
            parameters.Add("@ItemsJson", itemsJson);
            // Llamamos a tu SP sp_RegistrarVenta
            var idVenta = await connection.ExecuteScalarAsync<int>(
                "dbo.sp_RegistrarVentaJson",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return idVenta;

        }

    }
}
