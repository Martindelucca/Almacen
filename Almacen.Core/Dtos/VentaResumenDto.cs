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
        public string ClienteNombre { get; set; }
        public decimal Total { get; set; }
        public int cantidadItems { get; set; }
        public string Estado { get; set; }
    }
}
