using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Almacen.Core.Interfaces;
using Dapper;

namespace Almacen.Data.Repositories
{
    public class VentaRepository : SqlRepository, IVentaRepository
    {
        public VentaRepository(string connectionString) : base(connectionString) { }
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
