using Almacen.Core.Entities;
using Almacen.Core.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Data.Repositories
{
    public class CajaRepository : SqlRepository, ICajaRepository
    {
        public CajaRepository(string connectionString) : base(connectionString) { }

        public async Task<SesionCaja> ObtenerSesionAbiertaAsync(int idUsuario)
        {
            using var db = CreateConnection();
            string sql = "SELECT TOP 1 * FROM SesionesCaja WHERE IdUsuario = @IdUsuario AND Estado = 1";
            return await db.QueryFirstOrDefaultAsync<SesionCaja>(sql, new { IdUsuario = idUsuario });
        }

        // 2. Abrir Caja
        public async Task<int> AbrirCajaAsync(int idUsuario, decimal montoInicial)
        {
            using var db = CreateConnection();
            string sql = @"
            INSERT INTO SesionesCaja (IdUsuario, FechaApertura, MontoInicial, Estado)
            VALUES (@IdUsuario, GETDATE(), @MontoInicial, 1);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return await db.ExecuteScalarAsync<int>(sql, new { IdUsuario = idUsuario, MontoInicial = montoInicial });
        }

        // 3. Obtener Total Ventas de la Sesión Actual (Para el cierre)
        // Sumamos solo las ventas ACTIVAS (no anuladas) de esta sesión
        public async Task<decimal> ObtenerTotalVentasSesionAsync(int idSesion)
        {
            using var db = CreateConnection();
            // Asumiendo que 'Estado' en Venta es 'Anulada'/'Completada' o usas Activa=1
            // Ajusta el WHERE según tu estructura real de Venta
            string sql = @"
            SELECT ISNULL(SUM(Total), 0) 
            FROM Venta
            WHERE IdSesion = @IdSesion AND Estado != 'Anulada'";

            return await db.ExecuteScalarAsync<decimal>(sql, new { IdSesion = idSesion });
        }

        // 4. Cerrar Caja
        public async Task CerrarCajaAsync(int idSesion, decimal sistema, decimal real, decimal diferencia)
        {
            using var db = CreateConnection();
            string sql = @"
            UPDATE SesionesCaja
            SET FechaCierre = GETDATE(),
                MontoFinalSistema = @Sistema,
                MontoFinalReal = @Real,
                Diferencia = @Diferencia,
                Estado = 0
            WHERE IdSesion = @IdSesion";

            await db.ExecuteAsync(sql, new { IdSesion = idSesion, Sistema = sistema, Real = real, Diferencia = diferencia });
        }
    }
}
