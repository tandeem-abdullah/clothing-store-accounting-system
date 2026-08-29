# 👔 Retail Clothing Store Management & Accounting System

[![Language](https://img.shields.io/badge/Language-C%23-239120?style=flat&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platform](https://img.shields.io/badge/Platform-.NET%20Framework%20%7C%20WinForms-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/Database-Microsoft%20SQL%20Server-CC292B?style=flat&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Reporting](https://img.shields.io/badge/Reports-Crystal%20Reports-00758F?style=flat)](https://www.sap.com/)

An integrated desktop-based enterprise management and point-of-sale (POS) system engineered in **C# (.NET Windows Forms)** and backed by **Microsoft SQL Server**. Designed specifically for retail apparel businesses to automate daily point-of-sale transactions, multi-category inventory tracking, supplier purchasing workflows, customer debt ledgers, and financial reporting.

---

## 📌 Key Architectural Modules & Features

### 1. 📊 Interactive Dashboard & Operations
- Real-time business metrics tracking daily sales volume, revenue totals, and pending customer dues.
- Quick navigation shortcuts to core operational modules with role-based dashboard views.

### 2. 🛍️ Point of Sale (POS) & Billing Management
- Fast checkout and invoice generation with automated discount calculations and tax handling.
- Barcode lookup support, item search, and real-time inventory quantity validation to prevent negative stock.

### 3. 📦 Inventory & Stock Control
- Multi-tier product categorization (item types, sizes, seasonal collections, brands).
- Minimum stock threshold warnings and inventory reorder notifications.

### 4. 🛒 Purchasing & Supplier Ledger
- Supplier profile management with comprehensive purchase invoice logging.
- Accounts payable tracking and historical supplier transaction audits.

### 5. 👥 Customer Accounts & Debt Tracking
- Detailed customer profiles with credit limits and outstanding debt balances.
- Historical payment log and debt collection tracking.

### 6. 🔐 User Roles & System Security
- Multi-user authentication with customizable access permissions (Admin, Cashier, Inventory Manager).
- Protected administrative settings and secure session control.

### 7. 📄 Printable Financial & Audit Reports
- Integrated **Crystal Reports** engine for generating and exporting daily sales summaries, profit/loss overviews, invoice printouts, and full inventory audit sheets.

---

## 🛠️ Technology Stack

| Layer | Technology / Tool |
| :--- | :--- |
| **Frontend / UI** | Windows Forms (WinForms), C# (.NET Framework) |
| **Business Logic** | Object-Oriented Architecture (OOP in C#) |
| **Database Tier** | Microsoft SQL Server (T-SQL, Relational Schema, Triggers & Views) |
| **Reporting Engine** | Crystal Reports for Visual Studio |
| **IDE / Version Control** | Visual Studio 2022 / Git & GitHub |

---

## 📂 Project Structure

```text
clothing-store-accounting-system/
├── clothes store/                  # Source Code Directory
│   ├── Dashboard.cs                # Main administrative dashboard
│   ├── FRM_Manage_Product.cs       # Product inventory & stock management
│   ├── FRM_Customers.cs            # Customer CRM and debt records
│   ├── FRM_Pruches.cs              # Supplier purchasing management
│   ├── FRM_Users.cs                # User authentication & role management
│   ├── CrystalReport1.rpt          # Reporting templates
│   ├── App.config                  # Database connection string configuration
│   └── Program.cs                  # Application entry point
├── binmahfoz.sql                   # Complete SQL Server database schema & initial data
├── clothes store.sln               # Visual Studio Solution File
└── .gitignore                      # Visual Studio build artifacts ignore rules
