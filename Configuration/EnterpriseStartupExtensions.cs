using System.Data.Common;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using EnterpriseERP.Data;
using EnterpriseERP.Helpers;
using EnterpriseERP.Middleware;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using EnterpriseERP.Services.Email;
using EnterpriseERP.Services.Trial;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseERP.Configuration;

public static class EnterpriseStartupExtensions
{
    public static WebApplicationBuilder AddEnterpriseErpServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<EnterpriseERP.Services.Export.BrandingService>();

        builder.AddProductionDataProtection();
        builder.AddReverseProxySupport();
        builder.AddCompression();
        builder.AddApplicationServices();
        builder.AddDatabase();
        builder.AddSessionAndAntiforgery();
        builder.AddMobileCors();
        builder.AddJwtAuthentication();

        builder.Services.AddAuthorization();

        return builder;
    }

    public static WebApplication InitializeEnterpriseErpDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseStartup");

        EnsureSqliteDirectoryExists(configuration, logger);
        db.Database.Migrate();
        EnsurePasswordResetColumns(db, logger);
        SeedAdminFromConfiguration(db, configuration, logger);
        EnsureEnterprisePermissions(db);

        return app;
    }

    public static WebApplication UseEnterpriseErpPipeline(this WebApplication app)
    {
        app.UseForwardedHeaders();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseResponseCompression();

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
        app.UseRequestCultureFromSession();
        app.UseMiddleware<HtmlLocalizationMiddleware>();
        app.UseMiddleware<TrialReadOnlyMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapEnterpriseErpRoutes(this WebApplication app)
    {
        app.MapControllers();

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

        return app;
    }

    private static WebApplicationBuilder AddReverseProxySupport(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            if (builder.Environment.IsProduction())
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            }
        });

        return builder;
    }

    private static WebApplicationBuilder AddProductionDataProtection(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsProduction())
            return builder;

        var keysPath = builder.Configuration["DataProtection:KeysPath"]
            ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_DATA_PROTECTION_KEYS")
            ?? "/data/dataprotection-keys";

        Directory.CreateDirectory(keysPath);

        builder.Services
            .AddDataProtection()
            .SetApplicationName("EnterpriseERP")
            .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

        return builder;
    }

    private static WebApplicationBuilder AddCompression(this WebApplicationBuilder builder)
    {
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

        return builder;
    }

    private static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<TranslationService>();
        builder.Services.AddScoped<JwtService>();
        builder.Services.AddScoped<PasswordResetTokenService>();
        builder.Services.AddScoped<PasswordResetService>();
        builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
        builder.Services.AddScoped<EnterpriseERP.Services.AI.EnterpriseAiEngine>();
        builder.Services.AddScoped<EnterpriseERP.Services.AI.CEO.CeoDashboardEngine>();
        builder.Services.AddScoped<TrialPolicyService>();

        return builder;
    }

    private static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        return builder;
    }

    private static WebApplicationBuilder AddSessionAndAntiforgery(this WebApplicationBuilder builder)
    {
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

        return builder;
    }

    private static WebApplicationBuilder AddMobileCors(this WebApplicationBuilder builder)
    {
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

        return builder;
    }

    private static WebApplicationBuilder AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = ResolveJwtSecret(builder.Configuration)
            ?? throw new InvalidOperationException("JWT key is missing. Configure Jwt:Key, JwtSettings:SecretKey, JWT_KEY or ENTERPRISEERP_JWT_KEY in user-secrets or environment variables.");
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

        return builder;
    }

    private static IApplicationBuilder UseRequestCultureFromSession(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
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
    }

    private static void EnsureSqliteDirectoryExists(IConfiguration configuration, ILogger logger)
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

    private static void EnsurePasswordResetColumns(ApplicationDbContext db, ILogger logger)
    {
        var columns = new Dictionary<string, string>
        {
            ["PasswordResetTokenHash"] = "TEXT",
            ["PasswordResetTokenExpiresAt"] = "TEXT",
            ["PasswordResetTokenUsedAt"] = "TEXT",
            ["PasswordResetRequestWindowStartedAt"] = "TEXT",
            ["PasswordResetRequestCount"] = "INTEGER NOT NULL DEFAULT 0",
            ["PasswordResetLockedUntil"] = "TEXT"
        };

        try
        {
            foreach (var column in columns)
            {
                if (SqliteColumnExists(db, "Users", column.Key))
                    continue;

                ExecuteSqliteCommand(db, $"ALTER TABLE Users ADD COLUMN {column.Key} {column.Value}");
                logger.LogInformation("Added missing SQLite column Users.{Column}.", column.Key);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unable to verify password reset columns in SQLite.");
        }
    }

    private static bool SqliteColumnExists(ApplicationDbContext db, string tableName, string columnName)
    {
        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State == ConnectionState.Closed;

        if (shouldClose)
            connection.Open();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName})";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var name = reader["name"]?.ToString();
                if (string.Equals(name, columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
        finally
        {
            if (shouldClose)
                connection.Close();
        }
    }

    private static void ExecuteSqliteCommand(ApplicationDbContext db, string sql)
    {
        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State == ConnectionState.Closed;

        if (shouldClose)
            connection.Open();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
        finally
        {
            if (shouldClose)
                connection.Close();
        }
    }

    private static void EnsureEnterprisePermissions(ApplicationDbContext db)
    {
        var modules = new[]
        {
            "Dashboard",
            "Clients",
            "Employés",
            "Produits",
            "Stock",
            "Factures",
            "Commandes",
            "Fournisseurs",
            "Paiements",
            "Présences",
            "Exports",
            "Devis",
            "Notifications",
            "Social",
            "RH",
            "Projets",
            "Ecommerce",
            "Finance avancée",
            "IA",
            "Paramètres",
            "Audit",
            "Utilisateurs"
        };

        var actions = new[] { "Voir", "Créer", "Modifier", "Supprimer", "Exporter" };

        foreach (var module in modules)
        {
            foreach (var action in actions)
            {
                if (db.Permissions.Any(p => p.Module == module && p.Action == action))
                    continue;

                db.Permissions.Add(new Permission
                {
                    Module = module,
                    Action = action,
                    Description = $"{action} {module}"
                });
            }
        }

        db.SaveChanges();

        var permissions = db.Permissions.ToList();
        GrantMissingRolePermissions(db, permissions, "Admin", modules, actions);
        GrantMissingRolePermissions(db, permissions, "Manager",
            new[] { "Dashboard", "Clients", "Produits", "Stock", "Factures", "Commandes", "Paiements", "Présences", "Exports", "Devis", "Notifications", "Social", "RH", "Projets", "Ecommerce", "Finance avancée", "IA" },
            new[] { "Voir", "Créer", "Modifier", "Exporter" });
        GrantMissingRolePermissions(db, permissions, "Employee",
            new[] { "Dashboard", "Clients", "Produits", "Stock", "Présences", "Notifications", "Projets" },
            new[] { "Voir", "Créer", "Modifier" });
        GrantMissingRolePermissions(db, permissions, "Comptable",
            new[] { "Dashboard", "Clients", "Factures", "Paiements", "Exports", "Devis", "Finance avancée", "IA" },
            new[] { "Voir", "Créer", "Modifier", "Exporter" });
        GrantMissingRolePermissions(db, permissions, "RH",
            new[] { "Dashboard", "Employés", "Présences", "RH", "Projets", "Notifications", "IA" },
            new[] { "Voir", "Créer", "Modifier", "Exporter" });
        GrantMissingRolePermissions(db, permissions, "Client",
            new[] { "Dashboard", "Devis", "Factures", "Notifications" },
            new[] { "Voir" });

        db.SaveChanges();
    }

    private static void GrantMissingRolePermissions(
        ApplicationDbContext db,
        List<Permission> permissions,
        string role,
        IEnumerable<string> modules,
        IEnumerable<string> actions)
    {
        var moduleSet = modules.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var actionSet = actions.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var targetPermissions = permissions
            .Where(p => moduleSet.Contains(p.Module) && actionSet.Contains(p.Action))
            .ToList();

        foreach (var permission in targetPermissions)
        {
            if (db.RolePermissions.Any(rp => rp.Role == role && rp.PermissionId == permission.Id))
                continue;

            db.RolePermissions.Add(new RolePermission
            {
                Role = role,
                PermissionId = permission.Id
            });
        }
    }

    private static void SeedAdminFromConfiguration(ApplicationDbContext db, IConfiguration configuration, ILogger logger)
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

    private static bool IsWeakJwtSecret(string secretKey)
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

    private static string? ResolveJwtSecret(IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");
        var jwtSettings = configuration.GetSection("JwtSettings");

        return jwt["Key"]
            ?? jwtSettings["SecretKey"]
            ?? Environment.GetEnvironmentVariable("JWT_KEY")
            ?? Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_JWT_KEY")
            ?? Environment.GetEnvironmentVariable("ENTERPRISEERP_JWT_SECRET");
    }
}
