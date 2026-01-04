using Almacen.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks; // ¡OJO! Vamos a ser asíncronos desde el día 1.
using System;


namespace Almacen.Core.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<int> EntradaStockAsync(int idProducto, decimal cantidad, decimal? stockMinimo, string motivo);
        // Aquí no ponemos "InsertarProducto" porque tu lógica de negocio dicta que
        // los productos se gestionan vía stock o procesos administrativos.
        // Pero para el ABM básico, podrías tener CreateAsync.
    }
}
