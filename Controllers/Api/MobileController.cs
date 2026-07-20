using QuestPDF.Fluent;
using Microsoft.AspNetCore.Authorization;
using EnterpriseERP.Data;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers.Api
{
    [ApiController]
    [Route("api/mobile")]
[Authorize]
    public class MobileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TrialPolicyService _trialPolicy;

        public MobileController(ApplicationDbContext context, TrialPolicyService trialPolicy)
        {
            _context = context;
            _trialPolicy = trialPolicy;
        }

        [HttpGet("trial/status")]
        public async Task<IActionResult> GetTrialStatus()
        {
            return Ok(await _trialPolicy.GetStatusAsync(HttpContext.RequestAborted));
        }

        [HttpGet("clients")]
        public async Task<IActionResult> GetClients() =>
            Ok(await _context.Clients.OrderBy(c => c.FullName).ToListAsync());

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts() =>
            Ok(await _context.Products.OrderBy(p => p.Name).ToListAsync());

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var data = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    ClientName = o.Client != null ? o.Client.FullName : "",
                    ProductName = o.Product != null ? o.Product.Name : "",
                    o.Quantity,
                    o.Status
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("quotes")]
        public async Task<IActionResult> GetQuotes()
        {
            var data = await _context.Quotes
                .Include(q => q.Client)
                .OrderByDescending(q => q.QuoteDate)
                .Select(q => new
                {
                    q.Id,
                    q.QuoteNumber,
                    ClientName = q.Client != null ? q.Client.CompanyName : "",
                    q.QuoteDate,
                    q.TotalAmount,
                    q.Status
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            var data = await _context.Invoices
                .Include(i => i.Client)
                .OrderByDescending(i => i.InvoiceDate)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    ClientName = i.Client != null ? i.Client.FullName : "",
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.Status
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliers() =>
            Ok(await _context.Suppliers.OrderBy(s => s.Name).ToListAsync());

        [HttpGet("payments")]
        public async Task<IActionResult> GetPayments()
        {
            var data = await _context.Payments
                .Include(p => p.Invoice)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new
                {
                    p.Id,
                    p.InvoiceId,
                    InvoiceNumber = p.Invoice != null ? p.Invoice.InvoiceNumber : "",
                    p.Amount,
                    p.Method,
                    p.Reference,
                    p.PaymentDate
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("expenses")]
        public async Task<IActionResult> GetExpenses() =>
            Ok(await _context.Expenses.OrderByDescending(e => e.ExpenseDate).ToListAsync());

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees() =>
            Ok(await _context.Employees.OrderBy(e => e.FullName).ToListAsync());

        [HttpGet("presences")]
        public async Task<IActionResult> GetPresences()
        {
            var today = DateTime.Today;

            var data = await _context.Presences
                .Include(p => p.Employee)
                .OrderByDescending(p => p.Date)
                .ThenByDescending(p => p.CheckIn)
                .Select(p => new
                {
                    p.Id,
                    p.EmployeeId,
                    EmployeeName = p.Employee != null ? p.Employee.FullName : "",
                    p.Date,
                    CheckIn = p.CheckIn != null ? p.CheckIn.ToString() : "",
                    CheckOut = p.CheckOut != null ? p.CheckOut.ToString() : "",
                    IsToday = p.Date.Date == today,
                    IsPresent = p.Date.Date == today && p.CheckIn != null && p.CheckOut == null,
                    Status = p.CheckIn == null ? "Absent" : p.CheckOut == null ? "Présent" : "Sorti"
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("presences/today")]
        public async Task<IActionResult> GetTodayPresences()
        {
            var today = DateTime.Today;

            var data = await _context.Presences
                .Include(p => p.Employee)
                .Where(p => p.Date.Date == today)
                .OrderByDescending(p => p.CheckIn)
                .Select(p => new
                {
                    p.Id,
                    p.EmployeeId,
                    EmployeeName = p.Employee != null ? p.Employee.FullName : "",
                    p.Date,
                    CheckIn = p.CheckIn != null ? p.CheckIn.ToString() : "",
                    CheckOut = p.CheckOut != null ? p.CheckOut.ToString() : "",
                    IsPresent = p.CheckIn != null && p.CheckOut == null,
                    Status = p.CheckIn == null ? "Absent" : p.CheckOut == null ? "Présent" : "Sorti"
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("stock")]
        public async Task<IActionResult> GetStock()
        {
            var data = await _context.StockMovements
                .Include(s => s.Product)
                .OrderByDescending(s => s.Date)
                .Select(s => new
                {
                    s.Id,
                    ProductName = s.Product != null ? s.Product.Name : "",
                    s.Type,
                    s.Quantity,
                    s.Date
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("audit")]
        public async Task<IActionResult> GetAudit() =>
            Ok(await _context.AuditLogs.OrderByDescending(a => a.Date).Take(100).ToListAsync());

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings() =>
            Ok(await _context.AppSettings.FirstOrDefaultAsync());

        [HttpGet("company-profile")]
        public async Task<IActionResult> GetCompanyProfile() =>
            Ok(await _context.CompanyProfiles.FirstOrDefaultAsync());

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var data = await _context.Users.ToListAsync();
            return Ok(data);
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var data = await _context.Permissions.ToListAsync();
            return Ok(data);
        }

        [HttpGet("role-permissions")]
        public async Task<IActionResult> GetRolePermissions()
        {
            var data = await _context.RolePermissions
                .Include(r => r.Permission)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("reports-summary")]
        public async Task<IActionResult> GetReportsSummary()
        {
            var invoicesTotal = await SumDecimalAsync(_context.Invoices.Select(i => i.TotalAmount));
            var expensesTotal = await SumDecimalAsync(_context.Expenses.Select(e => e.Amount));
            var paymentsTotal = await SumDecimalAsync(_context.Payments.Select(p => p.Amount));

            return Ok(new
            {
                invoicesTotal,
                expensesTotal,
                paymentsTotal,
                profit = paymentsTotal - expensesTotal,
                clientsCount = await _context.Clients.CountAsync(),
                productsCount = await _context.Products.CountAsync(),
                invoicesCount = await _context.Invoices.CountAsync(),
                expensesCount = await _context.Expenses.CountAsync()
            });
        }

        private static async Task<decimal> SumDecimalAsync(IQueryable<decimal> query)
        {
            var values = await query.ToListAsync();
            return values.Sum();
        }

        [HttpGet("backup-summary")]
        public async Task<IActionResult> GetBackupSummary()
        {
            var settings = await _context.AppSettings.FirstOrDefaultAsync();

            return Ok(new
            {
                enableAutoBackup = settings != null && settings.EnableAutoBackup,
                backupFrequency = settings != null ? settings.BackupFrequency : "Daily",
                backupRetentionDays = settings != null ? settings.BackupRetentionDays : 30,
                backupPath = settings != null ? settings.BackupPath : "",
                lastStatus = "Disponible"
            });
        }

        [HttpGet("security-summary")]
        public async Task<IActionResult> GetSecuritySummary()
        {
            var settings = await _context.AppSettings.FirstOrDefaultAsync();

            return Ok(new
            {
                sessionTimeoutMinutes = settings != null ? settings.SessionTimeoutMinutes : 60,
                enableTwoFactorAuth = settings != null && settings.EnableTwoFactorAuth,
                maxLoginAttempts = settings != null ? settings.MaxLoginAttempts : 5,
                autoLockAccounts = settings == null || settings.AutoLockAccounts,
                auditEvents = await _context.AuditLogs.CountAsync()
            });
        }

        // =========================
        // CLIENTS CRUD
        // =========================

        [HttpPost("clients")]
        public async Task<IActionResult> CreateClient([FromBody] EnterpriseERP.Models.Client client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            client.CreatedAt = DateTime.UtcNow;

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok(client);
        }

        [HttpPut("clients/{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] EnterpriseERP.Models.Client client)
        {
            var existing = await _context.Clients.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.FullName = client.FullName;
            existing.CompanyName = client.CompanyName;
            existing.Email = client.Email;
            existing.Phone = client.Phone;
            existing.Address = client.Address;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("clients/{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Client supprimé."
            });
        }


        // =========================
        // PRODUCTS CRUD
        // =========================

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] EnterpriseERP.Models.Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productLimit = await _trialPolicy.CanCreateProductAsync(HttpContext.RequestAborted);
            if (!productLimit.Allowed)
                return StatusCode(StatusCodes.Status402PaymentRequired, new { success = false, message = productLimit.Message });

            product.CreatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] EnterpriseERP.Models.Product product)
        {
            var existing = await _context.Products.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.Name = product.Name;
            existing.Category = product.Category;
            existing.PurchasePrice = product.PurchasePrice;
            existing.SalePrice = product.SalePrice;
            existing.Quantity = product.Quantity;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Produit supprimé."
            });
        }


        // =========================
        // EMPLOYEES CRUD
        // =========================

        [HttpPost("employees")]
        public async Task<IActionResult> CreateEmployee([FromBody] EnterpriseERP.Models.Employee employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            employee.CreatedAt = DateTime.UtcNow;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpPut("employees/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EnterpriseERP.Models.Employee employee)
        {
            var existing = await _context.Employees.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.FullName = employee.FullName;
            existing.Position = employee.Position;
            existing.Email = employee.Email;
            existing.Phone = employee.Phone;
            existing.Salary = employee.Salary;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("employees/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Employé supprimé."
            });
        }


        // =========================
        // SUPPLIERS CRUD
        // =========================

        [HttpPost("suppliers")]
        public async Task<IActionResult> CreateSupplier([FromBody] EnterpriseERP.Models.Supplier supplier)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            supplier.CreatedAt = DateTime.UtcNow;

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return Ok(supplier);
        }

        [HttpPut("suppliers/{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromBody] EnterpriseERP.Models.Supplier supplier)
        {
            var existing = await _context.Suppliers.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.Name = supplier.Name;
            existing.ContactPerson = supplier.ContactPerson;
            existing.Email = supplier.Email;
            existing.Phone = supplier.Phone;
            existing.Address = supplier.Address;
            existing.Category = supplier.Category;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("suppliers/{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
                return NotFound();

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Fournisseur supprimé."
            });
        }


        // =========================
        // EXPENSES CRUD
        // =========================

        [HttpPost("expenses")]
        public async Task<IActionResult> CreateExpense([FromBody] EnterpriseERP.Models.Expense expense)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            expense.CreatedAt = DateTime.UtcNow;

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok(expense);
        }

        [HttpPut("expenses/{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] EnterpriseERP.Models.Expense expense)
        {
            var existing = await _context.Expenses.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.Title = expense.Title;
            existing.Category = expense.Category;
            existing.Amount = expense.Amount;
            existing.ExpenseDate = expense.ExpenseDate;
            existing.Description = expense.Description;
            existing.PaymentMethod = expense.PaymentMethod;
            existing.CreatedBy = expense.CreatedBy;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("expenses/{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
                return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Dépense supprimée."
            });
        }


        // =========================
        // ORDERS CRUD
        // =========================

        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] EnterpriseERP.Models.Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (order.TotalAmount <= 0)
                order.TotalAmount = order.UnitPrice * order.Quantity;

            order.OrderDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        [HttpPut("orders/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] EnterpriseERP.Models.Order order)
        {
            var existing = await _context.Orders.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.ClientId = order.ClientId;
            existing.ProductId = order.ProductId;
            existing.Quantity = order.Quantity;
            existing.UnitPrice = order.UnitPrice;
            existing.TotalAmount = order.TotalAmount <= 0 ? order.UnitPrice * order.Quantity : order.TotalAmount;
            existing.Status = order.Status;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("orders/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Commande supprimée."
            });
        }


        // =========================
        // INVOICES CRUD
        // =========================

        [HttpPost("invoices")]
        public async Task<IActionResult> CreateInvoice([FromBody] EnterpriseERP.Models.Invoice invoice)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invoiceLimit = await _trialPolicy.CanCreateInvoiceAsync(HttpContext.RequestAborted);
            if (!invoiceLimit.Allowed)
                return StatusCode(StatusCodes.Status402PaymentRequired, new { success = false, message = invoiceLimit.Message });

            invoice.InvoiceDate = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                invoice.InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}";

            if (invoice.TotalAmount <= 0)
            {
                invoice.VatAmount = invoice.SubTotal * invoice.VatRate / 100;
                invoice.TotalAmount = invoice.SubTotal + invoice.VatAmount;
            }

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        [HttpPut("invoices/{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] EnterpriseERP.Models.Invoice invoice)
        {
            var existing = await _context.Invoices.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.ClientId = invoice.ClientId;
            existing.InvoiceNumber = invoice.InvoiceNumber;
            existing.Status = invoice.Status;
            existing.VatIncluded = invoice.VatIncluded;
            existing.VatRate = invoice.VatRate;
            existing.SubTotal = invoice.SubTotal;
            existing.VatAmount = invoice.VatAmount;
            existing.TotalAmount = invoice.TotalAmount;
            existing.PaymentMethod = invoice.PaymentMethod;
            existing.ThankYouMessage = invoice.ThankYouMessage;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("invoices/{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Facture supprimée."
            });
        }


        // =========================
        // PAYMENTS CRUD
        // =========================

        [HttpPost("payments")]
        public async Task<IActionResult> CreatePayment([FromBody] EnterpriseERP.Models.Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            payment.PaymentDate = DateTime.UtcNow;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        [HttpPut("payments/{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] EnterpriseERP.Models.Payment payment)
        {
            var existing = await _context.Payments.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.InvoiceId = payment.InvoiceId;
            existing.Amount = payment.Amount;
            existing.Method = payment.Method;
            existing.Reference = payment.Reference;
            existing.PaymentDate = payment.PaymentDate;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("payments/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null)
                return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Paiement supprimé."
            });
        }


        // =========================
        // QUOTES CRUD
        // =========================

        [HttpPost("quotes")]
        public async Task<IActionResult> CreateQuote([FromBody] EnterpriseERP.Models.Quote quote)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            quote.CreatedAt = DateTime.UtcNow;
            quote.QuoteDate = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(quote.QuoteNumber))
                quote.QuoteNumber = $"DEV-{DateTime.UtcNow:yyyyMMddHHmmss}";

            if (quote.TotalAmount <= 0)
            {
                quote.DiscountAmount = quote.SubTotal * quote.DiscountRate / 100;
                quote.TaxAmount = (quote.SubTotal - quote.DiscountAmount) * quote.TaxRate / 100;
                quote.TotalAmount = quote.SubTotal - quote.DiscountAmount + quote.TaxAmount;
            }

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return Ok(quote);
        }

        [HttpPut("quotes/{id}")]
        public async Task<IActionResult> UpdateQuote(int id, [FromBody] EnterpriseERP.Models.Quote quote)
        {
            var existing = await _context.Quotes.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.QuoteNumber = quote.QuoteNumber;
            existing.ClientId = quote.ClientId;
            existing.ValidUntil = quote.ValidUntil;
            existing.Status = quote.Status;
            existing.SubTotal = quote.SubTotal;
            existing.DiscountRate = quote.DiscountRate;
            existing.DiscountAmount = quote.DiscountAmount;
            existing.TaxRate = quote.TaxRate;
            existing.TaxAmount = quote.TaxAmount;
            existing.TotalAmount = quote.TotalAmount;
            existing.PaymentTerms = quote.PaymentTerms;
            existing.Notes = quote.Notes;
            existing.InternalNotes = quote.InternalNotes;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("quotes/{id}")]
        public async Task<IActionResult> DeleteQuote(int id)
        {
            var quote = await _context.Quotes.FindAsync(id);

            if (quote == null)
                return NotFound();

            _context.Quotes.Remove(quote);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Devis supprimé."
            });
        }


        // =========================
        // STOCK CRUD
        // =========================

        [HttpPost("stock")]
        public async Task<IActionResult> CreateStockMovement([FromBody] EnterpriseERP.Models.StockMovement stock)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            stock.Date = DateTime.UtcNow;

            _context.StockMovements.Add(stock);

            var product = await _context.Products.FindAsync(stock.ProductId);
            if (product != null)
            {
                if (stock.Type.Equals("IN", StringComparison.OrdinalIgnoreCase))
                    product.Quantity += stock.Quantity;
                else if (stock.Type.Equals("OUT", StringComparison.OrdinalIgnoreCase))
                    product.Quantity -= stock.Quantity;
            }

            await _context.SaveChangesAsync();

            return Ok(stock);
        }

        [HttpPut("stock/{id}")]
        public async Task<IActionResult> UpdateStockMovement(int id, [FromBody] EnterpriseERP.Models.StockMovement stock)
        {
            var existing = await _context.StockMovements.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.ProductId = stock.ProductId;
            existing.Type = stock.Type;
            existing.Quantity = stock.Quantity;
            existing.Date = stock.Date;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("stock/{id}")]
        public async Task<IActionResult> DeleteStockMovement(int id)
        {
            var stock = await _context.StockMovements.FindAsync(id);

            if (stock == null)
                return NotFound();

            _context.StockMovements.Remove(stock);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Mouvement de stock supprimé."
            });
        }


        // =========================
        // PRESENCES CRUD
        // =========================

        [HttpPost("presences")]
        public async Task<IActionResult> CreatePresence([FromBody] EnterpriseERP.Models.Presence presence)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (presence.Date == default)
                presence.Date = DateTime.Today;

            _context.Presences.Add(presence);
            await _context.SaveChangesAsync();

            return Ok(presence);
        }

        [HttpPut("presences/{id}")]
        public async Task<IActionResult> UpdatePresence(int id, [FromBody] EnterpriseERP.Models.Presence presence)
        {
            var existing = await _context.Presences.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.EmployeeId = presence.EmployeeId;
            existing.Date = presence.Date;
            existing.CheckIn = presence.CheckIn;
            existing.CheckOut = presence.CheckOut;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("presences/{id}")]
        public async Task<IActionResult> DeletePresence(int id)
        {
            var presence = await _context.Presences.FindAsync(id);

            if (presence == null)
                return NotFound();

            _context.Presences.Remove(presence);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Présence supprimée."
            });
        }


        // =========================
        // USERS CRUD
        // =========================

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] EnterpriseERP.Models.User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userLimit = await _trialPolicy.CanCreateUserAsync(HttpContext.RequestAborted);
            if (!userLimit.Allowed)
                return StatusCode(StatusCodes.Status402PaymentRequired, new { success = false, message = userLimit.Message });

            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(user.Role))
                user.Role = "Employee";

            if (string.IsNullOrWhiteSpace(user.PasswordHash) && !string.IsNullOrWhiteSpace(user.Password))
                user.PasswordHash = Helpers.PasswordHelper.HashPassword(user.Password);
            else if (!string.IsNullOrWhiteSpace(user.PasswordHash) && user.PasswordHash == user.Password)
                user.PasswordHash = Helpers.PasswordHelper.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] EnterpriseERP.Models.User user)
        {
            var existing = await _context.Users.FindAsync(id);

            if (existing == null)
                return NotFound();

            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.Role = user.Role;
            existing.IsSuperAdmin = user.IsSuperAdmin;
            existing.IsActive = user.IsActive;
            existing.IsApproved = user.IsApproved;
            existing.Phone = user.Phone;
            existing.Address = user.Address;
            existing.Department = user.Department;
            existing.Position = user.Position;
            existing.PreferredLanguage = user.PreferredLanguage;
            existing.Theme = user.Theme;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Utilisateur supprimé."
            });
        }


        // =========================
        // SETTINGS UPDATE
        // =========================

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] EnterpriseERP.Models.AppSetting setting)
        {
            var existing = await _context.AppSettings.FirstOrDefaultAsync();

            if (existing == null)
            {
                setting.CreatedAt = DateTime.UtcNow;
                setting.UpdatedAt = DateTime.UtcNow;

                _context.AppSettings.Add(setting);
                await _context.SaveChangesAsync();

                return Ok(setting);
            }

            existing.CompanyName = setting.CompanyName;
            existing.CompanyPhone = setting.CompanyPhone;
            existing.CompanyEmail = setting.CompanyEmail;
            existing.CompanyWebsite = setting.CompanyWebsite;
            existing.DefaultCurrency = setting.DefaultCurrency;
            existing.DefaultLanguage = setting.DefaultLanguage;
            existing.Theme = setting.Theme;
            existing.PrimaryColor = setting.PrimaryColor;
            existing.SessionTimeoutMinutes = setting.SessionTimeoutMinutes;
            existing.EnableTwoFactorAuth = setting.EnableTwoFactorAuth;
            existing.MaxLoginAttempts = setting.MaxLoginAttempts;
            existing.AutoLockAccounts = setting.AutoLockAccounts;
            existing.EnableAutoBackup = setting.EnableAutoBackup;
            existing.BackupFrequency = setting.BackupFrequency;
            existing.BackupRetentionDays = setting.BackupRetentionDays;
            existing.BackupPath = setting.BackupPath;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpPut("company-profile")]
        public async Task<IActionResult> UpdateCompanyProfile([FromBody] EnterpriseERP.Models.CompanyProfile profile)
        {
            var existing = await _context.CompanyProfiles.FirstOrDefaultAsync();

            if (existing == null)
            {
                _context.CompanyProfiles.Add(profile);
                await _context.SaveChangesAsync();

                return Ok(profile);
            }

            existing.CompanyName = profile.CompanyName;
            existing.Slogan = profile.Slogan;
            existing.Address = profile.Address;
            existing.Phone = profile.Phone;
            existing.Email = profile.Email;
            existing.Website = profile.Website;
            existing.LegalInfo = profile.LegalInfo;
            existing.FooterMessage = profile.FooterMessage;
            existing.LogoPath = profile.LogoPath;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }


        // =========================
        // BACKUP CENTER
        // =========================

        [HttpPost("backup/create")]
        public async Task<IActionResult> CreateBackup()
        {
            var settings = await _context.AppSettings.FirstOrDefaultAsync();

            var folder = settings?.BackupPath;

            if (string.IsNullOrWhiteSpace(folder))
                folder = Path.Combine(AppContext.BaseDirectory, "Backups");

            Directory.CreateDirectory(folder);

            var fileName = $"enterpriseerp_backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.db";
            var backupPath = Path.Combine(folder, fileName);

            var sourceDb = Path.Combine(Directory.GetCurrentDirectory(), "enterpriseerp.db");

            if (!System.IO.File.Exists(sourceDb))
            {
                return NotFound(new
                {
                    success = false,
                    message = "Base de données introuvable.",
                    path = sourceDb
                });
            }

            System.IO.File.Copy(sourceDb, backupPath, true);

            return Ok(new
            {
                success = true,
                message = "Sauvegarde créée avec succès.",
                fileName,
                backupPath,
                createdAt = DateTime.UtcNow
            });
        }

        [HttpPut("backup/settings")]
        public async Task<IActionResult> UpdateBackupSettings([FromBody] EnterpriseERP.Models.AppSetting setting)
        {
            var existing = await _context.AppSettings.FirstOrDefaultAsync();

            if (existing == null)
            {
                setting.CreatedAt = DateTime.UtcNow;
                setting.UpdatedAt = DateTime.UtcNow;
                _context.AppSettings.Add(setting);
                await _context.SaveChangesAsync();
                return Ok(setting);
            }

            existing.EnableAutoBackup = setting.EnableAutoBackup;
            existing.BackupFrequency = setting.BackupFrequency;
            existing.BackupRetentionDays = setting.BackupRetentionDays;
            existing.BackupPath = setting.BackupPath;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }


        // =========================
        // AUDIT DELETE
        // =========================

        [HttpDelete("audit/{id}")]
        public async Task<IActionResult> DeleteAuditLog(int id)
        {
            var log = await _context.AuditLogs.FindAsync(id);

            if (log == null)
                return NotFound();

            _context.AuditLogs.Remove(log);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "Événement d'audit supprimé."
            });

        }
        // =========================
        // USER PROFILE
        // =========================

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto profile)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            user.FullName = string.IsNullOrWhiteSpace(profile.FullName) ? user.FullName : profile.FullName;
            user.Phone = profile.Phone;
            user.Address = profile.Address;
            user.Department = profile.Department;
            user.Position = profile.Position;
            user.PreferredLanguage = string.IsNullOrWhiteSpace(profile.PreferredLanguage) ? "fr" : profile.PreferredLanguage;
            user.Theme = string.IsNullOrWhiteSpace(profile.Theme) ? "Dark" : profile.Theme;
            user.PhotoPath = profile.PhotoPath;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        public class ProfileUpdateDto
        {
            public string? FullName { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? Department { get; set; }
            public string? Position { get; set; }
            public string? PreferredLanguage { get; set; }
            public string? Theme { get; set; }
            public string? PhotoPath { get; set; }
        }


        [HttpPost("profile/photo")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("Aucune photo reçue.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
            Directory.CreateDirectory(uploadsFolder);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"profile_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            user.PhotoPath = $"/uploads/profiles/{fileName}";
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                photoPath = user.PhotoPath
            });
        }


        // =========================
        // MOBILE PDF EXPORTS
        // =========================

        [HttpGet("invoices/{id}/pdf")]
        public async Task<IActionResult> DownloadInvoicePdfMobile(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var invoice = await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            var document = new EnterpriseERP.Services.Pdf.InvoicePdfDocument(invoice);
            var pdf = document.GeneratePdf();

            return File(pdf, "application/pdf", $"facture_{invoice.InvoiceNumber}.pdf");
        }

        [HttpGet("quotes/{id}/pdf")]
        public async Task<IActionResult> DownloadQuotePdfMobile(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var quote = await _context.Quotes
                .Include(q => q.Client)
                .Include(q => q.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return NotFound();

            var document = new EnterpriseERP.Services.Pdf.QuotePdfDocument(quote);
            var pdf = document.GeneratePdf();

            return File(pdf, "application/pdf", $"devis_{quote.QuoteNumber}.pdf");
        }


        // =========================
        // AI ASSISTANT ERP
        // =========================

        [HttpPost("ai/ask")]
        public async Task<IActionResult> AskAi([FromBody] System.Text.Json.JsonElement request)
        {
            var question = request.TryGetProperty("question", out var q1)
                ? q1.GetString() ?? ""
                : request.TryGetProperty("Question", out var q2)
                    ? q2.GetString() ?? ""
                    : "";

            var engine = HttpContext.RequestServices.GetRequiredService<EnterpriseERP.Services.AI.EnterpriseAiEngine>();
            var answer = await engine.AskAsync(question);

            return Ok(new
            {
                answer,
                generatedAt = DateTime.UtcNow
            });
        }


        [HttpGet("ai/dashboard")]
        public async Task<IActionResult> GetAiDashboard()
        {
            var engine = HttpContext.RequestServices.GetRequiredService<EnterpriseERP.Services.AI.EnterpriseAiEngine>();
            var dashboard = await engine.GetDashboardAsync();

            return Ok(dashboard);
        }


        [HttpGet("ai/ceo-dashboard")]
        public async Task<IActionResult> GetCeoDashboard()
        {
            var engine = HttpContext.RequestServices.GetRequiredService<EnterpriseERP.Services.AI.CEO.CeoDashboardEngine>();
            var dashboard = await engine.BuildAsync();

            return Ok(dashboard);
        }

    }
}
