using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Almacen.UI.Services
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
                    page.Margin(4, Unit.Millimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.SegoeUI));

                    // --- ENCABEZADO ---
                    page.Header().Column(col =>
                    {
                        col.Item().Text("MI ALMACEN UTN").SemiBold().FontSize(16).AlignCenter();
                        col.Item().Text("Av. Universidad 500 - Córdoba").FontSize(8).AlignCenter();
                        col.Item().Text("IVA Responsable Inscripto").FontSize(7).AlignCenter();
                        col.Item().Text("--------------------------------").AlignCenter();

                        col.Item().Text($"Ticket Nro: {datos.NroTicket}").Bold();
                        col.Item().Text($"Fecha: {datos.Fecha:dd/MM/yyyy HH:mm}");

                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().Text($"Cliente: {datos.ClienteNombre}");

                        col.Item().LineHorizontal(1).LineColor(Colors.Black);
                        col.Item().PaddingBottom(5);
                    });

                    // --- TABLA DE PRODUCTOS ---
                    page.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(25); // Cantidad
                            columns.RelativeColumn();   // Producto
                            columns.ConstantColumn(45); // Total
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Text("Cant").Bold().FontSize(8);
                            header.Cell().Text("Producto").Bold().FontSize(8);
                            header.Cell().AlignRight().Text("Total").Bold().FontSize(8);
                        });

                        foreach (var item in detalles)
                        {
                            // ✅ CRÍTICO: Formato correcto para cantidad decimal
                            tabla.Cell().PaddingBottom(2).Text($"{item.Cantidad:N2} x").FontSize(8);

                            tabla.Cell().PaddingBottom(2).Column(c =>
                            {
                                c.Item().Text(item.Producto).FontSize(8);
                                c.Item().Text($"($ {item.PrecioUnitario:N2})").FontSize(7).FontColor(Colors.Grey.Medium);
                            });

                            tabla.Cell().AlignRight().PaddingBottom(2).Text($"$ {item.Subtotal:N2}").FontSize(8);
                        }
                    });

                    // --- PIE DE PÁGINA ---
                    page.Footer().PaddingTop(5).Column(col =>
                    {
                        col.Item().LineHorizontal(1).LineColor(Colors.Black);

                        col.Item().PaddingTop(2).AlignRight().Text($"TOTAL: $ {datos.Total:N2}").FontSize(16).Bold();
                        col.Item().Text($"CAE: {datos.Cae}").FontSize(8);
                        col.Item().Text($"Vto. CAE: {datos.VtoCae}").FontSize(8);

                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Forma de Pago:").FontSize(8);
                            row.RelativeItem().AlignRight().Text(datos.FormaPago.ToUpper()).FontSize(8).Bold();
                        });

                        if (datos.FormaPago == "Efectivo")
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Su Pago:");
                                row.RelativeItem().AlignRight().Text($"$ {datos.PagoCon:N2}");
                            });
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Su Vuelto:");
                                row.RelativeItem().AlignRight().Text($"$ {datos.Vuelto:N2}");
                            });
                        }

                        col.Item().PaddingTop(10).AlignCenter().Text("¡Gracias por su compra!").Italic();
                    });
                });
            });

            // ✅ CRÍTICO: Manejo de errores y nombre único con timestamp
            try
            {
                string ruta = Path.Combine(Path.GetTempPath(), $"Ticket_{datos.NroTicket}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                documento.GeneratePdf(ruta);

                if (File.Exists(ruta))
                {
                    Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
                }
                else
                {
                    throw new FileNotFoundException("El archivo PDF no se generó correctamente.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al generar o abrir el ticket: {ex.Message}", ex);
            }
        }
    }

    // Clase para la cabecera del ticket
    public class TicketData
    {
        public int NroTicket { get; set; }
        public decimal Total { get; set; }
        public decimal PagoCon { get; set; }
        public decimal Vuelto { get; set; }
        public string ClienteNombre { get; set; } = "Consumidor Final";
        public DateTime Fecha { get; set; }
        public string FormaPago { get; set; } = "Efectivo";
        public string Cae { get; set; } // <--- NUEVO
        public string VtoCae { get; set; } // <--- NUEVO
    }

    // ✅ CRÍTICO: Cantidad cambiada a DECIMAL
    public class TicketDetalle
    {
        public string Producto { get; set; } = string.Empty;
        public decimal Cantidad { get; set; } // ← CAMBIADO DE INT A DECIMAL
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}