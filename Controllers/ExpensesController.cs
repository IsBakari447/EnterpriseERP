using EnterpriseERP.Data;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _context.Expenses
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();

            ViewBag.TotalExpenses = expenses.Sum(e => e.Amount);

            return View(expenses);
        }

        public IActionResult Create()
        {
            return View(new Expense
            {
                ExpenseDate = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return View(expense);
            }

            expense.CreatedAt = DateTime.Now;
            expense.CreatedBy = HttpContext.Session.GetString("UserEmail") ?? "System";

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Dépense ajoutée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(expense);
            }

            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null)
            {
                return NotFound();
            }

            existingExpense.Title = expense.Title;
            existingExpense.Category = expense.Category;
            existingExpense.Amount = expense.Amount;
            existingExpense.ExpenseDate = expense.ExpenseDate;
            existingExpense.Description = expense.Description;
            existingExpense.PaymentMethod = expense.PaymentMethod;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Dépense modifiée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Dépense supprimée avec succès.";
            return RedirectToAction(nameof(Index));
        }
    }
}