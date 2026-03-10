# <div align="center">🔧 MechanicShop</div>

<div align="center">

![.NET 10](https://img.shields.io/badge/.NET-10-512bd4?style=for-the-badge&logo=dotnet)
![C# 14](https://img.shields.io/badge/C%23-14-239120?style=for-the-badge&logo=csharp)
![Blazor](https://img.shields.io/badge/Blazor-WASM-512bd4?style=for-the-badge&logo=blazor)
![Fedora](https://img.shields.io/badge/Fedora-Linux-2f427e?style=for-the-badge&logo=fedora)

**A modern Automotive Workshop Management System built with Vertical Slice Architecture.**
</div>

---

## 📖 Overview
I built **MechanicShop** to solve real-world operational challenges in automotive workshops. Drawing from my experience in the service industry, I focused on building a system that ensures scheduling accuracy and financial integrity. 

This project is a deep dive into **.NET 10** and **Vertical Slice Architecture**, moving away from traditional layered designs to achieve high cohesion and maintainability.

---

## 🚀 Business Logic & Features

### 1. Smart Workshop Scheduling
The core engine manages 4 dedicated workshop spots (A, B, C, D) using 15-minute intervals. 
* **Conflict Prevention:** The domain logic prevents double-booking vehicles, mechanics, or workshop spots.
* **Operating Hours:** Built-in validation for workshop opening and closing times.

### 2. Work Order State Machine
Every repair follows a strict lifecycle to ensure data consistency:
`Scheduled` ➡️ `In Progress` ➡️ `Completed`

### 3. Automated Billing
Invoices are automatically generated only when a Work Order is marked as `Completed`. 
* **Calculation Logic:** Subtotal = (Labor duration × Rate) + Spare parts cost.
* **Tax & Discounts:** Configurable tax rates (e.g., 20%) and manual discount application.

---

## 🏗️ Architecture & Patterns

This system is built using **Vertical Slice Architecture (VSA)**. Each feature (Slice) is self-contained, making the codebase easy to navigate and scale.

* **CQRS with MediatR:** Clear separation between Commands (Writes) and Queries (Reads).
* **Domain-Driven Design (DDD):** Use of Aggregates and Value Objects to protect business rules.
* **Result Pattern:** Explicit error handling instead of using exceptions for flow control.
* **FluentValidation:** Automated request validation via MediatR pipeline behaviors.

---

## 🛠️ Technical Stack
* **Backend:** ASP.NET Core API (.NET 10).
* **Frontend:** Blazor WebAssembly.
* **Database:** SQL Server with Entity Framework Core 10.
* **Caching:** HybridCache for optimized performance.
* **Dev Env:** Developed on **Fedora Linux** using **VS Code**.

---

## 📁 Project Structure
```text
src/
├── MechanicShop.Api/           # API Endpoints & Configuration
├── MechanicShop.Application/   # Vertical Slices (Features)
│   ├── WorkOrders/             # Commands, Queries, and Logic
│   ├── Customers/              # Customer & Vehicle management
│   └── ... 
├── MechanicShop.Domain/        # Aggregates, Entities, and Invariants
└── MechanicShop.Infrastructure/# Database, Identity, and PDF Services.

---

## About me
Othman Er-Rouydy

Email: othman.errouydy@gmail.com
Location: Martil, Morocco
GitHub: @Othman-ER