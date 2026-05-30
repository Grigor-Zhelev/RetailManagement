# Retail Management System

A distributed multi-store retail management system built with **.NET 9**, **EF Core 9**, and **SQL Server**. Demonstrates real-world backend architecture with centralized inventory synchronization across multiple store instances via REST APIs.

## Overview

The system simulates a retail chain (similar to Billa, Kaufland, or Lidl) with independent store nodes and a central hub. Each store operates autonomously with its own database, while staying synchronized with the central system in real time.

## Architecture

```
┌─────────────┐     REST API     ┌─────────────────┐
│  StoreApp   │ ◄──────────────► │   CentralApp    │
│  (Lyulin)   │                  │  (Central Hub)  │
├─────────────┤                  │                 │
│  StoreApp   │ ◄──────────────► │  - Inventory    │
│ (Dianabad)  │                  │  - Sync Engine  │
├─────────────┤                  │  - REST API     │
│  StoreApp   │ ◄──────────────► │  - HTML UI      │
│  (Bukston)  │                  └─────────────────┘
└─────────────┘
```

- **StoreApp** — Per-store REST API with its own isolated SQL Server database
- **CentralApp** — Central hub that receives and dispatches data to/from all stores
- **Bidirectional sync** — Store → Central (on product add/update) and Central → Store (on central product assignment)

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET 9 / ASP.NET Core Web API |
| ORM | Entity Framework Core 9 |
| Database | SQL Server Express / LocalDB |
| API | RESTful API |
| API Docs | Swagger / OpenAPI |
| Frontend | HTML / JavaScript (CentralApp UI) |

## Store Instances

| Store | Port | Database |
|-------|------|----------|
| Billa Lyulin | 5155 | StoreDb1 |
| Billa Dianabad | 5156 | StoreDb2 |
| Billa Bukston | 5157 | StoreDb3 |
| Central | 5001 | CentralDb |

## Project Structure

```
RetailManagement/
├── StoreApp/
│   ├── Controllers/        ← ProductsController
│   ├── Data/               ← StoreDbContext
│   ├── Models/             ← Product
│   └── Services/           ← SyncService (pushes to Central)
│
└── CentralApp/
    ├── Controllers/        ← ProductsController
    ├── Data/               ← CentralDbContext
    ├── Models/             ← Product (with StoreId, StoreName)
    ├── Services/           ← SyncService (pushes to Store)
    └── wwwroot/            ← index.html (UI)
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- SQL Server Express or LocalDB
- Visual Studio 2022+ or VS Code

### 1. Clone the repository

```bash
git clone https://github.com/Grigor-Zhelev/RetailManagement
cd RetailManagement
```

### 2. Create the databases

```powershell
# Lyulin store
cd StoreApp
$env:ASPNETCORE_ENVIRONMENT="Lyulin"
dotnet ef database update

# Dianabad store
$env:ASPNETCORE_ENVIRONMENT="Dianabad"
dotnet ef database update

# Bukston store
$env:ASPNETCORE_ENVIRONMENT="Bukston"
dotnet ef database update

# Central hub
cd ..\CentralApp
dotnet ef database update
```

### 3. Run all instances (4 terminals)

```bash
# Terminal 1 — Central Hub
cd CentralApp
dotnet run --urls "http://localhost:5001"

# Terminal 2 — Billa Lyulin
cd StoreApp
dotnet run --launch-profile "Lyulin"

# Terminal 3 — Billa Dianabad
cd StoreApp
dotnet run --launch-profile "Dianabad"

# Terminal 4 — Billa Bukston
cd StoreApp
dotnet run --launch-profile "Bukston"
```

## Usage

### Web UI (Central App)

Open your browser at `http://localhost:5001`

- Select a store from the dropdown
- Add, edit, or delete products
- Changes sync automatically to/from the selected store

### Swagger API

| Instance | Swagger URL |
|----------|-------------|
| Central | http://localhost:5001/swagger |
| Billa Lyulin | http://localhost:5155/swagger |
| Billa Dianabad | http://localhost:5156/swagger |
| Billa Bukston | http://localhost:5157/swagger |

## Sync Logic

**Store → Central** (automatic on product change):
```
Store adds product → saves to own DB → pushes to Central
```

**Central → Store** (on store selection):
```
Central assigns product to Lyulin → saves to CentralDb → pushes to Lyulin store
```

> Each store sees only its own products. Dianabad cannot see Lyulin's inventory and vice versa.

## Author

**Grigor Zhelev** — Senior Software Engineer  
[LinkedIn](https://www.linkedin.com/in/grigor-zhelev-ph-d-6312946/) | [GitHub](https://github.com/Grigor-Zhelev)
