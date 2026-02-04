using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Dtos
{
    public class VentaResumenDto
    {
        public int IdVenta { get; set; }
        public DateTime FechaHora { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int CantidadItems { get; set; }
        public string Estado { get; set; }
    }
}
