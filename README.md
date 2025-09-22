

<h1 align="center">📊 Attendance & Departure System</h1>
<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-blue" alt=".NET Version">
  <img src="https://img.shields.io/badge/Blazor-WebAssembly-purple" alt="Blazor">
 <img src="https://img.shields.io/badge/SQL_Server-2019+-CC2927" alt="SQL Server">
  
</p>



![Landing Page](https://github.com/ehabmosalah/DS-Project/blob/main/Attendance_Project/Website.png)
A full-stack web application to manage employees’ attendance and departure efficiently — perfect for HR teams and business managers. Built using modern .NET technologies and a clean UI experience ✨

---

## 📖 The Story Behind the Project

This project was created to address the common challenges faced by HR departments in accurately tracking employee attendance and managing leave requests.

The goal: **build a robust, secure, and user-friendly system** that automates manual processes, giving HR teams and managers the tools they need to monitor and analyze employee presence data effectively.

---

## 📚 Table of Contents

* [🔍 Overview](#overview)
* [🏛️ System Architecture](#system-architecture)
* [🚀 Features](#features)

  * [🔐 Authentication & Authorization](#authentication--authorization)
  * [👨‍💼 Employee Management](#employee-management)
  * [📅 Attendance Management](#attendance-management)
  * [🌴 Vacation Request Management](#vacation-request-management)
  * [🖥️ User Interface](#user-interface)
* [🛠️ Technology Stack](#technology-stack)
* [⚙️ Setup & Installation](#setup--installation)
* [📁 Folder Structure](#folder-structure)
* [📸 Screenshots](#screenshots)
* [📊 Views](#views)
* [🚀 Potential Improvements](#potential-improvements)
* [📜 License](#license)

---
<a id="overview"></a>
## 🔍 Overview

**Attendance and Departure System** is a modern HR solution for managing employees and tracking daily presence.

✨ Highlights:

* Employee CRUD operations
* CSV-based attendance importing
* Daily, Monthly, and Yearly views for monitoring
* Role-based authentication and authorization

---
 <a id="system-architecture"></a>
## 🏛️ System Architecture

* **Frontend:** Blazor WebAssembly (SPA)
* **Backend:** ASP.NET Core Web API with MediatR + CQRS
* **Database:** SQL Server
* **ORM:** Entity Framework Core
* **Authentication:** JWT (JSON Web Tokens)

---
<a id="features"></a>
## 🚀 Features
<a id="authentication--authorization"></a>
### 🔐 Authentication & Authorization

* User registration & secure JWT login
* Role-based access (Admin, Employee)
* Password reset & admin-controlled password management
<a id="employee-management"></a>
### 👨‍💼 Employee Management

* Full CRUD on employee profiles
* Admin user account approvals & unlocks
* Searchable, sortable employee lists
<a id="attendance-management"></a>
### 📅 Attendance Management

* CSV-based bulk attendance import
* Daily, Monthly, Yearly views
* Automatic working hours calculation
<a id="vacation-request-management"></a>
### 🌴 Vacation Request Management

* Employee request submission
* Admin approval/rejection workflow
* Request status tracking
<a id="user-interface"></a>
### 🖥️ User Interface

* Responsive design (desktop & mobile)
* Sidebar navigation
* Real-time notifications

---
<a id="technology-stack"></a>
## 🛠️ Technology Stack

* ASP.NET Core Blazor
* ASP.NET Core Web API
* SQL Server + Entity Framework Core
* MediatR + AutoMapper
* Blazored.LocalStorage
* Bootstrap

---
<a id="setup--installation"></a>
## ⚙️ Setup & Installation

1. **Clone repository**

   ```sh
   git clone https://github.com/YoussefIbrahim13/Attendance-and-departure-system.git
   cd Attendance-and-departure-system
   git checkout semi-Merge
   ```

2. **Database Setup**

   * Create a SQL Server database
   * Update `appsettings.json` connection string in `AttendanceSystem.Auth.API` & `AttendanceSystem.ImportFile.API`
   * Run EF Core migrations

3. **Backend Setup**

   * Open solution in Visual Studio / Rider
   * Build solution (NuGet restore)
   * Run `EmployeeAttendanceSolution.AppHost`

4. **Frontend Setup**

   * Blazor UI is served via backend
   * Access via the provided host URL

---
<a id="folder-structure"></a>
## 📁 Folder Structure

```
📦 EmployeeAttendanceSolution
├── 📂 Applications (CQRS Features)
├── 📂 AttendanceSystem.Auth.API
├── 📂 AttendanceSystem.Auth.Services
├── 📂 AttendanceSystem.ImportFile.API
├── 📂 AttendanceSystem.ImportFile.ui
├── 📂 Domain
├── 📂 Employee.Shared
├── 📂 EmployeeAttendanceSolution.AppHost
├── 📂 EmployeeAttendanceSolution.ServiceDefaults
└── 📂 Infrastructure
```

---
<a id="screenshots"></a>
## 📸 Screenshots

### 🔑 System Overview & Authentication

| Landing Page                                                                                       | Login Interface                                                                           |
| -------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------- |
| ![Landing Page](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Website.png) | ![Login](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Login.png) |

---

### 🛠️ Administration & Management

| User Management                                                                                                | Employee Management                                                                                                  |
| -------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------- |
| ![User Management](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Admin_Mnnagement.png) | ![Employee Management](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Employee_Managment.png) |

---

### ⏱️ Attendance Tracking

| Calendar Views                                                                                             | Year View                                                                                    | Month View                                                                                     |
| ---------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| ![Calendar Views](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Calendr_views.png) | ![Year](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Year_view.png) | ![Month](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Month_view.png) |

| Day View                                                                                   | Attendance Import                                                                                           | Attendance Records                                                                                    |
| ------------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| ![Day](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Day_view.png) | ![Import](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Import_Attendance_file.png) | ![Records](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Attendance_File.png) |

| Attendance Planner                                                                                       |
| -------------------------------------------------------------------------------------------------------- |
| ![Planner](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Attendance_Planner.png) |

---

### 🌙 Dark Mode

![Dark Mode](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Dark_Mode.png)

---

### 🌴 Vacation Management

| Vacation Requests                                                                                                | Admin Vacation Management                                                                                          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------ |
| ![Vacation Requests](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/Vacation_Request.png) | ![Admin Vacation](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/All_Vacation_Requests.png) |

---

### 👤 User Profile

![User Profile](https://github.com/ehabmosalah/DS-Project/raw/main/Attendance_Project/User_Profile.png)

---
<a id="views"></a>
## 📊 Views

* **Daily View** – Check-ins, check-outs, hours worked, punctuality status
* **Monthly View** – Summarized attendance trends per employee
* **Yearly View** – Long-term overview and trend analysis
* **Employee Management View** – Full CRUD on employees with search/sort
* **Vacation Request View** – Employee submission & admin approval workflow
* **User Management View** – Approvals, role changes, and account unlocks

---
<a id="potential-improvements"></a>
## 🚀 Potential Improvements

### Short-Term Goals

* Real-time tracking (face recognition/geofencing)
* Enhanced notifications (email + in-app)
* Audit trails for critical actions

### Long-Term Goals

* Advanced analytics & reporting
* Customizable leave policies
* Mobile companion app
* Payroll system integration
* Multi-language support

---
<a id="license"></a>
## 📜 License

Licensed under the **MIT License**. See `LICENSE` for details.


---
<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET Version">
  <img src="https://img.shields.io/badge/Blazor-WebAssembly-512BD4?style=for-the-badge&logo=blazor&logoColor=white" alt="Blazor">
  <img src="https://img.shields.io/badge/SQL_Server-2019+-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/EF_Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core">
  <br>
  <img src="https://img.shields.io/badge/Architecture-CQRS-0078D4?style=for-the-badge&logo=azure-devops&logoColor=white" alt="CQRS Architecture">
  <img src="https://img.shields.io/badge/API-REST-FF6B6B?style=for-the-badge&logo=postman&logoColor=white" alt="REST API">
  <img src="https://img.shields.io/badge/Auth-JWT-000000?style=for-the-badge&logo=json-web-tokens&logoColor=white" alt="JWT Auth">
  <br>
  <img src="https://img.shields.io/github/license/YoussefIbrahim13/Attendance-and-departure-system?style=for-the-badge&color=green" alt="License">
  <img src="https://img.shields.io/github/stars/YoussefIbrahim13/Attendance-and-departure-system?style=for-the-badge&color=yellow" alt="Stars">
  <img src="https://img.shields.io/github/forks/YoussefIbrahim13/Attendance-and-departure-system?style=for-the-badge&color=blue" alt="Forks">
</p>

 
