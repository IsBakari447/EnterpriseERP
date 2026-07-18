# EnterpriseERP

EnterpriseERP is a professional ERP web platform built with ASP.NET Core 8, Entity Framework Core, SQLite, QuestPDF and ClosedXML. The project centralizes the essential modules of a company: CRM, customers, suppliers, products, stock, quotes, invoices, payments, attendance, expenses, exports, audit logs, roles, security, AI assistant and CEO dashboard.

Current production URL:

```text
https://enterpriseerp-1.onrender.com
```

## Main Features

- CEO dashboard with financial indicators, sales, stock, invoices and charts.
- Customer and supplier CRM.
- Products, stock and stock movements.
- Professional quotes with invoice conversion.
- Invoices, PDF generation, printing and payment tracking.
- Professional Excel exports.
- User, role and permission management.
- Security audit and activity logging.
- Free trial with business limits.
- Internationalization through the internal translation service.
- Mobile API with JWT authentication.
- QA endpoints: `/health` and `/health/ready`.

## Tech Stack

- .NET 8 / ASP.NET Core MVC
- Entity Framework Core 8
- SQLite
- QuestPDF
- ClosedXML
- JWT Bearer
- Docker
- Render for deployment

## Project Structure

```text
Controllers/              MVC pages and API controllers
Controllers/Api/          Mobile/API endpoints
Data/                     EF Core DbContext
Helpers/                  Security and utility helpers
Middleware/               Custom middleware
Models/                   Business entities
Services/                 Business, AI, PDF, export and translation services
Views/                    Razor views
wwwroot/                  CSS, JS, images and public assets
Migrations/               EF Core migrations
SECURITY_QA_CHECKLIST.md  QA and pentest checklist
Dockerfile                Production Docker build
```

## Requirements

- .NET SDK 8
- Git
- Docker, optional for container testing

Check your installation:

```bash
dotnet --version
git --version
docker --version
```

## Local Setup

Clone the project:

```bash
git clone https://github.com/IsBakari447/EnterpriseERP.git
cd EnterpriseERP
```

Restore and build:

```bash
dotnet restore
dotnet build --no-restore
```

Run locally:

```bash
dotnet run --project EnterpriseERP.csproj
```

By default, the application uses SQLite:

```json
"DefaultConnection": "Data Source=enterpriseerp.db"
```

## Sensitive Configuration

Never commit real secrets to the repository.

Important variables:

```text
Jwt__Key=a_long_random_key_with_at_least_32_characters
Cors__AllowedOrigins__0=https://your-domain.com
DataProtection__KeysPath=/data/dataprotection-keys
```

In production, `Jwt__Key` is required. The application refuses weak or placeholder JWT keys.

## Render Deployment

The project includes a `Dockerfile`. Render can build the application directly from GitHub.

Recommended configuration:

```text
Repository: https://github.com/IsBakari447/EnterpriseERP
Branch: main
Dockerfile: Dockerfile
Port: 8080
Disk path: /data
```

Minimum Render environment variables:

```text
ASPNETCORE_ENVIRONMENT=Production
Jwt__Key=a_long_random_key_with_at_least_32_characters
Cors__AllowedOrigins__0=https://enterpriseerp-1.onrender.com
DataProtection__KeysPath=/data/dataprotection-keys
```

The production SQLite database is configured as:

```json
"DefaultConnection": "Data Source=/data/enterpriseerp.db"
```

ASP.NET Data Protection keys are stored in:

```text
/data/dataprotection-keys
```

This prevents session and antiforgery errors after redeployment.

## QA Endpoints

Verify that the application is running:

```http
GET /health
```

Verify that the application can connect to the database:

```http
GET /health/ready
```

Examples:

```bash
curl https://enterpriseerp-1.onrender.com/health
curl https://enterpriseerp-1.onrender.com/health/ready
```

## Useful QA Commands

Debug build:

```bash
dotnet build --no-restore
```

Release build, close to Render:

```bash
dotnet build -c Release --no-restore
```

Release publish:

```bash
dotnet publish EnterpriseERP.csproj -c Release -o ./publish
```

Docker test:

```bash
docker build -t enterpriseerp .
docker run --rm -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production -e Jwt__Key=a_long_random_key_with_at_least_32_characters enterpriseerp
```

## Security

- Recent passwords use PBKDF2 with a unique salt.
- Legacy SHA256 hashes are accepted only for migration and are rehashed after a successful login.
- Plain text password fallback is disabled.
- Session and antiforgery cookies are separated and secured in production.
- CORS is restrictive in production.
- Data Protection keys are persisted on Render through `/data`.
- Private pages redirect to `/Account/Login` when the user is not authenticated.

## Pre-Production Checklist

- Configure `Jwt__Key` in Render.
- Replace placeholder domains in `Cors:AllowedOrigins`.
- Verify `/health` and `/health/ready`.
- Verify login/register.
- Test Admin, Manager and Employee permissions.
- Test customer, product, quote, invoice and payment creation.
- Test invoice/quote PDF generation.
- Test Excel exports.
- Test language switching.
- Test read-only mode after the trial ends.

## Ignored Files

The repository ignores runtime files:

```text
bin/
obj/
*.db
*.db-shm
*.db-wal
*.log
wwwroot/uploads/
Backups/
```

These files must not be versioned.

## Maintenance

Before each push:

```bash
git status
dotnet build -c Release --no-restore
```

Then:

```bash
git add .
git commit -m "Clear message"
git push origin main
```

If Render does not redeploy automatically, use:

```text
Render Dashboard -> Manual Deploy -> Deploy latest commit
```

## Author

Project developed by Issa Bakari.

GitHub:

```text
https://github.com/IsBakari447
```
