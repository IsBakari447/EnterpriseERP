using EnterpriseERP.Configuration;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.AddEnterpriseErpServices();

var app = builder.Build();

app.InitializeEnterpriseErpDatabase();
app.UseEnterpriseErpPipeline();
app.MapEnterpriseErpRoutes();

app.Run();
