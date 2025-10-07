# 🔧 Mechanic Workshop Management System

A full-stack workshop management application built with **ASP.NET Core** and **Blazor WebAssembly**, implementing **Clean Architecture** and **CQRS** patterns.

---

## ✨ Features

### 🚗 Customer & Vehicle Management
- Customer registration with email and phone validation
- Multi-vehicle tracking per customer
- Vehicle details (Make, Model, Year, License Plate)

### 🔧 Repair Task Management
- Create repair tasks with labor costs and duration estimates (15-180 mins)
- Parts inventory with cost and quantity tracking
- Automatic total cost calculation (labor + parts)

### 📅 Work Order Scheduling
- Visual calendar with 4 workshop spots (A, B, C, D)
- 15-minute time slot intervals
- State machine: `Scheduled → In Progress → Completed/Cancelled`
- Prevent scheduling conflicts (spot, vehicle, mechanic)
- Operating hours validation
- Mechanic assignment with availability checking

### 💰 Invoicing & Billing
- Automatic invoice generation from completed work orders
- Detailed line items with task and parts breakdown
- Discount and tax calculations (configurable tax rate)
- Payment tracking with timestamp
- PDF invoice generation

### 👥 Employee Management
- Role-based system (Labor, Manager)
- Employee assignment to work orders
- Availability tracking

### 🔐 Security & Authentication
- JWT-based authentication with refresh tokens
- ASP.NET Core Identity integration
- Role-based authorization
- Secure password hashing

### 📊 Dashboard & Analytics
- Daily work order statistics
- Revenue and cost tracking
- Completion and cancellation rates
- Profit margin calculations

### ⚡ Performance Features
- Hybrid caching with Redis support
- CQRS pattern for optimized reads/writes
- Pagination for large datasets
- Real-time updates via SignalR

---

## 🏗️ Architecture

Clean Architecture with four layers:

```
MechanicShop/
├── Domain/              # Business logic & entities
│   ├── Customers/
│   ├── Employees/
│   ├── RepairTasks/
│   ├── WorkOrders/
│   └── Common/
│
├── Application/         # Use cases (CQRS handlers)
│   ├── Customers/
│   ├── RepairTasks/
│   ├── WorkOrders/
│   ├── Billing/
│   ├── Identity/
│   └── Common/
│
├── Infrastructure/      # Data access & external services
│   ├── Persistence/
│   ├── Identity/
│   └── Services/
│
└── Presentation/
    ├── Api/            # ASP.NET Core API
    └── Client/         # Blazor WebAssembly
```

### Design Patterns

- **CQRS**: Separate commands and queries with MediatR
- **Repository Pattern**: Data access abstraction
- **Result Pattern**: Type-safe error handling
- **Domain Events**: WorkOrder state changes trigger notifications
- **Strategy Pattern**: WorkOrder validation rules
- **Pipeline Behaviors**: Logging, validation, caching, performance monitoring

---

## 🛠️ Tech Stack

### Backend
- **ASP.NET Core 8.0** - Web API
- **Entity Framework Core** - ORM
- **MediatR** - CQRS implementation
- **FluentValidation** - Request validation
- **ASP.NET Core Identity** - Authentication
- **HybridCache** - Distributed caching

### Frontend
- **Blazor WebAssembly** - SPA framework

### Database
- **SQL Server** - Primary database

### Additional Libraries
- **SignalR** - Real-time notifications
- **Serilog** - Structured logging
- **QuestPDF** - Invoice PDF generation

---

## 📁 Project Structure

### Domain Layer
```
Domain/
├── Customers/
│   ├── Customer.cs              # Aggregate root
│   ├── CustomerErrors.cs
│   └── Vehicles/
│       ├── Vehicle.cs
│       └── VehicleErrors.cs
│
├── WorkOrders/
│   ├── WorkOrder.cs             # Aggregate root with state machine
│   ├── WorkOrderErrors.cs
│   ├── Enums/
│   │   ├── WorkOrderState.cs
│   │   └── Spot.cs
│   ├── Events/
│   │   ├── WorkOrderCompleted.cs
│   │   └── WorkOrderCollectionModified.cs
│   └── Billing/
│       ├── Invoice.cs
│       ├── InvoiceLineItem.cs
│       └── InvoiceStatus.cs
│
└── Common/
    └── Results/
        └── Result.cs            # Result pattern implementation
```

