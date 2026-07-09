using EnterpriseERP.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EnterpriseERP.Services.Pdf
{
    public class QuotePdfDocument : IDocument
    {
        private readonly Quote _quote;

        public QuotePdfDocument(Quote quote)
        {
            _quote = quote;
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
                        left.Item().Text("DEVIS")
                            .FontSize(28)
                            .Bold();

                        left.Item().Text($"N° {_quote.QuoteNumber}")
                            .FontSize(14)
                            .FontColor(Colors.Blue.Darken2);
                    });

                    row.RelativeItem().AlignRight().Column(right =>
                    {
                        right.Item().Text($"Date : {_quote.QuoteDate:dd/MM/yyyy}");
                        right.Item().Text($"Valable jusqu’au : {_quote.ValidUntil:dd/MM/yyyy}");
                        right.Item().Text($"Statut : {_quote.Status}");
                        right.Item().Text($"Conditions : {_quote.PaymentTerms ?? "-"}");
                    });
                });

                col.Item().PaddingTop(25).Row(row =>
                {
                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(box =>
                    {
                        box.Item().Text("Client").Bold().FontColor(Colors.Blue.Darken2);
                        box.Item().Text(_quote.Client?.CompanyName ?? "-").Bold();
                        box.Item().Text(_quote.Client?.FullName ?? "-");
                        box.Item().Text(_quote.Client?.Email ?? "-");
                        box.Item().Text(_quote.Client?.Phone ?? "-");
                    });

                    row.ConstantItem(20);

                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(box =>
                    {
                        box.Item().Text("Informations").Bold().FontColor(Colors.Blue.Darken2);
                        box.Item().Text($"Préparé par : {_quote.CreatedBy ?? "EnterpriseERP"}");
                        box.Item().Text($"Date création : {_quote.CreatedAt:dd/MM/yyyy HH:mm}");
                        box.Item().Text($"Document généré : {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
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
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        HeaderCell(header.Cell(), "#");
                        HeaderCell(header.Cell(), "Produit");
                        HeaderCell(header.Cell(), "Qté");
                        HeaderCell(header.Cell(), "PU");
                        HeaderCell(header.Cell(), "Remise");
                        HeaderCell(header.Cell(), "TVA");
                        HeaderCell(header.Cell(), "Total");
                    });

                    int index = 1;

                    foreach (var item in _quote.Items)
                    {
                        BodyCell(table.Cell(), index.ToString());
                        BodyCell(table.Cell(), item.Product?.Name ?? "-");
                        BodyCell(table.Cell(), item.Quantity.ToString());
                        BodyCell(table.Cell(), $"{item.UnitPrice:N2} €");
                        BodyCell(table.Cell(), $"{item.DiscountAmount:N2} €");
                        BodyCell(table.Cell(), $"{item.TaxAmount:N2} €");
                        BodyCell(table.Cell(), $"{item.LineTotal:N2} €");

                        index++;
                    }
                });

                col.Item().PaddingTop(20).AlignRight().Width(260).Column(summary =>
                {
                    SummaryRow(summary, "Sous-total HT", $"{_quote.SubTotal:N2} €");
                    SummaryRow(summary, "Remise", $"{_quote.DiscountAmount:N2} €");
                    SummaryRow(summary, "TVA", $"{_quote.TaxAmount:N2} €");

                    summary.Item()
                        .Background(Colors.Blue.Darken2)
                        .Padding(10)
                        .Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL TTC").Bold().FontColor(Colors.White);
                            row.RelativeItem().AlignRight().Text($"{_quote.TotalAmount:N2} €").Bold().FontColor(Colors.White);
                        });
                });

                col.Item().PaddingTop(25).Text("Notes").Bold().FontColor(Colors.Blue.Darken2);
                col.Item().Text(string.IsNullOrWhiteSpace(_quote.Notes)
                    ? "Merci pour votre confiance. Ce devis reprend les produits et services mentionnés ci-dessus."
                    : _quote.Notes);

                col.Item().PaddingTop(35).Row(row =>
                {
                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(box =>
                    {
                        box.Item().Text("Signature du client").Bold();
                        box.Item().PaddingTop(60).LineHorizontal(1);
                        box.Item().Text("Date : ____ / ____ / ______");
                    });

                    row.ConstantItem(25);

                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(box =>
                    {
                        box.Item().Text("EnterpriseERP").Bold();
                        box.Item().PaddingTop(60).LineHorizontal(1);
                        box.Item().Text($"Date : {DateTime.Now:dd/MM/yyyy}");
                    });
                });
            });
        }

        private void Footer(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Colors.Blue.Darken2);

                col.Item().PaddingTop(8).AlignCenter().Text("Merci pour votre confiance.")
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                col.Item().AlignCenter().Text("EnterpriseERP AB — Stockholm, Suède — +46 70 736 45 55 — bakarii447@gmail.com")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);

                col.Item().AlignCenter().Text($"Document généré automatiquement le {DateTime.Now:dd/MM/yyyy HH:mm}")
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