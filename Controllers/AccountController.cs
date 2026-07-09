using EnterpriseERP.Data;
using EnterpriseERP.Helpers;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EnterpriseERP.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly TranslationService _translation;
        private readonly TrialPolicyService _trialPolicy;

        public AccountController(ApplicationDbContext context, TranslationService translation, TrialPolicyService trialPolicy)
        {
            _context = context;
            _translation = translation;
            _trialPolicy = trialPolicy;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = _translation.T("FillAllFields");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || user.PasswordHash != PasswordHelper.HashPassword(password))
            {
                ViewBag.Error = _translation.T("InvalidLogin");
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Ce compte est désactivé.";
                return View();
            }

            if (!user.IsApproved)
            {
                ViewBag.Error = "Ce compte n'a pas encore été approuvé.";
                return View();
            }

            user.LastLogin = DateTime.Now;
            user.LastConnection = DateTime.Now;
            user.LoginCount += 1;
            user.LastIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            _context.SaveChanges();

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("IsSuperAdmin", user.IsSuperAdmin ? "true" : "false");

            // 🔥 Audit : connexion
            AuditService.Log(
                _context,
                HttpContext,
                "Connexion",
                "Login",
                $"{user.FullName} s'est connecté"
            );

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register()
        {
            bool hasUsers = _context.Users.Any();

            if (hasUsers && !CanCreateUsers())
                return RedirectToAction("Index", "Dashboard");

            LoadRegisterViewBags(hasUsers);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public IActionResult RegisterPost()
        {
            bool hasUsers = _context.Users.Any();

            if (hasUsers && !CanCreateUsers())
                return RedirectToAction("Index", "Dashboard");

            string fullName = Request.Form["FullName"].ToString();
            string email = Request.Form["Email"].ToString();
            string password = Request.Form["Password"].ToString();
            string requestedRole = Request.Form["Role"].ToString();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = _translation.T("FillAllFields");
                LoadRegisterViewBags(hasUsers);
                return View("Register");
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Cette adresse e-mail est déjà utilisée.";
                LoadRegisterViewBags(hasUsers);
                return View("Register");
            }

            var userLimit = _trialPolicy.CanCreateUserAsync(HttpContext.RequestAborted).GetAwaiter().GetResult();
            if (!userLimit.Allowed)
            {
                ViewBag.Error = userLimit.Message;
                LoadRegisterViewBags(hasUsers);
                return View("Register");
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = PasswordHelper.HashPassword(password),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true,
                IsApproved = true
            };

            if (!hasUsers)
            {
                user.Role = TrialLimits.TrialRole;
                user.IsSuperAdmin = true;
            }
            else
            {
                bool isSuperAdmin = HttpContext.Session.GetString("IsSuperAdmin") == "true";
                string currentRole = HttpContext.Session.GetString("UserRole") ?? "";

                if (requestedRole == "SuperAdmin")
                    requestedRole = "Admin";

                if (requestedRole == "Admin" && !isSuperAdmin)
                {
                    ViewBag.Error = "Seul le SuperAdmin peut créer un Admin.";
                    LoadRegisterViewBags(hasUsers);
                    return View("Register");
                }

                if (currentRole == "Admin" && requestedRole == "Admin")
                {
                    ViewBag.Error = "Un Admin ne peut pas créer un autre Admin.";
                    LoadRegisterViewBags(hasUsers);
                    return View("Register");
                }

                user.Role = string.IsNullOrWhiteSpace(requestedRole) ? "Employee" : requestedRole;
                user.IsSuperAdmin = false;

                if (int.TryParse(HttpContext.Session.GetString("UserId"), out int creatorId))
                    user.CreatedById = creatorId;
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            // 🔥 Audit : création de compte
            AuditService.Log(
                _context,
                HttpContext,
                "Utilisateurs",
                "Création",
                $"Création du compte : {user.FullName}"
            );

            if (!hasUsers)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("IsSuperAdmin", "true");
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // 🔥 Audit : déconnexion
            AuditService.Log(
                _context,
                HttpContext,
                "Connexion",
                "Logout",
                $"{HttpContext.Session.GetString("UserName")} s'est déconnecté"
            );

            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        private bool CanCreateUsers()
        {
            string role = HttpContext.Session.GetString("UserRole") ?? "";
            bool isSuperAdmin = HttpContext.Session.GetString("IsSuperAdmin") == "true";

            return isSuperAdmin || role == "Admin";
        }

        private void LoadRegisterViewBags(bool hasUsers)
        {
            ViewBag.HasUsers = hasUsers;
            ViewBag.CurrentRole = HttpContext.Session.GetString("UserRole");
            ViewBag.IsSuperAdmin = HttpContext.Session.GetString("IsSuperAdmin");
        }
    }
}
