# EnterpriseERP

EnterpriseERP is a professional ERP web platform built with ASP.NET Core 8, Entity Framework Core, SQLite, QuestPDF and ClosedXML. The project centralizes the essential modules of a company: CRM, customers, suppliers, products, stock, quotes, invoices, payments, attendance, expenses, exports, audit logs, roles, security, social feedback, AI assistant and CEO dashboard.

Current production URL:

```text
https://enterpriseerp-1.onrender.com
```

## Main Features

- CEO dashboard with financial indicators, sales, stock, invoices and charts.
- Customer and supplier CRM.
- Products, stock and stock movements, with product categories limited to EnterpriseERP, Mobile and Cloud across the web form and mobile API.
- Professional quotes with invoice conversion.
- Invoices, PDF generation, printing and payment tracking.
- Professional Excel exports.
- User, role and permission management.
- Security audit and activity logging.
- Social module with user feedback, review and rating management, service likes and moderation tools.
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

## Product Categories

Products are organized with a controlled category list:

```text
EnterpriseERP
Mobile
Cloud
```

The MVC product creation form uses a dropdown for these categories, and the mobile API validates the same list when creating or updating products. This keeps the product catalog consistent across the EnterpriseERP web app and mobile clients.

## Sensitive Configuration

Never commit real secrets to the repository.

Important variables:

```text
Jwt__Key=a_long_random_key_with_at_least_32_characters
JWT_KEY=a_long_random_key_with_at_least_32_characters
Cors__AllowedOrigins__0=https://your-domain.com
DataProtection__KeysPath=/data/dataprotection-keys
```

In production, a JWT key is required. You can use either `Jwt__Key` or `JWT_KEY`.
The application refuses weak or placeholder JWT keys.

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
# or JWT_KEY=a_long_random_key_with_at_least_32_characters
Cors__AllowedOrigins__0=https://enterpriseerp-1.onrender.com
DataProtection__KeysPath=/data/dataprotection-keys
```

Generate a strong JWT key before deploying:

```powershell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Maximum 256 }))
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

## Demo and Mobile Corrections

Recent demo-focused fixes:

- Added mobile registration endpoint: `POST /api/mobile/auth/register`.
- The first registered user is automatically created as `SuperAdmin` to make the initial demo easier.
- Mobile registration can also create the first company profile when `CompanyName` is provided.
- Mobile login and registration return JWT data compatible with `EnterpriseERP.Mobile`.
- Health endpoints remain available at `/health` and `/health/ready` for the mobile API test button.

For a phone demo, run the backend on the PC and configure the mobile app with the PC LAN IP, for example:

```text
http://192.168.1.20:5167/
```

Do not use `localhost` from a real Android phone; it points to the phone itself, not to the PC.

## Social Feedback Module

EnterpriseERP includes a social features module available at:

```http
GET /Social/Index
```

The module provides:

- Feedback collection with category, priority, status and optional admin response.
- Review and rating management with 1 to 5 star scoring.
- Service likes for EnterpriseERP modules and services.
- Moderation actions for feedback status and review approval.
- Audit logging for feedback, review, like and moderation actions.
- Role-based permissions through the `Social` permission module.

Database tables added by the social module:

```text
Feedbacks
Reviews
SocialLikes
```

After deployment, make sure the latest EF Core migrations are applied. Super admins can access the module directly. For Admin, Manager or Employee accounts, assign the required `Social` permissions from the Roles & Permissions page.

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

## User Manual and Video Tutorials

The simple non-technical user manual displayed from the public home page is available at:

```text
MANUEL_UTILISATEUR.md
```

Technical suite documentation and the EnterpriseERP Web video tutorial script are also available in this repository:

```text
docs/MANUEL_UTILISATION_SUITE_ENTERPRISEERP.md
docs/tutoriels-video/01_ENTERPRISEERP_WEB.md
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
- Test the Social module: feedback creation, review rating, likes and moderation permissions.
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

## Author

Project developed by Issa Bakari.

GitHub:

```text
https://github.com/IsBakari447
```
