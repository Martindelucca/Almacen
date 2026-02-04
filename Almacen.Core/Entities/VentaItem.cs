using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Entities
{
   // public sealed class VentaItem(int IdVenta, int NroItem, int IdProducto, int Cantidad, decimal PrecioUnitario);
   public class VentaItem
    {
        public int IdVenta { get; set; }
        public int NroItem { get; set; }
        public int IdProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

}
