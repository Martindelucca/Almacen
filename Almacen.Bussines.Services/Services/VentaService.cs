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
        private readonly ICajaRepository _cajaRepo;
        private readonly IFacturacionService _facturacionService;

        // ESTADO: Aquí vive el carrito temporal
        private readonly List<DetalleVentaModel> _carrito = new List<DetalleVentaModel>();

        public VentaService(IProductoRepository productoRepo, IVentaRepository ventaRepo, ICajaRepository cajaRepo, IFacturacionService facturacionService)
        {
            _productoRepo = productoRepo ?? throw new ArgumentNullException(nameof(productoRepo));
            _ventaRepo = ventaRepo ?? throw new ArgumentNullException(nameof(ventaRepo));
            _cajaRepo = cajaRepo;
            _facturacionService = facturacionService;
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
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0.");

            // 1. Buscamos el producto en la BD (Validación de existencia)
            var producto = await _productoRepo.ObtenerPorIdAsync(idProducto);
            if (producto == null)
                throw new InvalidOperationException("Producto no encontrado.");
            if (!producto.Activo)
                throw new InvalidOperationException("Producto inactivo.");

            // 2. Lógica de "Upsert" (Update or Insert) en memoria
            var itemExistente = _carrito.FirstOrDefault(x => x.IdProducto == idProducto);

            // Validamos stock CONTRA LO QUE YA TIENE EN EL CARRITO + LO NUEVO
            decimal cantidadTotalRequerida = cantidad + (itemExistente?.Cantidad ?? 0);

            // ✅ CRÍTICO: VALIDACIÓN DE STOCK ACTIVADA
            if (producto.StockActual < cantidadTotalRequerida)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente.\n\n" +
                    $"Disponible: {producto.StockActual:N2}\n" +
                    $"Ya en carrito: {itemExistente?.Cantidad ?? 0:N2}\n" +
                    $"Solicitado ahora: {cantidad:N2}\n" +
                    $"Total requerido: {cantidadTotalRequerida:N2}");
            }

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

        public async Task<int> ConfirmarVenta(int? idCliente, int idUsuario)
        {
            if (!_carrito.Any()) throw new InvalidOperationException("Carrito vacío.");

            // 1. Validar Caja (Ahora funcionará bien tras el script SQL)
            var sesion = await _cajaRepo.ObtenerSesionAbiertaAsync(idUsuario);
            if (sesion == null) throw new InvalidOperationException("⛔ DEBES ABRIR CAJA PRIMERO.");

            // 2. Guardar Venta en SQL (Transacción local)
            var json = JsonSerializer.Serialize(_carrito);
            int idVenta = await _ventaRepo.RegistrarVentaAsync(idCliente, json, sesion.IdSesion);

            // 3. FACTURACIÓN ELECTRÓNICA (ARCA/AFIP) 🚀
            try
            {
                decimal totalVenta = ObtenerTotal();
                // Pasamos null en CUIT por ahora (Consumidor Final)
                var resultadoAfip = await _facturacionService.FacturarVentasAsync(idVenta, totalVenta, null);

                if (resultadoAfip.Exito)
                {
                    // Si AFIP aprobó, guardamos el CAE en nuestra BD
                    await _ventaRepo.ActualizarDatosFacturacionAsync(
                        idVenta,
                        resultadoAfip.CAE,
                        resultadoAfip.VencimientoCAE,
                        resultadoAfip.NumeroComprobante
                    );
                }
            }
            catch (Exception ex)
            {
                // OJO: La venta YA se hizo en SQL, pero falló AFIP.
                // En un sistema real, esto se loguea como "Pendiente de Facturar".
                // Para tu proyecto, simplemente no detenemos el flujo, pero podrías avisar.
                System.Diagnostics.Debug.WriteLine($"Error facturando: {ex.Message}");
            }

            // 4. Limpiar y retornar
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

        // Método para resetear (útil para testing o cambio de usuario)
        public void ResetearCarrito()
        {
            _carrito.Clear();
        }

        // Historial de ventas
        public async Task<IEnumerable<VentaResumenDto>> ObtenerHistorialVentas(DateTime fecha)
        {
            return await _ventaRepo.ObtenerVentasRecientesAsync(fecha);
        }

        public async Task<IEnumerable<DetalleVentaDto>> ObtenerDetalleVenta(int idVenta)
        {
            return await _ventaRepo.ObtenerDetalleDeVentaAsync(idVenta);
        }

        public async Task AnularVenta(int idVenta, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Debe especificar un motivo de anulación");

            await _ventaRepo.AnularVentaAsync(idVenta, motivo);
        }
    }
}