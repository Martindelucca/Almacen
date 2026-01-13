using Almacen.Core.Entities;
using Dapper;
using Almacen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Almacen.Data.Repositories
{
    public class UsuarioRepository : SqlRepository, IUsuarioRepository
    {
        public UsuarioRepository(string connectionString) : base(connectionString) { }

        public async Task<Usuario> ObtenerPorNombreAsync(string nombreUsuario)
        {
            using var connection = CreateConnection();
            // Solo buscamos por nombre y que esté activo
            string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = @nombre AND Activo = 1";

            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { nombre = nombreUsuario });
        }

    }
}
