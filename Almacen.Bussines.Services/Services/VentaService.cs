using Almacen.Business.Models;
using Almacen.Core.Dtos;
using Almacen.Core.Entities;
using Almacen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Almacen.Business.Services
{
    public class VentaService
    {
        private readonly IProductoRepository _productoRepo;
        private readonly IVentaRepository _ventaRepo;

        // ESTADO: Aquí vive el carrito temporal
        private readonly List<DetalleVentaModel> _carrito = new List<DetalleVentaModel>();

        public VentaService(IProductoRepository productoRepo, IVentaRepository ventaRepo)
        {
            _productoRepo = productoRepo;
            _ventaRepo = ventaRepo;
        }
        public async Task<IEnumerable<Producto>> ObtenerTodosLosProductos()
        {
            return await _productoRepo.GetAllAsync();
        }

        // Método para obtener la lista (solo lectura para la UI)
        public IReadOnlyList<DetalleVentaModel> ObtenerCarrito() => _carrito.AsReadOnly();

        public decimal ObtenerTotal() => _carrito.Sum(x => x.Subtotal);

        public async Task AgregarProductoAlCarrito(int idProducto, decimal cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a 0.");

            // 1. Buscamos el producto en la BD (Validación de existencia)
            var producto = await _productoRepo.GetByIdAsync(idProducto);
            if (producto == null) throw new Exception("Producto no encontrado.");
            if (!producto.Activo) throw new Exception("Producto inactivo.");

            // 2. Lógica de "Upsert" (Update or Insert) en memoria
            var itemExistente = _carrito.FirstOrDefault(x => x.IdProducto == idProducto);

            // Validamos stock CONTRA LO QUE YA TIENE EN EL CARRITO + LO NUEVO
            decimal cantidadTotalRequerida = cantidad + (itemExistente?.Cantidad ?? 0);

            // OJO: Aquí asumimos que StockActual es confiable. 
            // En alta concurrencia esto puede fallar, pero el SP final lo validará de nuevo.
            /* NOTA PARA EL USER: Tu clase Producto necesita tener la propiedad StockActual mapeada 
               desde la base de datos para que esto funcione.
            */
            // if (producto.StockActual < cantidadTotalRequerida) 
            //     throw new Exception($"Stock insuficiente. Stock: {producto.StockActual}, En carrito: {cantidadTotalRequerida}");

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                _carrito.Add(new DetalleVentaModel
                {
                    IdProducto = producto.IdProducto,
                    NombreProducto = producto.Nombre,
                    PrecioUnitario = producto.PrecioActual,
                    Cantidad = cantidad
                });
            }
        }

        public async Task<int> ConfirmarVenta(int? idCliente)
        {
            if (!_carrito.Any())
                throw new InvalidOperationException("El carrito está vacío.");

            // 1. Convertir la lista de objetos C# a String JSON
            // Tu SP espera: [{"IdProducto":1, "Cantidad":2, "PrecioUnitario":100}, ...]
            var json = JsonSerializer.Serialize(_carrito);

            // 2. Enviar a la base de datos
            int idVenta = await _ventaRepo.RegistrarVentaAsync(idCliente, json);

            // 3. Si todo salió bien, limpiar el carrito
            _carrito.Clear();

            return idVenta;
        }

        public void QuitarProducto(int idProducto)
        {
            var item = _carrito.FirstOrDefault(x => x.IdProducto == idProducto);
            if (item != null) _carrito.Remove(item);
        }

        public void LimpiarCarrito()
        {
            _carrito.Clear();
        }

        // Historial de ventas
        public async Task<IEnumerable<VentaResumenDto>> ObtenerHistorialVentas()
        {
            return await _ventaRepo.ObtenerVentasRecientesAsync();
        }

        public async Task<IEnumerable<DetalleVentaDto>> ObtenerDetalleVenta(int idVenta)
        {
            return await _ventaRepo.ObtenerDetalleDeVentaAsync(idVenta);
        }
        public async Task AnularVenta(int idVenta, string motivo)
        {
            // Aquí podrías validar que el motivo no esté vacío si quieres
            await _ventaRepo.AnularVentaAsync(idVenta, motivo);
        }
    }
}