# 🛒 SalesPoint POS System

A modular Point of Sale (POS) web application built with ASP.NET Core MVC and Identity. It supports role-based access for Admins, Managers, and Cashiers, with features for user management, sales transactions, inventory, and reporting.

---

## 📌 Features by Role

### 🔐 Admin

-   ✅ User Management (Add/Edit/Delete with role filtering)
-   ✅ Audit Logs (searchable by user, action, and date)
-   ✅ Inventory and Categories management
-   ✅ Transaction History (filterable)
-   ✅ Reporting with metrics and chart export (PDF/Excel)

### 👨‍💼 Manager

-   ✅ Inventory and Categories management
-   ✅ Transaction History (filterable by cashier)
-   ✅ Reporting with export features

### 🧾 Cashier

-   ✅ Sales Panel with:
    -   Product selection by category tab
    -   Cart management and live total
    -   Checkout confirmation and receipt printing
-   ✅ View own transaction history (read-only)

---

## 🧱 Technology Stack

| Layer            | Technology                             |
| ---------------- | -------------------------------------- |
| Backend          | ASP.NET Core MVC                       |
| Authentication   | ASP.NET Core Identity + JWT            |
| Database         | SQL Server with Entity Framework Core  |
| Frontend         | Razor Views with Tailwind              |
| PDF/Excel Export | [Rotativa/Aspose/ClosedXML (optional)] |
| Charts           | Chart.js or ApexCharts (suggested)     |

---

## 🗂 Project Structure

```plaintext
SalesPoint/
├── Areas/
│   ├── Admin/
│   ├── Management/
│   ├── Sales/
├── Controllers/
├── Models/
├── DTO/
├── Enum/
├── Exceptions/
├── Interfaces/
├── Services/
├── Repositories/
├── Data/
│   └── AppDbContext.cs
│   └── IdentitySeeder.cs
├── Migrations/
├── Views/
│   └── Shared/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
├── Program.cs
├── package.json
├── appsettings.json
```

---

## ⚙️ Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/MDanielle007/SalesPoint.git
cd SalesPoint
```

### 2. Update Configuration

Setup your connection string and JWT config and write a `appsettings.Development.json`:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "DefaultConnectionString": "Your SQL Server connection string here"
    },
    "Jwt": {
        "Key": "your-strong-key-32bit",
        "Issuer": "SalesPointApp",
        "Audience": "SalesPointUsers",
        "ExpireHours": 12
    },
    "SeedAdmin": {
        "Email": "admin@localhost.com",
        "Password": "Admin123!"
    }
}
```

### 3. Run Migrations

In **Visual Studio**, open the **Package Manager Console** and run:

```powershell
EntityFrameworkCore\Update-Database
```

> If you’re using dotnet CLI instead:

```bash
dotnet ef database update
```

### 4. 🎨 Tailwind CSS Setup

Install Tailwind CSS dependencies (from the `/MyApp` directory with `package.json`):

```bash
cd MyApp
npm install
```

> Run this to start taiwind

```bash
npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch
```

---

### 5. Run the Application

You can run the application in two ways:

#### ✅ Option 1: Using .NET CLI

```bash
dotnet run
```

#### ✅ Option 2: Using Visual Studio

-   Open the project in Visual Studio.
-   Click **"Run"** or press **F5**.

Once running, navigate to:

## 📍 [https://localhost:7145/](https://localhost:7145/)

## 🧪 Seeding

On first run, the app will:

-   Create roles from the `UserRole` enum (Admin, Manager, Cashier)
-   Seed a default admin account from `appsettings.json`

---

## 🔐 Role-Based Access

| Area       | Required Role      |
| ---------- | ------------------ |
| Admin      | `Admin`            |
| Management | `Admin`, `Manager` |
| Sales      | `Cashier`          |

Handled via `[Authorize(Roles = "...")]` and policies in `Program.cs`.

---

## 🛡️ Git Best Practices

-   `appsettings.Development.json` is ignored via `.gitignore`.
-   Do **not** commit secrets or connection strings.
-   Reset history by deleting `.git` folder (if needed):

```powershell
Remove-Item -Recurse -Force .git
git init
git add .
git commit -m "Initial commit"
```
