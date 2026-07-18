using EnterpriseERP.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<Presence> Presences => Set<Presence>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<AppSetting> AppSettings => Set<AppSetting>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<Quote> Quotes => Set<Quote>();
        public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();
        public DbSet<CompanyProfile> CompanyProfiles => Set<CompanyProfile>();
        public DbSet<AutomationRule> AutomationRules => Set<AutomationRule>();
        public DbSet<ExternalIntegration> ExternalIntegrations => Set<ExternalIntegration>();
        public DbSet<DataImportJob> DataImportJobs => Set<DataImportJob>();
        public DbSet<DynamicReport> DynamicReports => Set<DynamicReport>();
        public DbSet<CollaborationComment> CollaborationComments => Set<CollaborationComment>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<SocialLike> SocialLikes => Set<SocialLike>();
        public DbSet<DataVersion> DataVersions => Set<DataVersion>();
        public DbSet<TenantAccount> TenantAccounts => Set<TenantAccount>();
        public DbSet<MarketplaceExtension> MarketplaceExtensions => Set<MarketplaceExtension>();
        public DbSet<CustomFieldDefinition> CustomFieldDefinitions => Set<CustomFieldDefinition>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Quote>()
                .Property(q => q.SubTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Quote>()
                .Property(q => q.DiscountRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Quote>()
                .Property(q => q.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Quote>()
                .Property(q => q.TaxRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Quote>()
                .Property(q => q.TaxAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Quote>()
                .Property(q => q.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<QuoteItem>()
                .Property(qi => qi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<QuoteItem>()
                .Property(qi => qi.DiscountRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<QuoteItem>()
                .Property(qi => qi.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<QuoteItem>()
                .Property(qi => qi.TaxRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<QuoteItem>()
                .Property(qi => qi.TaxAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<QuoteItem>()
                .Property(qi => qi.LineTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Quote>()
                .HasMany(q => q.Items)
                .WithOne(i => i.Quote)
                .HasForeignKey(i => i.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyProfile>().HasData(new CompanyProfile
            {
                Id = 1,
                CompanyName = "EnterpriseERP AB",
                Slogan = "Votre succès, notre priorité.",
                Address = "Stockholm, Suède",
                Phone = "+46 70 736 45 55",
                Email = "bakarii447@gmail.com",
                Website = "www.enterpriseerp.com",
                LegalInfo = "Document généré automatiquement par EnterpriseERP.",
                FooterMessage = "Merci pour votre confiance. Nous restons à votre disposition pour vous accompagner.",
                LogoPath = ""
            });

            modelBuilder.Entity<AutomationRule>()
                .HasIndex(a => a.Name);

            modelBuilder.Entity<ExternalIntegration>()
                .HasIndex(i => i.Provider);

            modelBuilder.Entity<DataImportJob>()
                .HasIndex(i => i.Module);

            modelBuilder.Entity<DynamicReport>()
                .HasIndex(r => r.Module);

            modelBuilder.Entity<CollaborationComment>()
                .HasIndex(c => new { c.EntityType, c.EntityId });

            modelBuilder.Entity<Feedback>()
                .HasIndex(f => new { f.Status, f.CreatedAt });

            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.Module, r.IsApproved, r.CreatedAt });

            modelBuilder.Entity<SocialLike>()
                .HasIndex(l => new { l.TargetType, l.TargetId });

            modelBuilder.Entity<SocialLike>()
                .HasIndex(l => new { l.TargetType, l.TargetId, l.UserEmail })
                .IsUnique();

            modelBuilder.Entity<DataVersion>()
                .HasIndex(v => new { v.EntityType, v.EntityId, v.VersionNumber });

            modelBuilder.Entity<TenantAccount>()
                .HasIndex(t => t.Slug)
                .IsUnique();

            modelBuilder.Entity<MarketplaceExtension>()
                .HasIndex(e => e.Category);

            modelBuilder.Entity<CustomFieldDefinition>()
                .HasIndex(f => new { f.EntityType, f.FieldKey })
                .IsUnique();
        }
    }
}
