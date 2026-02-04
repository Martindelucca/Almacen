using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Entities
{
    public class ResultadoFacturacion
    {
        public bool Exito { get; set; }
        public string CAE { get; set; }
        public System.DateTime VencimientoCAE { get; set; }
        public string NumeroComprobante { get; set; } // El número oficial AFIP
        public string ErrorMsg { get; set; }
    }
}
