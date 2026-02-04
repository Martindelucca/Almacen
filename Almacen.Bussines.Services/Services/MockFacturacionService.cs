using Almacen.Core.Entities;
using Almacen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Almacen.Core.Dtos;

namespace Almacen.Business.Services
{
    public class MockFacturacionService : IFacturacionService
    {
        public async Task<ResultadoFacturacion> FacturarVentasAsync(int idVenta, decimal total, long? cuitCliente)
        {
            // SIMULAMOS LA ESPERA DEL SERVIDOR DE AFIP (Lag real)
            await Task.Delay(1500);

            // Simulamos una respuesta EXITOSA de ARCA
            // En el futuro, aquí va el código real de AFIP SDK
            return new ResultadoFacturacion
            {
                Exito = true,
                CAE = "74152639845125", // Un CAE falso pero con formato válido
                VencimientoCAE = DateTime.Now.AddDays(10),
                NumeroComprobante = $"0001-{idVenta:D8}" // Ej: 0001-00000009
            };
        }
    }
}
