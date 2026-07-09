using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EnterpriseERP.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            string email = HttpContext.Session.GetString("UserEmail") ?? "";

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return RedirectToAction("Logout", "Account");

            return View(user);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            string email = HttpContext.Session.GetString("UserEmail") ?? "";

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return RedirectToAction("Logout", "Account");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return RedirectToAction("Logout", "Account");

            // Mise à jour des champs
            user.Phone = Request.Form["Phone"].ToString();
            user.Address = Request.Form["Address"].ToString();
            user.Department = Request.Form["Department"].ToString();
            user.Position = Request.Form["Position"].ToString();
            user.PreferredLanguage = Request.Form["PreferredLanguage"].ToString();
            user.Theme = Request.Form["Theme"].ToString();
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            //  Audit : modification du profil
            AuditService.Log(
                _context,
                HttpContext,
                "Profil",
                "Modification",
                $"Profil modifié : {user.FullName}"
            );

            return RedirectToAction("Index");
        }
    }
}
