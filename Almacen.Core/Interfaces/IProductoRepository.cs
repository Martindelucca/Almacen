using Almacen.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks; // ¡OJO! Vamos a ser asíncronos desde el día 1.
using System;


namespace Almacen.Core.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAllAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<int> EntradaStockAsync(int idProducto, decimal cantidad, decimal? stockMinimo, string motivo);
        Task<int> CrearProductoAsync(Producto producto);
        Task ModificarProductoAsync(Producto producto);
        Task EliminarProductoAsync(int id); // Esto será el Soft Delete internamente
        Task<IEnumerable<Categoria>> ObtenerCategoriasAsync();
        Task AgregarStockAsync(int idProducto, decimal cantidad, string motivo);
    }
}