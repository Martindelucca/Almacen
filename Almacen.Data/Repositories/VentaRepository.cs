using Almacen.Core.Dtos;
using Almacen.Core.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<VentaResumenDto>> ObtenerVentasRecientesAsync(DateTime fechaSeleccionada)
        {
            using var connection = CreateConnection();
            string sql = @"
        SELECT 
            v.IdVenta,
            v.FechaHora AS Fecha, -- Ajusta si tu DTO espera 'Fecha' o 'FechaHora'
            v.Total,
            ISNULL(c.Nombre, 'Consumidor Final') as ClienteNombre, -- Tu lógica de cliente
            (SELECT COUNT(*) FROM dbo.DetalleVenta dv WHERE dv.IdVenta = v.IdVenta) as CantidadItems,
            v.Estado
        FROM dbo.Venta v  
        LEFT JOIN dbo.Cliente c ON v.IdCliente = c.IdCliente
        WHERE CAST(v.FechaHora AS DATE) = CAST(@FechaParam AS DATE)
        ORDER BY v.FechaHora DESC"; // <--- Asegúrate de que esta línea esté presente

            return await connection.QueryAsync<VentaResumenDto>(sql, new { FechaParam = fechaSeleccionada });
        }

        public async Task<int> RegistrarVentaAsync(int? idCliente, string itemsJson, int? idSesion)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@IdCliente", idCliente);
            parameters.Add("@ItemsJson", itemsJson);
            parameters.Add("IdSesion", idSesion);
            // Llamamos a tu SP sp_RegistrarVenta
            var idVenta = await connection.ExecuteScalarAsync<int>(
                "dbo.sp_RegistrarVentaJson",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure
            );
            return idVenta;

        }
        public async Task AnularVentaAsync(int idVenta,string motivo)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync(
          "dbo.sp_AnularVenta",
          new { IdVenta = idVenta, Motivo = motivo },
          commandType: CommandType.StoredProcedure);
        }

        public async Task ActualizarDatosFacturacionAsync(int idVenta, string cae, DateTime vtoCae, string nroComprobante)
        {
            using var connection = CreateConnection();
            string sql = @"
        UPDATE dbo.Venta 
        SET CAE = @Cae, 
            VencimientoCAE = @Vto, 
            NumeroComprobante = @Nro
        WHERE IdVenta = @Id";

            await connection.ExecuteAsync(sql, new
            {
                Id = idVenta,
                Cae = cae,
                Vto = vtoCae,
                Nro = nroComprobante
            });
        }
    }
}
