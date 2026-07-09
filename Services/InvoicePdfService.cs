using EnterpriseERP.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EnterpriseERP.Services
{
    public static class InvoicePdfService
    {
        public static byte[] Generate(Invoice invoice)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text("EnterpriseERP")
                                    .FontSize(28)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken3);

                                left.Item().Text("Business Suite")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                            row.ConstantItem(220).Column(right =>
                            {
                                right.Item().AlignRight().Text("FACTURE")
                                    .FontSize(24)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken3);

                                right.Item().AlignRight().Text(invoice.InvoiceNumber);
                                right.Item().AlignRight().Text($"Date : {invoice.InvoiceDate:dd/MM/yyyy}");
                                right.Item().AlignRight().Text($"Statut : {invoice.Status}");
                            });
                        });

                        col.Item().PaddingTop(15).LineHorizontal(2).LineColor(Colors.Blue.Darken3);
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(client =>
                            {
                                client.Item().Text("Facturé à").Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                                client.Item().Text(invoice.Client?.FullName ?? "");
                                client.Item().Text(invoice.Client?.CompanyName ?? "");
                                client.Item().Text(invoice.Client?.Email ?? "");
                                client.Item().Text(invoice.Client?.Phone ?? "");
                                client.Item().Text(invoice.Client?.Address ?? "");
                            });

                            row.RelativeItem().AlignRight().Column(info =>
                            {
                                info.Item().Text("Informations paiement").Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                                info.Item().Text($"Méthode : {invoice.PaymentMethod}");
                                info.Item().Text(invoice.VatIncluded ? "TVA incluse : Oui" : "TVA incluse : Non");
                                info.Item().Text($"TVA : {invoice.VatRate}%");
                            });
                        });

                        col.Item().PaddingVertical(20);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(70);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(45);
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                HeaderCell(header.Cell(), "Date");
                                HeaderCell(header.Cell(), "Produit");
                                HeaderCell(header.Cell(), "Description");
                                HeaderCell(header.Cell(), "Qté");
                                HeaderCell(header.Cell(), "PU");
                                HeaderCell(header.Cell(), "TVA");
                                HeaderCell(header.Cell(), "Total");
                            });

                            foreach (var item in invoice.Items)
                            {
                                BodyCell(table.Cell(), item.Date.ToString("dd/MM/yy"));
                                BodyCell(table.Cell(), item.ProductName);
                                BodyCell(table.Cell(), item.Description);
                                BodyCell(table.Cell(), item.Quantity.ToString("0.##"));
                                BodyCell(table.Cell(), $"{item.UnitPrice:0.00} €");
                                BodyCell(table.Cell(), $"{item.VatRate:0.##}%");
                                BodyCell(table.Cell(), $"{item.TotalTTC:0.00} €");
                            }
                        });

                        col.Item().AlignRight().PaddingTop(20).Width(260).Column(total =>
                        {
                            total.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Total HT");
                                row.ConstantItem(100).AlignRight().Text($"{invoice.SubTotal:0.00} €");
                            });

                            total.Item().Row(row =>
                            {
                                row.RelativeItem().Text("TVA");
                                row.ConstantItem(100).AlignRight().Text($"{invoice.VatAmount:0.00} €");
                            });

                            total.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                            total.Item().Row(row =>
                            {
                                row.RelativeItem().Text("Total TTC").Bold().FontSize(14);
                                row.ConstantItem(120).AlignRight().Text($"{invoice.TotalAmount:0.00} €").Bold().FontSize(14);
                            });
                        });

                        col.Item().PaddingTop(30).Background(Colors.Blue.Lighten5).Padding(15).Column(msg =>
                        {
                            msg.Item().Text("Message").Bold().FontColor(Colors.Blue.Darken3);
                            msg.Item().Text(invoice.ThankYouMessage);
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Merci pour votre confiance - ");
                        text.Span("EnterpriseERP").Bold();
                    });
                });
            }).GeneratePdf();
        }

        private static void HeaderCell(IContainer container, string text)
        {
            container
                .Background(Colors.Blue.Darken3)
                .Padding(6)
                .Text(text)
                .FontColor(Colors.White)
                .Bold();
        }

        private static void BodyCell(IContainer container, string text)
        {
            container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6)
                .Text(text);
        }
    }
}