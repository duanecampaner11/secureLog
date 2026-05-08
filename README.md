# SecureLog — Digital Guest Logbook (ASP.NET Core 8 + MySQL)

A web-based guest logbook system. Built with ASP.NET Core MVC 8, EF Core, ASP.NET Identity, and MySQL (via Pomelo).

## Features
- Register / Login / Logout (ASP.NET Identity, hashed passwords)
- Add visitor (auto Time In)
- Time Out, Delete
- Search by name or date (`yyyy-MM-dd`)
- Multi-user, cloud MySQL storage

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL (XAMPP locally, or any cloud MySQL)
- VS Code with the **C# Dev Kit** extension

## Setup

1. Open the folder in VS Code.
2. Edit `appsettings.json` → set your MySQL connection string.

   Local XAMPP example (default):
   ```
   Server=localhost;Port=3306;Database=securelog;User=root;Password=;
   ```
   Create the empty database first in phpMyAdmin: `CREATE DATABASE securelog;`

3. Restore + install EF tools:
   ```bash
   dotnet restore
   dotnet tool install --global dotnet-ef
   ```

4. Create the initial migration & apply it:
   ```bash
   dotnet ef migrations add Init
   dotnet ef database update
   ```
   (Or just `dotnet run` — the app calls `Database.Migrate()` on startup once a migration exists.)

5. Run:
   ```bash
   dotnet run
   ```
   Open the URL printed in the terminal (e.g. `http://localhost:5000`).

## Project Structure
```
SecureLog/
├── Controllers/        # Account, Guest, Home
├── Models/             # ApplicationUser, GuestEntry, ViewModels
├── Data/               # ApplicationDbContext
├── Views/              # Razor views
├── wwwroot/css/        # site.css
├── Program.cs          # App startup, DI, Identity, EF
├── appsettings.json    # Connection string
└── SecureLog.csproj
```

## Deployment Notes
- Set the MySQL connection string via env var `ConnectionStrings__DefaultConnection` in production.
- Enable HTTPS / SSL on your host.
- Works on Azure App Service, Render, Hostinger VPS, etc.
