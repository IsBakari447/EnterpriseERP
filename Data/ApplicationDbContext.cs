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
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<HrSchedule> HrSchedules => Set<HrSchedule>();
        public DbSet<PayrollSlip> PayrollSlips => Set<PayrollSlip>();
        public DbSet<HrDocument> HrDocuments => Set<HrDocument>();
        public DbSet<ProjectBoard> ProjectBoards => Set<ProjectBoard>();
        public DbSet<ProjectTaskItem> ProjectTaskItems => Set<ProjectTaskItem>();
        public DbSet<EcommerceConnection> EcommerceConnections => Set<EcommerceConnection>();
        public DbSet<BankReconciliation> BankReconciliations => Set<BankReconciliation>();
        public DbSet<CashflowForecast> CashflowForecasts => Set<CashflowForecast>();

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

            modelBuilder.Entity<LeaveRequest>()
                .HasIndex(l => new { l.EmployeeId, l.Status, l.StartDate });

            modelBuilder.Entity<HrSchedule>()
                .HasIndex(s => new { s.EmployeeId, s.WorkDate });

            modelBuilder.Entity<PayrollSlip>()
                .Property(p => p.GrossAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PayrollSlip>()
                .Property(p => p.NetAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PayrollSlip>()
                .HasIndex(p => new { p.EmployeeId, p.Period });

            modelBuilder.Entity<HrDocument>()
                .HasIndex(d => new { d.EmployeeId, d.DocumentType });

            modelBuilder.Entity<ProjectBoard>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.ProjectBoard)
                .HasForeignKey(t => t.ProjectBoardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectBoard>()
                .HasIndex(p => new { p.Status, p.Deadline });

            modelBuilder.Entity<ProjectTaskItem>()
                .HasIndex(t => new { t.Status, t.Deadline });

            modelBuilder.Entity<EcommerceConnection>()
                .HasIndex(e => e.Platform);

            modelBuilder.Entity<BankReconciliation>()
                .Property(b => b.StatementBalance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BankReconciliation>()
                .Property(b => b.ErpBalance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BankReconciliation>()
                .HasIndex(b => new { b.Status, b.StatementDate });

            modelBuilder.Entity<CashflowForecast>()
                .Property(c => c.ExpectedInflow)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CashflowForecast>()
                .Property(c => c.ExpectedOutflow)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CashflowForecast>()
                .HasIndex(c => new { c.Period, c.Scenario });
        }
    }
}
