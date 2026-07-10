using EnterpriseERP.Data;
using EnterpriseERP.Helpers;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Mvc;

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

            var user = _context.Users.FirstOrDefault(u => u.Email == email.Trim());

            if (user == null || user.PasswordHash != PasswordHelper.HashPassword(password))
            {
                ViewBag.Error = _translation.T("InvalidLogin");
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Ce compte est desactive.";
                return View();
            }

            if (!user.IsApproved)
            {
                ViewBag.Error = "Ce compte n'a pas encore ete approuve.";
                return View();
            }

            user.LastLogin = DateTime.Now;
            user.LastConnection = DateTime.Now;
            user.LoginCount += 1;
            user.LastIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            _context.SaveChanges();

            SetSession(user);

            AuditService.Log(
                _context,
                HttpContext,
                "Connexion",
                "Login",
                $"{user.FullName} s'est connecte"
            );

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var hasUsers = _context.Users.Any();

            if (hasUsers && !CanRegisterDuringTrial())
                return RedirectToAction("Index", "Dashboard");

            LoadRegisterViewBags(hasUsers);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Register")]
        public IActionResult RegisterPost()
        {
            var hasUsers = _context.Users.Any();

            if (hasUsers && !CanRegisterDuringTrial())
                return RedirectToAction("Index", "Dashboard");

            var fullName = Request.Form["FullName"].ToString().Trim();
            var email = Request.Form["Email"].ToString().Trim();
            var password = Request.Form["Password"].ToString();
            var requestedRole = Request.Form["Role"].ToString();

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
                ViewBag.Error = "Cette adresse e-mail est deja utilisee.";
                LoadRegisterViewBags(hasUsers);
                return View("Register");
            }

            var userLimit = _trialPolicy.CanCreateUserAsync(HttpContext.RequestAborted).GetAwaiter().GetResult();
            if (!userLimit.Allowed && !CanCreateUsers())
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
                user.Role = "SuperAdmin";
                user.IsSuperAdmin = true;
            }
            else
            {
                user.Role = NormalizeTrialRole(requestedRole);
                user.IsSuperAdmin = false;

                if (int.TryParse(HttpContext.Session.GetString("UserId"), out var creatorId))
                    user.CreatedById = creatorId;
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Utilisateurs",
                "Creation",
                $"Creation du compte : {user.FullName}"
            );

            if (!hasUsers)
                SetSession(user);

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
            AuditService.Log(
                _context,
                HttpContext,
                "Connexion",
                "Logout",
                $"{HttpContext.Session.GetString("UserName")} s'est deconnecte"
            );

            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        private void SetSession(User user)
        {
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("IsSuperAdmin", user.IsSuperAdmin ? "true" : "false");
        }

        private bool CanCreateUsers()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            var isSuperAdmin = HttpContext.Session.GetString("IsSuperAdmin") == "true";

            return isSuperAdmin || role == "SuperAdmin" || role == "Admin";
        }

        private bool CanRegisterDuringTrial()
        {
            if (CanCreateUsers())
                return true;

            var status = _trialPolicy.GetStatusAsync(HttpContext.RequestAborted).GetAwaiter().GetResult();
            return !status.IsPaid && !status.IsReadOnly && status.UsersUsed < TrialLimits.MaxUsers;
        }

        private static string NormalizeTrialRole(string role)
        {
            return role switch
            {
                "Admin" => "Admin",
                "Manager" => "Manager",
                "Employee" => "Employee",
                _ => "Employee"
            };
        }

        private void LoadRegisterViewBags(bool hasUsers)
        {
            ViewBag.HasUsers = hasUsers;
            ViewBag.CurrentRole = HttpContext.Session.GetString("UserRole");
            ViewBag.IsSuperAdmin = HttpContext.Session.GetString("IsSuperAdmin");
            ViewBag.UserCount = _context.Users.Count(u => !u.IsSuperAdmin && u.Role != "SuperAdmin");
            ViewBag.MaxTrialUsers = TrialLimits.MaxUsers;
            ViewBag.CanCreateUsers = CanCreateUsers();
        }
    }
}
