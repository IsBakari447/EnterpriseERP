using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers;

public class SocialController : Controller
{
    private readonly ApplicationDbContext _context;

    public SocialController(ApplicationDbContext context)
    {
        _context = context;
    }

    [RequirePermission("Social", "Voir")]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToAction("Login", "Account");

        var feedbacks = await _context.Feedbacks
            .OrderByDescending(f => f.CreatedAt)
            .Take(30)
            .ToListAsync();

        var reviews = await _context.Reviews
            .OrderByDescending(r => r.CreatedAt)
            .Take(30)
            .ToListAsync();

        var likes = await _context.SocialLikes
            .OrderByDescending(l => l.CreatedAt)
            .Take(50)
            .ToListAsync();

        ViewBag.Feedbacks = feedbacks;
        ViewBag.Reviews = reviews;
        ViewBag.Likes = likes;
        ViewBag.OpenFeedbackCount = feedbacks.Count(f => f.Status != "Ferme");
        ViewBag.AverageRating = reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating);
        ViewBag.PlatformLikes = await _context.SocialLikes.CountAsync(l => l.TargetType == "Service" && l.TargetId == "EnterpriseERP");
        ViewBag.UserLikedPlatform = await HasUserLikedAsync("Service", "EnterpriseERP");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Social", "Créer")]
    public async Task<IActionResult> CreateFeedback(Feedback feedback)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(feedback.Title) || string.IsNullOrWhiteSpace(feedback.Message))
            return RedirectToAction(nameof(Index));

        feedback.Title = feedback.Title.Trim();
        feedback.Message = feedback.Message.Trim();
        feedback.Category = string.IsNullOrWhiteSpace(feedback.Category) ? "General" : feedback.Category.Trim();
        feedback.Priority = string.IsNullOrWhiteSpace(feedback.Priority) ? "Normale" : feedback.Priority.Trim();
        feedback.Status = "Nouveau";
        feedback.CreatedAt = DateTime.UtcNow;
        StampCreator(feedback);

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        AuditService.Log(_context, HttpContext, "Social", "Feedback", $"Feedback cree : {feedback.Title}");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Social", "Créer")]
    public async Task<IActionResult> CreateReview(Review review)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(review.Title) || string.IsNullOrWhiteSpace(review.Comment))
            return RedirectToAction(nameof(Index));

        review.Title = review.Title.Trim();
        review.Comment = review.Comment.Trim();
        review.Module = string.IsNullOrWhiteSpace(review.Module) ? "EnterpriseERP" : review.Module.Trim();
        review.Rating = Math.Clamp(review.Rating, 1, 5);
        review.CreatedAt = DateTime.UtcNow;
        StampCreator(review);

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        AuditService.Log(_context, HttpContext, "Social", "Avis", $"Avis {review.Rating}/5 cree : {review.Title}");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Social", "Créer")]
    public async Task<IActionResult> ToggleLike(string targetType, string targetId, string? returnUrl = null)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToAction("Login", "Account");

        targetType = string.IsNullOrWhiteSpace(targetType) ? "Service" : targetType.Trim();
        targetId = string.IsNullOrWhiteSpace(targetId) ? "EnterpriseERP" : targetId.Trim();

        var email = CurrentEmail();
        var existing = await _context.SocialLikes.FirstOrDefaultAsync(l =>
            l.TargetType == targetType &&
            l.TargetId == targetId &&
            l.UserEmail == email);

        if (existing == null)
        {
            _context.SocialLikes.Add(new SocialLike
            {
                TargetType = targetType,
                TargetId = targetId,
                UserId = CurrentUserId(),
                UserName = CurrentName(),
                UserEmail = email,
                CreatedAt = DateTime.UtcNow
            });
            AuditService.Log(_context, HttpContext, "Social", "Like", $"Like ajoute : {targetType}/{targetId}");
        }
        else
        {
            _context.SocialLikes.Remove(existing);
            AuditService.Log(_context, HttpContext, "Social", "Unlike", $"Like retire : {targetType}/{targetId}");
        }

        await _context.SaveChangesAsync();
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Social", "Modifier")]
    public async Task<IActionResult> UpdateFeedbackStatus(int id, string status, string? adminResponse)
    {
        var feedback = await _context.Feedbacks.FindAsync(id);
        if (feedback == null)
            return RedirectToAction(nameof(Index));

        feedback.Status = string.IsNullOrWhiteSpace(status) ? feedback.Status : status.Trim();
        feedback.AdminResponse = string.IsNullOrWhiteSpace(adminResponse) ? feedback.AdminResponse : adminResponse.Trim();
        feedback.AdminResponseAt = string.IsNullOrWhiteSpace(adminResponse) ? feedback.AdminResponseAt : DateTime.UtcNow;
        feedback.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "Social", "Feedback status", $"Feedback mis a jour : {feedback.Title}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Social", "Modifier")]
    public async Task<IActionResult> ToggleReviewApproval(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return RedirectToAction(nameof(Index));

        review.IsApproved = !review.IsApproved;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "Social", "Moderation avis", $"Avis modere : {review.Title}");

        return RedirectToAction(nameof(Index));
    }

    private IActionResult SafeRedirect(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> HasUserLikedAsync(string targetType, string targetId)
    {
        var email = CurrentEmail();
        return await _context.SocialLikes.AnyAsync(l =>
            l.TargetType == targetType &&
            l.TargetId == targetId &&
            l.UserEmail == email);
    }

    private void StampCreator(Feedback feedback)
    {
        feedback.CreatedByUserId = CurrentUserId();
        feedback.CreatedByName = CurrentName();
        feedback.CreatedByEmail = CurrentEmail();
    }

    private void StampCreator(Review review)
    {
        review.CreatedByUserId = CurrentUserId();
        review.CreatedByName = CurrentName();
        review.CreatedByEmail = CurrentEmail();
    }

    private int? CurrentUserId()
    {
        return int.TryParse(HttpContext.Session.GetString("UserId"), out var userId) ? userId : null;
    }

    private string CurrentName()
    {
        return HttpContext.Session.GetString("UserName") ?? CurrentEmail();
    }

    private string CurrentEmail()
    {
        return HttpContext.Session.GetString("UserEmail") ?? "";
    }
}
