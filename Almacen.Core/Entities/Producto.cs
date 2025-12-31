// Almacen.Core/Entities/Producto.cs
using System;

namespace Almacen.Core.Entities
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty; // Evita nulos por defecto
        public decimal PrecioActual { get; set; }
        public bool Activo { get; set; }

        // Propiedades de navegación (Opcional, útil para Dapper multimap)
        public string CategoriaNombre { get; set; }
    }
}