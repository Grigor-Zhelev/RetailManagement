# Retail Management System

## Идея

Системата симулира управление на верига магазини (като Billa, Kaufland, Lidl).
Състои се от две приложения:

- **StoreApp** — инсталира се във всеки магазин. Има собствена база данни.
- **CentralApp** — централна система която получава данни от всички магазини.

При добавяне или промяна на продукт в магазин — данните автоматично се изпращат към централата.
При добавяне на продукт в централата — данните се изпращат към конкретен магазин.

---

## Технологии

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9
- SQL Server Express
- Swagger (за тестване на API)

---

## Структура

```
RetailManagement/
├── StoreApp/         ← API за магазин
│   ├── Controllers/  ← ProductsController
│   ├── Data/         ← StoreDbContext
│   ├── Models/       ← Product
│   └── Services/     ← SyncService (праща към Central)
│
└── CentralApp/       ← API за централата
    ├── Controllers/  ← ProductsController
    ├── Data/         ← CentralDbContext
    ├── Models/       ← Product (+ StoreId, StoreName)
    ├── Services/     ← SyncService (праща към Store)
    └── wwwroot/      ← index.html (UI)
```

---

## Магазини

| Магазин        | StoreId                              | Порт  | База      |
|----------------|--------------------------------------|-------|-----------|
| Billa Lyulin   | 11111111-1111-1111-1111-111111111111 | 5155  | StoreDb1  |
| Billa Dianabad | 22222222-2222-2222-2222-222222222222 | 5156  | StoreDb2  |
| Billa Bukston  | 33333333-3333-3333-3333-333333333333 | 5157  | StoreDb3  |
| Central        | —                                    | 5001  | CentralDb |

---

## Инсталация

### Изисквания
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- SQL Server Express (или LocalDB)
- Visual Studio 2022+ или VS Code

### Стъпки

**1. Клонирай проекта**
```bash
git clone https://github.com/Grigor-Zhelev/RetailManagement
cd RetailManagement
```

**2. Създай базите данни**
```powershell
# Lyulin
cd StoreApp
$env:ASPNETCORE_ENVIRONMENT="Lyulin"
dotnet ef database update

# Dianabad
$env:ASPNETCORE_ENVIRONMENT="Dianabad"
dotnet ef database update

# Bukston
$env:ASPNETCORE_ENVIRONMENT="Bukston"
dotnet ef database update

# Central
cd ..\CentralApp
dotnet ef database update
```

**3. Провери connection strings**

В `StoreApp/appsettings.Lyulin.json` (и останалите):
```json
"Server=.\\SQLEXPRESS;Database=StoreDb1;Trusted_Connection=True;TrustServerCertificate=True;"
```

В `CentralApp/appsettings.json`:
```json
"Server=.\\SQLEXPRESS;Database=CentralDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

---

## Стартиране

Отвори **4 терминала**:

```powershell
# Терминал 1 — Central
cd CentralApp
dotnet run --urls "http://localhost:5001"

# Терминал 2 — Billa Lyulin
cd StoreApp
dotnet run --launch-profile "Lyulin"

# Терминал 3 — Billa Dianabad
cd StoreApp
dotnet run --launch-profile "Dianabad"

# Терминал 4 — Billa Bukston
cd StoreApp
dotnet run --launch-profile "Bukston"
```

---

## Използване

### UI (Central App)
Отвори браузър на:
```
http://localhost:5001
```
- Избери магазин от dropdown
- Добави / редактирай / изтрий продукти
- Продуктите се синхронизират автоматично

### Swagger (API тестване)
```
http://localhost:5001/swagger   ← Central
http://localhost:5155/swagger   ← Billa Lyulin
http://localhost:5156/swagger   ← Billa Dianabad
http://localhost:5157/swagger   ← Billa Bukston
```

---

## Синхронизация

**Store → Central** (автоматично):
```
Магазин добавя продукт → записва в своята БД → праща към Central
```

**Central → Store** (при избор на магазин):
```
Central добавя продукт за Lyulin → записва в CentralDb → праща към Lyulin
```

**Важно:** Всеки магазин вижда само своите продукти.
Dianabad не вижда продуктите на Lyulin и обратно.
