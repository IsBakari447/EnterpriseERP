# EnterpriseERP Backend QA & Pentest Checklist

## Build and Smoke Tests

- `dotnet restore`
- `dotnet build --no-restore`
- Start the app and verify:
  - `GET /health` returns `200`
  - `GET /health/ready` returns `200`
  - Public pages return `200`
  - Private pages redirect to `/Account/Login` when not authenticated

## Required Production Configuration

- Configure a strong JWT secret outside source control:
  - `Jwt__Key`
  - Minimum 32 characters
  - Random and unique per environment
- Configure CORS explicitly:
  - `Cors__AllowedOrigins__0=https://your-domain.com`
  - Add only trusted web origins
- Keep database files, backups and uploads out of Git.

## Authentication and Authorization

- New passwords use PBKDF2 with per-password salt.
- Legacy SHA256 hashes are accepted only for migration and are rehashed after a successful login.
- Plain text password fallback is intentionally disabled.
- Verify protected modules redirect unauthenticated users to login.
- Verify role permissions for:
  - Dashboard
  - Clients
  - Devis
  - Factures
  - Produits
  - Utilisateurs
  - Parametres

## API and Mobile

- Test JWT login and token expiration.
- Test invalid, expired and tampered tokens.
- Confirm CORS does not allow arbitrary browser origins in production.
- Confirm API errors do not expose stack traces in production.

## Data and Files

- Confirm uploads are stored outside Git-tracked source.
- Confirm backups are protected and not directly downloadable without authorization.
- Confirm PDF/Excel exports require authenticated access.

## Regression Checks

- Login and register
- Dashboard charts
- Create/edit/delete clients
- Create PDF invoice
- Export Excel reports
- Language switch
- Trial read-only mode
