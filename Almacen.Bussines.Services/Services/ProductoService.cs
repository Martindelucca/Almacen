using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Almacen.Core.Interfaces;
using Almacen.Core.Entities;


namespace Almacen.Business.Services
{
    public  class ProductoService
    {
        private readonly IProductoRepository _productoRepo;
        public ProductoService(IProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }
        public async Task<IEnumerable<Producto>> ObtenerTodos()
        {
            return await _productoRepo.GetAllAsync();
        }

        public async Task<Producto> ObtenerPorId(int id)
        {
            return await _productoRepo.ObtenerPorIdAsync(id);
        }

        public async Task <int> CrearProducto(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new ArgumentException("El nombre del producto no puede estar vacío.");
            if (producto.PrecioActual < 0)
                throw new ArgumentException("El precio del producto no puede ser negativo.");
            if (producto.Nombre.Length > 100)
                throw new ArgumentException("El nombre no puede superar los 100 caracteres.");
            if (producto.IdCategoria <= 0)
                throw new ArgumentException("Debe seleccionar una categoría válida.");
             return await _productoRepo.CrearProductoAsync(producto);
        }

        public async Task ModificarProducto (Producto producto)
        {
            if (producto.IdProducto <= 0) 
                throw new ArgumentException("No se puede editar un producto sin ID");
            if (string.IsNullOrEmpty(producto.Nombre))
                throw new ArgumentException("El nombre es obligatorio");
            if (producto.PrecioActual < 0)
                throw new ArgumentException("El precio no puede ser negativo");
            await _productoRepo.ModificarProductoAsync(producto);
        }

        public async Task EliminarProducto (int idProducto)
        {
            if (idProducto <= 0)
                throw new ArgumentException("ID de producto inválido");
            await _productoRepo.EliminarProductoAsync(idProducto);
        }
        public async Task<IEnumerable<Categoria>> ObtenerCategorias()
        {
            return await _productoRepo.ObtenerCategoriasAsync();
        }
        public async Task SumarStock(int idProducto, decimal cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a 0.");
            await _productoRepo.AgregarStockAsync(idProducto, cantidad, "Compra Proveedor (Manual)");
        }
    }
}
