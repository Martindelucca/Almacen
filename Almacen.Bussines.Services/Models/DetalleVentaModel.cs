// Almacen.Business/Models/DetalleVentaModel.cs
namespace Almacen.Business.Models
{
    public class DetalleVentaModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }

        // Propiedad calculada: La vista la ama, la base de datos la ignora
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}