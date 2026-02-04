using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Dtos
{
    public class VentasPorFechaDto
    {
        public string FechaLabel { get; set; } // Ej: "05/02" o "Enero"
        public decimal TotalVendido { get; set; }
        public int CantidadVentas { get; set; }
    }
}
