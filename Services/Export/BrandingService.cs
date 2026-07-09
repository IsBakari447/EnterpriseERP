using EnterpriseERP.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Services.Export;

public class BrandingService
{
    private readonly ApplicationDbContext _context;

    public BrandingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyBrand> GetBrandAsync()
    {
        var profile = await _context.CompanyProfiles.FirstOrDefaultAsync();
        var settings = await _context.AppSettings.FirstOrDefaultAsync();

        return new CompanyBrand
        {
            CompanyName = profile?.CompanyName ?? settings?.CompanyName ?? "EnterpriseERP AB",
            Slogan = profile?.Slogan ?? "Business Management Suite",
            Address = profile?.Address ?? settings?.CompanyAddress ?? "Stockholm, Suède",
            Phone = profile?.Phone ?? settings?.CompanyPhone ?? "+46 70 736 45 55",
            Email = profile?.Email ?? settings?.CompanyEmail ?? "bakarii447@gmail.com",
            Website = profile?.Website ?? settings?.CompanyWebsite ?? "www.enterpriseerp.com",
            FooterMessage = profile?.FooterMessage ?? settings?.DefaultThankYouMessage ?? "Merci pour votre confiance.",
            PrimaryColor = settings?.PrimaryColor ?? "#2563EB",
            AccentColor = "#7C3AED"
        };
    }
}
