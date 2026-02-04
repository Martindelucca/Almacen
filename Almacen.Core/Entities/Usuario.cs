using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Almacen.Core.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public string Clave { get; set; } = string.Empty; // Hash

        [Required]
        public string Rol { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}
