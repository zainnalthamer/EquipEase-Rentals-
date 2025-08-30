# EquipEase Rentals
An Equipment Rental Management System built as part of the BICT IT8118 Advanced Programming course project. The system helps rental companies manage their inventory, process rental requests, and track rentals/returns.
It consists of two applications sharing the same database and Entity Framework Core models:
- **Rental (ASP.NET Core MVC Web App)**: Customer-facing + Staff portal.
- **FormApp (Windows Forms App)**: Admin/Manager desktop tool for day-to-day operations.

## Project Overview
The goal of this project is to create a multi-tier system where customers and staff can manage the entire rental lifecycle:
- Customers can browse equipment, request rentals, track orders, and return items.
- Staff can approve/reject requests, manage inventory, handle returns, monitor overdue rentals, and generate reports.
Both apps share a Microsoft SQL Server database and a C# Class Library (EF Core) that handles the data access layer.

![EquipEase - Landing Page](https://res.cloudinary.com/dvhwvkip4/image/upload/v1756543195/Screenshot_2025-08-30_111741_p1x1wy.png)

## Applications
### 1. Rental - ASP.NET Core MVC Web Application
Main web interface for customers, staff, and administrators.

#### Features
**1. Authentication & Authorization**: Registration, login, role-based access (Customer, Manager, Admin).

**2. Categories & Equipment Management**: Create, update, delete, and search inventory.

**3. Rental Requests**: Customers submit requests; managers approve/reject.

**4. Rental Transactions**: Track rentals, fees, deposits, and documents.

**5. Returns**: Manage returned equipment, track conditions, and late fees.

**6. Feedback & Notifications**: Customers leave feedback; system generates status updates.

**7. Dashboards & Reports**: Admin/managers see KPIs (pending vs. completed, overdue, damaged items, financials).

### 2. FormApp - Windows Forms Application
Desktop application mainly for managers and administrators to speed up daily operations.

#### Features
**1. Login**: Using the same credentials as the web app.

**2. Quick View of Rental Requests**: Search, filter, update status.

**3. Equipment Checkout / Transactions**: Approve rentals, manage ongoing ones.

**4. Equipment Check-in / Returns**: Record returned equipment, update conditions.

**5. Equipment Information**: Search, update status (available, rented, under maintenance).

**6. Dashboards**: Real-time summaries of rentals, overdue items, inventory stats.

**7. Logs & Audit Trails**: Exceptions and key actions logged for admin review.

## Technologies Used
### Languages & Frameworks
![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET 6](https://img.shields.io/badge/.NET%206+-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core%20MVC-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows Forms](https://img.shields.io/badge/WinForms-0078D6?style=for-the-badge&logo=windows&logoColor=white)

### Database & ORM
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

### Tools
![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio%202022-5C2D91?style=for-the-badge&logo=visualstudio&logoColor=white)
![SSMS](https://img.shields.io/badge/SQL%20Server%20Management%20Studio-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)
