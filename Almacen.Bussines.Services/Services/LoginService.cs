using Almacen.Core.Entities;
using Almacen.Core.Interfaces;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Almacen.Business.Services
{
    public class LoginService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        public LoginService(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }
        public async Task<Usuario> ValidarUsuarioAsync(string usuario, string claveIngresada)
        {
            // 1. Buscamos si el usuario existe en BD
            var usuarioEncontrado = await _usuarioRepo.ObtenerPorNombreAsync(usuario);

            if (usuarioEncontrado == null) return null; // Usuario no existe

            // 2. LA MAGIA: Comparamos la clave ingresada (texto plano) contra el Hash de la BD
            // BCrypt hace la matemática compleja por nosotros.
            bool esValida = BCrypt.Net.BCrypt.Verify(claveIngresada, usuarioEncontrado.Clave);

            if (esValida)
            {
                return usuarioEncontrado; // ¡Pase usted!
            }
            else
            {
                return null; // Contraseña incorrecta
            }
        }

        // Método auxiliar para crear usuarios nuevos (lo usaremos pronto)
        public string EncriptarClave(string claveTextoPlano)
        {
            return BCrypt.Net.BCrypt.HashPassword(claveTextoPlano);
        }
    }
}
