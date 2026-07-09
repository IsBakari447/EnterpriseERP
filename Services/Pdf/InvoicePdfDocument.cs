using EnterpriseERP.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EnterpriseERP.Services.Pdf
{
    public class InvoicePdfDocument : IDocument
    {
        private readonly Invoice _invoice;

        public InvoicePdfDocument(Invoice invoice)
        {
            _invoice = invoice;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(Header);
                page.Content().Element(Content);
                page.Footer().Element(Footer);
            });
        }

        private void Header(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text("EnterpriseERP")
                            .FontSize(24)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        left.Item().Text("Business Management Suite")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);

                        left.Item().Text("Votre succès, notre priorité.")
                            .FontSize(10)
                            .Italic()
                            .FontColor(Colors.Blue.Darken2);
                    });

                    row.RelativeItem().AlignRight().Column(right =>
                    {
                        right.Item().Text("EnterpriseERP AB").Bold();
                        right.Item().Text("Stockholm, Suède");
                        right.Item().Text("Téléphone : +46 70 736 45 55");
                        right.Item().Text("Email : bakarii447@gmail.com");
                        right.Item().Text("Site web : www.enterpriseerp.com");
                    });
                });

                col.Item().PaddingTop(15).LineHorizontal(2).LineColor(Colors.Blue.Darken2);
            });
        }

        private void Content(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text("FACTURE").FontSize(28).Bold();
                        left.Item().Text($"N° {_invoice.InvoiceNumber}")
                            .FontSize(14)
                            .FontColor(Colors.Blue.Darken2);
                    });

                    row.RelativeItem().AlignRight().Column(right =>
                    {
                        right.Item().Text($"Date : {_invoice.InvoiceDate:dd/MM/yyyy}");
                        right.Item().Text($"Statut : {_invoice.Status}");
                        right.Item().Text($"Méthode : {_invoice.PaymentMethod}");
                    });
                });

                col.Item().PaddingTop(25).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(box =>
                {
                    box.Item().Text("Client").Bold().FontColor(Colors.Blue.Darken2);
                    box.Item().Text(_invoice.Client?.CompanyName ?? "-").Bold();
                    box.Item().Text(_invoice.Client?.FullName ?? "-");
                    box.Item().Text(_invoice.Client?.Email ?? "-");
                    box.Item().Text(_invoice.Client?.Phone ?? "-");
                });

                col.Item().PaddingTop(25).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);
                        columns.RelativeColumn(3);
                        columns.ConstantColumn(45);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header.Cell(), "#");
                        HeaderCell(header.Cell(), "Produit");
                        HeaderCell(header.Cell(), "Qté");
                        HeaderCell(header.Cell(), "PU");
                        HeaderCell(header.Cell(), "TVA");
                        HeaderCell(header.Cell(), "Total");
                    });

                    int index = 1;

                    foreach (var item in _invoice.Items)
                    {
                        BodyCell(table.Cell(), index.ToString());
                        BodyCell(table.Cell(), item.ProductName);
                        BodyCell(table.Cell(), item.Quantity.ToString("N2"));
                        BodyCell(table.Cell(), $"{item.UnitPrice:N2} €");
                        BodyCell(table.Cell(), $"{item.VatAmount:N2} €");
                        BodyCell(table.Cell(), $"{item.TotalTTC:N2} €");

                        index++;
                    }
                });

                col.Item().PaddingTop(20).AlignRight().Width(260).Column(summary =>
                {
                    SummaryRow(summary, "Sous-total HT", $"{_invoice.SubTotal:N2} €");
                    SummaryRow(summary, "TVA", $"{_invoice.VatAmount:N2} €");

                    summary.Item()
                        .Background(Colors.Blue.Darken2)
                        .Padding(10)
                        .Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL TTC").Bold().FontColor(Colors.White);
                            row.RelativeItem().AlignRight().Text($"{_invoice.TotalAmount:N2} €").Bold().FontColor(Colors.White);
                        });
                });

                col.Item().PaddingTop(25).Text(_invoice.ThankYouMessage)
                    .Italic()
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        private void Footer(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken2);

                col.Item().PaddingTop(8).AlignCenter()
                    .Text("EnterpriseERP AB — Stockholm, Suède — +46 70 736 45 55 — bakarii447@gmail.com")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);

                col.Item().AlignCenter()
                    .Text($"Document généré automatiquement le {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        private void HeaderCell(IContainer cell, string text)
        {
            cell.Background(Colors.Blue.Darken2)
                .Padding(6)
                .Text(text)
                .Bold()
                .FontColor(Colors.White);
        }

        private void BodyCell(IContainer cell, string text)
        {
            cell.BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(6)
                .Text(text);
        }

        private void SummaryRow(ColumnDescriptor summary, string label, string value)
        {
            summary.Item()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(8)
                .Row(row =>
                {
                    row.RelativeItem().Text(label);
                    row.RelativeItem().AlignRight().Text(value).Bold();
                });
        }
    }
}