using System.Text;
using EnterpriseERP.Data;
using EnterpriseERP.Middleware;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<EnterpriseERP.Services.Export.BrandingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<TranslationService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<EnterpriseERP.Services.AI.EnterpriseAiEngine>();
builder.Services.AddScoped<EnterpriseERP.Services.AI.CEO.CeoDashboardEngine>();
builder.Services.AddScoped<TrialPolicyService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var jwt = builder.Configuration.GetSection("Jwt");
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwt["Key"] ?? jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT key is missing. Configure Jwt:Key or JwtSettings:SecretKey in user-secrets or environment variables.");
var issuer = jwt["Issuer"] ?? jwtSettings["Issuer"] ?? "EnterpriseERP";
var audience = jwt["Audience"] ?? jwtSettings["Audience"] ?? "EnterpriseERP.Mobile";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("MobileCors");

app.UseSession();

app.Use(async (context, next) =>
{
    var language = TranslationService.NormalizeLanguage(
        context.Session.GetString("Language")
        ?? context.Request.Cookies["Language"]
        ?? context.Request.Headers.AcceptLanguage.ToString());
    var cultureName = TranslationService.SupportedCultures.TryGetValue(language, out var culture)
        ? culture
        : "fr-FR";
    var cultureInfo = CultureInfo.GetCultureInfo(cultureName);

    CultureInfo.CurrentCulture = cultureInfo;
    CultureInfo.CurrentUICulture = cultureInfo;

    await next();
});

app.UseMiddleware<EnterpriseERP.Middleware.HtmlLocalizationMiddleware>();
app.UseMiddleware<TrialReadOnlyMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "invoices",
    pattern: "Invoices",
    defaults: new { controller = "Invoices", action = "Index" });

app.MapControllerRoute(
    name: "orders",
    pattern: "Orders",
    defaults: new { controller = "Orders", action = "Index" });

app.MapControllerRoute(
    name: "quotes",
    pattern: "Quotes",
    defaults: new { controller = "Quotes", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
