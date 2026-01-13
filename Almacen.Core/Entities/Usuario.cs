using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string Clave { get; set; } // Aquí viaja el Hash
        public string Rol { get; set; } // "Admin" o "Cajero"
        public bool Activo { get; set; }
    }
}
