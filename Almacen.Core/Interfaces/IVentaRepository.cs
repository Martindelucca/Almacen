using Almacen.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Interfaces
{
    public interface IVentaRepository
    {
        // Recibe el JSON ya formateado. El repo no debe saber de lógica de negocio, solo de transporte.
        Task<int> RegistrarVentaAsync(int? idCliente, string itemsJson);
        Task<IEnumerable<VentaResumenDto>> ObtenerVentasRecientesAsync();
        Task<IEnumerable<DetalleVentaDto>> ObtenerDetalleDeVentaAsync(int idVenta);

        Task AnularVentaAsync(int idVenta, string motivo);
    }
}
