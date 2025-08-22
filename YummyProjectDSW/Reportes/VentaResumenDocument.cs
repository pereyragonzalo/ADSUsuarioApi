using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using yummyApp.Dtos;

public sealed class VentaResumenDocument : IDocument
{
    public string Titulo { get; init; } = "Reporte de Ventas - Resumen";
    public string Periodo { get; init; } = string.Empty;  // ej. "01/08/2025 - 16/08/2025"
    public IReadOnlyList<VentaResumenDto> Filas { get; init; } = Array.Empty<VentaResumenDto>();

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        var culture = new CultureInfo("es-PE");

        container.Page(page =>
        {
            page.Margin(36);

            // Header
            page.Header().Column(col =>
            {
                col.Item().Row(r =>
                {
                    r.RelativeItem().Text(Titulo).SemiBold().FontSize(16);
                    if (!string.IsNullOrWhiteSpace(Periodo))
                        r.ConstantItem(240).AlignRight().Text(Periodo);
                });
                col.Item().Height(6);
                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });

            // Content
            page.Content().Element(c =>
            {
                var totalImporte = Filas.Sum(x => x.Total);
                var totalLineas = Filas.Sum(x => x.Lineas);
                var totalUnidades = Filas.Sum(x => x.Cantidad);

                c.PaddingTop(10).Table(t =>
                {
                    t.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(55);  // Factura
                        cols.ConstantColumn(70);  // Fecha
                        cols.RelativeColumn(3);   // Cliente
                        cols.RelativeColumn(3);   // Vendedor
                        cols.ConstantColumn(55);  // Líneas
                        cols.ConstantColumn(60);  // Cant.
                        cols.ConstantColumn(90);  // Total
                    });

                    // Encabezado
                    t.Header(h =>
                    {
                        h.Cell().Element(HeaderCell).Text("Factura");
                        h.Cell().Element(HeaderCell).Text("Fecha");
                        h.Cell().Element(HeaderCell).Text("Cliente");
                        h.Cell().Element(HeaderCell).Text("Vendedor");
                        h.Cell().Element(HeaderCell).Text("Líneas");
                        h.Cell().Element(HeaderCell).Text("Cant.");
                        h.Cell().Element(HeaderCell).Text("Total");
                    });

                    // Filas (zebra ligera)
                    var i = 0;
                    foreach (var f in Filas)
                    {
                        var shade = (i++ % 2 == 1);
                        t.Cell().Element(c => BodyCell(c, shade)).Text(f.NFactura.ToString());
                        t.Cell().Element(c => BodyCell(c, shade)).Text(f.Fecha.ToString("dd/MM/yyyy"));
                        t.Cell().Element(c => BodyCell(c, shade)).Text(f.Cliente);
                        t.Cell().Element(c => BodyCell(c, shade)).Text(f.Vendedor);
                        t.Cell().Element(c => BodyCell(c, shade)).AlignRight().Text(f.Lineas.ToString());
                        t.Cell().Element(c => BodyCell(c, shade)).AlignRight().Text(f.Cantidad.ToString());
                        t.Cell().Element(c => BodyCell(c, shade)).AlignRight().Text(f.Total.ToString("C", culture));
                    }

                    // Footer con totales
                    t.Footer(foot =>
                    {
                        foot.Cell().ColumnSpan(5).Element(FooterCell).AlignRight().Text("TOTALES →").SemiBold();
                        foot.Cell().Element(FooterCell).AlignRight().Text(totalUnidades.ToString());
                        foot.Cell().Element(FooterCell).AlignRight().Text(totalImporte.ToString("C", culture)).SemiBold();
                    });

                    static IContainer HeaderCell(IContainer c) =>
                        c.DefaultTextStyle(s => s.SemiBold())
                         .Background(Colors.Grey.Lighten3)
                         .PaddingVertical(6).PaddingHorizontal(6)
                         .BorderBottom(1).BorderColor(Colors.Grey.Medium);

                    static IContainer BodyCell(IContainer c, bool shade) =>
                        c.Background(shade ? Colors.Grey.Lighten5 : Colors.White)
                         .PaddingVertical(4).PaddingHorizontal(6)
                         .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);

                    static IContainer FooterCell(IContainer c) =>
                        c.PaddingVertical(6).PaddingHorizontal(6)
                         .BorderTop(1).BorderColor(Colors.Grey.Medium);
                });
            });

            page.Footer().AlignCenter().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
        });
    }
}



