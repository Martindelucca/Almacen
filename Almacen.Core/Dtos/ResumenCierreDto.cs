using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Dtos
{
    public class ResumenCierreDto
    {
        public decimal MontoInicial { get; set; }
        public decimal TotalVentasEfectivo { get; set; }
        public decimal TotalEsperado { get; set; } // Inicial + Ventas
    }
}

