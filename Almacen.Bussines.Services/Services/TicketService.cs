using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

// Asegúrate de usar tus namespaces correctos
using Almacen.Core.Entities;
using Almacen.Core.Dtos;

namespace Almacen.UI.Services // O el namespace que uses
{
    public class TicketService
    {
        public void GenerarTicket(TicketData datos, List<TicketDetalle> detalles)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.ContinuousSize(80, Unit.Millimetre);
                    page.Margin(5, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.SegoeUI));

                    // Encabezado
                    page.Header().Column(col =>
                    {
                        col.Item().Text("ALMACEN").SemiBold().FontSize(14).AlignCenter();
                        col.Item().Text($"Fecha: {datos.Fecha:dd/MM/yyyy HH:mm}").FontSize(8).AlignCenter();
                        col.Item().Text("--------------------------------").AlignCenter();
                    });

                    // Contenido
                    page.Content().PaddingVertical(5).Column(col =>
                    {
                        col.Item().Text($"Ticket Nro: {datos.NroTicket}").Bold();
                        col.Item().Text($"Cliente: {datos.Cliente}");

                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(20);
                                columns.RelativeColumn();
                                columns.ConstantColumn(40);
                            });

                            // Filas
                            foreach (var item in detalles)
                            {
                                tabla.Cell().Text(item.Cantidad.ToString());
                                tabla.Cell().Text(item.Producto); // <--- Ahora usa la propiedad correcta
                                tabla.Cell().AlignRight().Text($"$ {item.Subtotal:N2}");
                            }
                        });
                        col.Item().Text("--------------------------------").AlignCenter();
                    });

                    // Pie
                    page.Footer().Column(col =>
                    {
                        col.Item().AlignRight().Text($"TOTAL: $ {datos.Total:N2}").FontSize(14).Bold();
                        col.Item().AlignRight().Text($"Pago: $ {datos.PagoCon:N2}");
                        col.Item().AlignRight().Text($"Vuelto: $ {datos.Vuelto:N2}");
                        col.Item().PaddingTop(5).Text("Gracias por su compra!").AlignCenter();
                    });
                });
            });

            string ruta = Path.Combine(Path.GetTempPath(), $"Ticket_{datos.NroTicket}.pdf");
            documento.GeneratePdf(ruta);
            Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }
        // Clase para la cabecera del ticket
        public class TicketData
        {
            public int NroTicket { get; set; }
            public decimal Total { get; set; }
            public decimal PagoCon { get; set; }
            public decimal Vuelto { get; set; }
            public string Cliente { get; set; }
            public DateTime Fecha { get; set; }
        }

        // Clase para cada renglón de productos
        public class TicketDetalle
        {
            public string Producto { get; set; }
            public int Cantidad { get; set; }
            public decimal Subtotal { get; set; }
        }
    }
}