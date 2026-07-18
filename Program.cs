using System.Text;
using System.Data.Common;
using EnterpriseERP.Data;
using EnterpriseERP.Helpers;
using EnterpriseERP.Middleware;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<EnterpriseERP.Services.Export.BrandingService>();

if (builder.Environment.IsProduction())
{
    var keysPath = builder.Configuration["DataProtection:KeysPath"]
        ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_DATA_PROTECTION_KEYS")
        ?? "/data/dataprotection-keys";

    Directory.CreateDirectory(keysPath);

    builder.Services
        .AddDataProtection()
        .SetApplicationName("EnterpriseERP")
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath));
}

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "image/svg+xml",
        "application/manifest+json"
    });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

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
    options.Cookie.Name = ".EnterpriseERP.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".EnterpriseERP.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsProduction()
        ? CookieSecurePolicy.Always
        : CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MobileCors", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        policy.AllowAnyHeader()
              .AllowAnyMethod();

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }
        else if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin();
        }
    });
});

var jwt = builder.Configuration.GetSection("Jwt");
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwt["Key"] ?? jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT key is missing. Configure Jwt:Key or JwtSettings:SecretKey in user-secrets or environment variables.");
var issuer = jwt["Issuer"] ?? jwtSettings["Issuer"] ?? "EnterpriseERP";
var audience = jwt["Audience"] ?? jwtSettings["Audience"] ?? "EnterpriseERP.Mobile";

if (builder.Environment.IsProduction() && IsWeakJwtSecret(secretKey))
{
    throw new InvalidOperationException("Production JWT key is weak or still uses a placeholder. Configure Jwt:Key with a long random secret.");
}

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseStartup");

    EnsureSqliteDirectoryExists(configuration, logger);
    db.Database.Migrate();
    SeedAdminFromConfiguration(db, configuration, logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseResponseCompression();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseResponseCompression();
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 30;
        context.Context.Response.Headers.CacheControl = $"public,max-age={durationInSeconds}";
    }
});

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
    name: "clients",
    pattern: "Clients",
    defaults: new { controller = "Clients", action = "Index" });

app.MapControllerRoute(
    name: "clientsIndex",
    pattern: "Clients/Index",
    defaults: new { controller = "Clients", action = "Index" });

app.MapControllerRoute(
    name: "clientAlias",
    pattern: "Client/{action=Index}/{id?}",
    defaults: new { controller = "Clients" });

app.MapControllerRoute(
    name: "customersAlias",
    pattern: "Customers/{action=Index}/{id?}",
    defaults: new { controller = "Clients" });

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

static void EnsureSqliteDirectoryExists(IConfiguration configuration, ILogger logger)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
        return;

    try
    {
        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (!builder.TryGetValue("Data Source", out var dataSourceValue))
            return;

        var dataSource = dataSourceValue?.ToString();

        if (string.IsNullOrWhiteSpace(dataSource) || dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
            return;

        var directory = Path.GetDirectoryName(Path.GetFullPath(dataSource));

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Unable to prepare the SQLite database directory.");
    }
}

static void SeedAdminFromConfiguration(ApplicationDbContext db, IConfiguration configuration, ILogger logger)
{
    if (db.Users.Any())
        return;

    var enabled = configuration.GetValue("SeedAdmin:Enabled", false)
        || string.Equals(Environment.GetEnvironmentVariable("ENTERPRISEERP_SEED_ADMIN"), "true", StringComparison.OrdinalIgnoreCase);

    if (!enabled)
    {
        logger.LogInformation("Seed admin is disabled. The first account can be created from /Account/Register.");
        return;
    }

    var email = configuration["SeedAdmin:Email"] ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_ADMIN_EMAIL");
    var password = configuration["SeedAdmin:Password"] ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_ADMIN_PASSWORD");
    var fullName = configuration["SeedAdmin:FullName"] ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_ADMIN_NAME") ?? "EnterpriseERP Admin";

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        logger.LogInformation("No seed admin configured. The first account can be created from /Account/Register.");
        return;
    }

    db.Users.Add(new User
    {
        FullName = fullName,
        Email = email.Trim(),
        PasswordHash = PasswordHelper.HashPassword(password),
        Role = "SuperAdmin",
        IsSuperAdmin = true,
        IsActive = true,
        IsApproved = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    });

    db.SaveChanges();
    logger.LogInformation("Seed admin account created for {Email}.", email);
}

static bool IsWeakJwtSecret(string secretKey)
{
    if (secretKey.Length < 32)
        return true;

    var weakMarkers = new[]
    {
        "CHANGE_THIS",
        "ChangeThis",
        "SuperSecret",
        "EnterpriseERP_2026"
    };

    return weakMarkers.Any(marker => secretKey.Contains(marker, StringComparison.OrdinalIgnoreCase));
}
