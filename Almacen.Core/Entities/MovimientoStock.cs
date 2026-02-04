using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Entities
{
    public class MovimientoStock
    {
        public int IdMovimiento { get; set; }
        public int IdProducto { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public decimal Cantidad { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public int? IdVenta { get; set; } // Nullable
    }
}
