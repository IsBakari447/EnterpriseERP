using EnterpriseERP.Data;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using EnterpriseERP.Services.Pdf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace EnterpriseERP.Controllers
{
    public class QuotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var quotes = await _context.Quotes
                .Include(q => q.Client)
                .OrderByDescending(q => q.QuoteDate)
                .ToListAsync();

            ViewBag.TotalQuotes = quotes.Count;
            ViewBag.TotalAmount = quotes.Sum(q => q.TotalAmount);

            return View(quotes);
        }

        public async Task<IActionResult> Create()
        {
            await LoadSelectLists();

            return View(new Quote
            {
                QuoteNumber = GenerateQuoteNumber(),
                QuoteDate = DateTime.Now,
                ValidUntil = DateTime.Now.AddDays(30),
                Status = "Brouillon",
                TaxRate = 0,
                DiscountRate = 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quote quote)
        {
            ModelState.Clear();

            var form = Request.Form;

            var productIds = form["productIds"].ToArray();
            var quantities = form["quantities"].ToArray();
            var unitPrices = form["unitPrices"].ToArray();
            var discountRates = form["discountRates"].ToArray();
            var taxRates = form["taxRates"].ToArray();

            if (quote.ClientId <= 0)
            {
                ModelState.AddModelError("", "Veuillez sélectionner un client.");
            }

            quote.Items = new List<QuoteItem>();

            decimal subTotal = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;

            for (int i = 0; i < productIds.Length; i++)
            {
                int productId = int.TryParse(productIds[i], out var p) ? p : 0;
                int quantity = quantities.Length > i && int.TryParse(quantities[i], out var q) ? q : 0;

                decimal unitPrice = ParseDecimal(unitPrices, i);
                decimal discountRate = ParseDecimal(discountRates, i);
                decimal taxRate = ParseDecimal(taxRates, i);

                if (productId <= 0 || quantity <= 0 || unitPrice <= 0)
                {
                    continue;
                }

                decimal lineBase = quantity * unitPrice;
                decimal lineDiscount = lineBase * discountRate / 100;
                decimal taxableAmount = lineBase - lineDiscount;
                decimal lineTax = taxableAmount * taxRate / 100;
                decimal lineTotal = taxableAmount + lineTax;

                quote.Items.Add(new QuoteItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountRate = discountRate,
                    DiscountAmount = lineDiscount,
                    TaxRate = taxRate,
                    TaxAmount = lineTax,
                    LineTotal = lineTotal
                });

                subTotal += lineBase;
                totalDiscount += lineDiscount;
                totalTax += lineTax;
            }

            if (!quote.Items.Any())
            {
                ModelState.AddModelError("", "Aucune ligne valide. Choisissez un produit, une quantité et un prix supérieur à 0.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectLists();

                quote.QuoteNumber = string.IsNullOrWhiteSpace(quote.QuoteNumber)
                    ? GenerateQuoteNumber()
                    : quote.QuoteNumber;

                TempData["Error"] = "Le devis n'a pas été enregistré. Vérifiez le client, le produit, la quantité et le prix.";

                return View(quote);
            }

            quote.QuoteNumber = string.IsNullOrWhiteSpace(quote.QuoteNumber)
                ? GenerateQuoteNumber()
                : quote.QuoteNumber;

            quote.Status = string.IsNullOrWhiteSpace(quote.Status)
                ? "Brouillon"
                : quote.Status;

            quote.SubTotal = subTotal;
            quote.DiscountAmount = totalDiscount;
            quote.TaxAmount = totalTax;
            quote.TotalAmount = subTotal - totalDiscount + totalTax;
            quote.CreatedAt = DateTime.Now;
            quote.CreatedBy = HttpContext.Session.GetString("UserEmail") ?? "System";

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Devis créé avec succès.";
            return RedirectToAction(nameof(Details), new { id = quote.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var quote = await _context.Quotes
                .Include(q => q.Client)
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
            {
                return NotFound();
            }

            return View(quote);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var quote = await _context.Quotes
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
            {
                return NotFound();
            }

            await LoadSelectLists();
            return View(quote);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Quote quote)
        {
            if (id != quote.Id)
            {
                return NotFound();
            }

            ModelState.Clear();

            var existingQuote = await _context.Quotes
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (existingQuote == null)
            {
                return NotFound();
            }

            var form = Request.Form;

            var productIds = form["productIds"].ToArray();
            var quantities = form["quantities"].ToArray();
            var unitPrices = form["unitPrices"].ToArray();
            var discountRates = form["discountRates"].ToArray();
            var taxRates = form["taxRates"].ToArray();

            if (quote.ClientId <= 0)
            {
                ModelState.AddModelError("", "Veuillez sélectionner un client.");
            }

            var newItems = new List<QuoteItem>();

            decimal subTotal = 0;
            decimal totalDiscount = 0;
            decimal totalTax = 0;

            for (int i = 0; i < productIds.Length; i++)
            {
                int productId = int.TryParse(productIds[i], out var p) ? p : 0;
                int quantity = quantities.Length > i && int.TryParse(quantities[i], out var q) ? q : 0;

                decimal unitPrice = ParseDecimal(unitPrices, i);
                decimal discountRate = ParseDecimal(discountRates, i);
                decimal taxRate = ParseDecimal(taxRates, i);

                if (productId <= 0 || quantity <= 0 || unitPrice <= 0)
                {
                    continue;
                }

                decimal lineBase = quantity * unitPrice;
                decimal lineDiscount = lineBase * discountRate / 100;
                decimal taxableAmount = lineBase - lineDiscount;
                decimal lineTax = taxableAmount * taxRate / 100;
                decimal lineTotal = taxableAmount + lineTax;

                newItems.Add(new QuoteItem
                {
                    QuoteId = existingQuote.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    DiscountRate = discountRate,
                    DiscountAmount = lineDiscount,
                    TaxRate = taxRate,
                    TaxAmount = lineTax,
                    LineTotal = lineTotal
                });

                subTotal += lineBase;
                totalDiscount += lineDiscount;
                totalTax += lineTax;
            }

            if (!newItems.Any())
            {
                ModelState.AddModelError("", "Aucune ligne valide. Choisissez un produit, une quantité et un prix supérieur à 0.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                quote.Items = newItems;
                TempData["Error"] = "Le devis n'a pas été modifié. Vérifiez les informations saisies.";
                return View(quote);
            }

            _context.QuoteItems.RemoveRange(existingQuote.Items);

            existingQuote.ClientId = quote.ClientId;
            existingQuote.QuoteDate = quote.QuoteDate;
            existingQuote.ValidUntil = quote.ValidUntil;
            existingQuote.Status = string.IsNullOrWhiteSpace(quote.Status) ? "Brouillon" : quote.Status;
            existingQuote.PaymentTerms = quote.PaymentTerms;
            existingQuote.Notes = quote.Notes;
            existingQuote.InternalNotes = quote.InternalNotes;
            existingQuote.UpdatedAt = DateTime.Now;

            existingQuote.SubTotal = subTotal;
            existingQuote.DiscountAmount = totalDiscount;
            existingQuote.TaxAmount = totalTax;
            existingQuote.TotalAmount = subTotal - totalDiscount + totalTax;

            _context.QuoteItems.AddRange(newItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Devis modifié avec succès.";
            return RedirectToAction(nameof(Details), new { id = existingQuote.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var quote = await _context.Quotes
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
            {
                return NotFound();
            }

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Devis supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }

       public async Task<IActionResult> Pdf(int id)
    {
    QuestPDF.Settings.License = LicenseType.Community;

    var quote = await _context.Quotes
        .Include(q => q.Client)
        .Include(q => q.Items)
            .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(q => q.Id == id);

    if (quote == null)
    {
        return NotFound();
    }

    var document = new QuotePdfDocument(quote);
    var pdfBytes = document.GeneratePdf();

    return File(
        pdfBytes,
        "application/pdf",
        $"{quote.QuoteNumber}.pdf"
    );
    }

        public async Task<IActionResult> SendEmail(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            TempData["Success"] = "Fonction d’envoi par email bientôt disponible.";
            return RedirectToAction(nameof(Details), new { id });
        }

       public async Task<IActionResult> ConvertToInvoice(int id)
{
    var quote = await _context.Quotes
        .Include(q => q.Client)
        .Include(q => q.Items)
            .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(q => q.Id == id);

    if (quote == null)
    {
        return NotFound();
    }

    // Empêcher une double conversion
    bool invoiceExists = await _context.Invoices
        .AnyAsync(i => i.InvoiceNumber == quote.QuoteNumber.Replace("DEV", "FAC"));

    if (invoiceExists)
    {
        TempData["Error"] = "Cette facture existe déjà.";
        return RedirectToAction(nameof(Details), new { id });
    }

    var invoice = new Invoice
    {
        ClientId = quote.ClientId,
        InvoiceDate = DateTime.Now,
        InvoiceNumber = quote.QuoteNumber.Replace("DEV", "FAC"),
        Status = "Unpaid",

        VatIncluded = true,
        VatRate = quote.TaxRate,

        SubTotal = quote.SubTotal,
        VatAmount = quote.TaxAmount,
        TotalAmount = quote.TotalAmount,

        PaymentMethod = "Cash",

        ThankYouMessage =
            "Merci beaucoup pour votre confiance. Nous serons ravis de vous accueillir à nouveau.",

        Items = new List<InvoiceItem>()
    };

    foreach (var item in quote.Items)
    {
        invoice.Items.Add(new InvoiceItem
        {
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? "",

            Description = item.Product?.Name ?? "",
            Quantity = item.Quantity,

            UnitPrice = item.UnitPrice,

            Discount = item.DiscountRate,

            VatRate = item.TaxRate,

            VatIncluded = true,

            TotalHT =
                (item.Quantity * item.UnitPrice) - item.DiscountAmount,

            VatAmount = item.TaxAmount,

            TotalTTC = item.LineTotal
        });
    }

    _context.Invoices.Add(invoice);

    quote.Status = "Facturé";

    await _context.SaveChangesAsync();

    TempData["Success"] = "Le devis a été converti en facture.";

    return RedirectToAction(
        "Details",
        "Invoices",
        new { id = invoice.Id });
}

        public async Task<IActionResult> ConvertToOrder(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
            {
                return NotFound();
            }

            TempData["Success"] = "Conversion en commande bientôt disponible.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task LoadSelectLists()
        {
            ViewBag.Clients = new SelectList(
                await _context.Clients.OrderBy(c => c.CompanyName).ToListAsync(),
                "Id",
                "CompanyName"
            );

            ViewBag.Products = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        private decimal ParseDecimal(string?[] values, int index)
        {
            if (values == null || values.Length <= index)
            {
                return 0;
            }

            var value = values[index];

            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            value = value.Replace(",", ".");

            return decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var result)
                ? result
                : 0;
        }

        private string GenerateQuoteNumber()
        {
            return $"DEV-{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}
