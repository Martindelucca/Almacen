using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Entities
{
    public class SesionCaja
    {
        public int IdSesion { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal? MontoFinalSistema { get; set; }
        public decimal? MontoFinalReal { get; set; }
        public decimal? Diferencia { get; set; }
        public bool Estado { get; set; } // true = Abierta
    }
}
