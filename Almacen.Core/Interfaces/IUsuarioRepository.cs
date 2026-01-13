using Almacen.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Core.Interfaces
{
    
    public interface IUsuarioRepository
    {
     
        Task<Usuario> ObtenerPorNombreAsync(string nombreUsuario);
    }
}