### Application Layer
```
Application/
├── Customers/
│   ├── Commands/
│   │   ├── CreateCustomer/
│   │   ├── UpdateCustomer/
│   │   └── RemoveCustomer/
│   ├── Queries/
│   │   ├── GetCustomers/
│   │   └── GetCustomerById/
│   └── DTOs/
│
├── WorkOrders/
│   ├── Commands/
│   │   ├── CreateWorkOrder/
│   │   ├── UpdateWorkOrderState/
│   │   ├── RelocateWorkOrder/
│   │   ├── AssignLabor/
│   │   └── DeleteWorkOrder/
│   ├── Queries/
│   │   ├── GetWorkOrders/      # Paginated with filters
│   │   └── GetWorkOrderById/
│   └── EventHandlers/
│       └── SendWorkOrderCompletedEmailHandler.cs
│
├── Billing/
│   ├── Commands/
│   │   ├── IssueInvoice/
│   │   └── SettleInvoice/
│   └── Queries/
│       ├── GetInvoiceById/
│       └── GetInvoicePDF/
│
└── Common/
    ├── Behaviors/
    │   ├── ValidationBehavior.cs
    │   ├── LoggingBehavior.cs
    │   ├── CachingBehavior.cs
    │   └── PerformanceBehavior.cs
    └── Interfaces/
        ├── IAppDbContext.cs
        ├── IWorkOrderPolicy.cs
        └── ICachedQuery.cs
```

---

## 🎯 Key Features Implementation

### Work Order State Machine
```csharp
Scheduled → InProgress → Completed
    ↓
Cancelled (before completion only)
```

**Business Rules:**
- Can only transition to `InProgress` after scheduled start time
- Cannot modify work order when `InProgress`, `Completed`, or `Cancelled`
- Completing work order triggers automatic invoice generation

### Scheduling Validation
- Checks operating hours (configurable)
- Prevents spot conflicts (same spot, overlapping time)
- Prevents vehicle double-booking
- Prevents mechanic overbooking

### Invoice Generation
```csharp
Subtotal = Sum of all repair tasks (labor + parts)
Tax = Subtotal × Tax Rate (20%)
Discount = Manual discount amount
Total = Subtotal - Discount + Tax
```

### Caching Strategy
```csharp
// Cached queries with tags for invalidation
- Customers: cached for 10 mins, tag: "customer"
- Work Orders: cached for 10 mins, tag: "work-order"
- Repair Tasks: cached for 10 mins, tag: "repair-task"
- Schedule: cached for 10 mins, tag: "work-order"

// Cache invalidation on mutations
await cache.RemoveByTagAsync("work-order", ct);
```

---

## 📊 API Endpoints

### Authentication
```http
POST   /api/auth/login          # Generate JWT tokens
POST   /api/auth/refresh        # Refresh access token
GET    /api/auth/user           # Get current user
```

### Customers
```http
GET    /api/customers           # List all customers
GET    /api/customers/{id}      # Get customer details
POST   /api/customers           # Create customer
PUT    /api/customers/{id}      # Update customer
DELETE /api/customers/{id}      # Delete customer
```

### Work Orders
```http
GET    /api/workorders                    # Paginated list with filters
GET    /api/workorders/{id}               # Get work order details
POST   /api/workorders                    # Create work order
PUT    /api/workorders/{id}/state         # Update state
PUT    /api/workorders/{id}/relocate      # Reschedule
PUT    /api/workorders/{id}/labor         # Assign mechanic
PUT    /api/workorders/{id}/repair-tasks  # Update tasks
DELETE /api/workorders/{id}               # Delete (Scheduled only)
```

### Invoices
```http
GET    /api/invoices/{id}        # Get invoice
POST   /api/invoices/issue       # Issue invoice for completed work order
PUT    /api/invoices/{id}/settle # Mark as paid
GET    /api/invoices/{id}/pdf    # Download PDF
```

### Schedule
```http
GET    /api/schedule?date=2025-01-15&laborId={guid}
```

---

## 🧪 Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## 📝 Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=MechanicShopDb;..."
  },
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-chars",
    "Issuer": "MechanicShopApi",
    "Audience": "MechanicShopClient",
    "ExpiryInMinutes": 60
  },
  "WorkOrderSettings": {
    "OpeningTime": "08:00",
    "ClosingTime": "18:00",
    "MinimumDurationMinutes": 15
  },
  "TaxRate": 0.20
}
```

---

## 🎓 Learning Outcomes

This project demonstrates:
- ✅ Clean Architecture with clear layer separation
- ✅ CQRS with MediatR for scalable command/query handling
- ✅ Rich domain models with business logic encapsulation
- ✅ Result pattern for type-safe error handling
- ✅ Entity Framework Core with complex relationships
- ✅ JWT authentication with refresh token rotation
- ✅ FluentValidation for request validation
- ✅ Domain events for decoupled communication
- ✅ Pipeline behaviors (logging, caching, validation)
- ✅ Hybrid caching for performance
- ✅ Pagination and filtering for large datasets

---

## 👤 Author

**Othman Er-Rouydi**
- Email: erothman15er@gmail.com
- Location: Martil, Morocco
- GitHub: [@yourusername](https://github.com/Othman-ER)

---

## 🙏 Acknowledgments

- Training: Mastering ASP.NET Core: Concepts to Production-Ready APIs
- Inspired by real-world workshop management challenges
- Built with modern .NET best practices
