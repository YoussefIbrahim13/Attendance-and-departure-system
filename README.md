# 🕒 Attendance and Departure System

A full-stack web application to manage employees’ attendance and departure efficiently — perfect for HR teams and business managers. Built using modern .NET technologies and a clean UI experience ✨

## 📚 Table of Contents
- [🔍 Overview](#overview)
- [🏛️ System Architecture](#system-architecture)
- [🚀 Features](#features)
  - [👨‍💼 Employee Management](#employee-management)
  - [📅 Attendance Management](#attendance-management)
  - [🖥️ User Interface](#user-interface)
- [🛠️ Technology Stack](#technology-stack)
- [⚙️ Setup & Installation](#setup--installation)
  - [🗄️ Database Setup](#database-setup)
  - [🔌 Backend API Setup](#backend-api-setup)
  - [🎨 Frontend UI Setup](#frontend-ui-setup)
- [🧭 Usage Guide](#usage-guide)
  - [🧩 Navigating the UI](#navigating-the-ui)
  - [👥 Managing Employees](#managing-employees)
  - [📊 Managing Attendance](#managing-attendance)
- [📁 Folder Structure](#folder-structure)
- [📡 API Endpoints](#api-endpoints)


## 🔍 Overview
**Attendance and Departure System** is a modern web-based HR solution for managing employees and tracking their daily presence. It supports employee CRUD operations, CSV-based attendance importing, and dynamic daily/monthly views for analysis and monitoring.

## 🏛️ System Architecture
- **Frontend:** Blazor WebAssembly (Single Page Application)
- **Backend:** ASP.NET Core Web API (RESTful)
- **Database:** SQL Server
- **ORM:** Entity Framework Core

## 🚀 Features

### 👨‍💼 Employee Management
- **Add:** Create new employee profiles with ID, Name, Department, and Role.
- **Edit:** Modify employee details using a modal form.
- **Delete:** Remove employees from the system (permanent deletion).
- **View:** Display employee list with search and sort functionality.

### 📅 Attendance Management
- **CSV Import:** Upload attendance files containing check-in/check-out times.
- **Daily View:** See attendance status and working hours per employee for a specific day.
- **Monthly View:** Summarize attendance statistics (presents/absents) for a given month.
- **Time Records:** Calculate working hours and store optional notes.

### 🖥️ User Interface
- Sidebar navigation for quick page access
- Responsive layout (desktop and mobile friendly)
- Notification system for success and error feedback

## 🛠️ Technology Stack
- ASP.NET Core Blazor
- ASP.NET Core Web API
- SQL Server
- Entity Framework Core
- Bootstrap (styling)
- CSV Parsing Logic

## ⚙️ Setup & Installation

### 🗄️ Database Setup
1. Create a new SQL Server database.
2. Run EF Core migrations or use SQL scripts to initialize the following tables:
   - `Employees`
   - `AttendanceRecords`

### 🔌 Backend API Setup
1. Navigate to the API project directory:
   ```sh
   dotnet run --project AttendanceSystem.ImportFile.API
Make sure the following endpoints are working:

GET /Attendance/get-all-employees

POST /Attendance/add-employee

PUT /Attendance/update-employee

DELETE /Attendance/delete-employee/{id}

POST /Attendance/import-attendance

GET /Attendance/day-view

GET /Attendance/month-view

🎨 Frontend UI Setup
Run the Blazor UI project:

sh
Copy
Edit
dotnet run --project AttendanceSystem.ImportFile.ui/AttendanceSystem.ImportFile.ui.csproj
Visit http://localhost:5000 in your browser.

🧭 Usage Guide
🧩 Navigating the UI
Use the sidebar to switch between:

Day View: Monitor daily attendance.

Month View: View monthly summaries.

Import: Upload attendance CSV files.

Employees: Manage employee records.

👥 Managing Employees
Add: Fill out the employee form and click "Add Employee".

Edit: Click "Edit", modify details, and save.

Delete: Remove an employee by clicking "Delete".

📊 Managing Attendance
Import: Use the import page to upload a valid CSV file.

View: Browse attendance stats via Day/Month views.

📁 Folder Structure
Copy
Edit
📦 AttendanceSystem
├── 📂 AttendanceSystem.ImportFile.API
│   ├── Controllers/
│   ├── Models/
│   └── DataContext.cs
│
├── 📂 AttendanceSystem.ImportFile.ui
│   ├── Pages/
│   │   └── Employees.razor
│   └── Services/
│       └── AttendanceService.cs
📡 API Endpoints
Employee Management
GET /Attendance/get-all-employees

POST /Attendance/add-employee

PUT /Attendance/update-employee

DELETE /Attendance/delete-employee/{id}

Attendance Management
POST /Attendance/import-attendance

GET /Attendance/day-view?date=yyyy-MM-dd

GET /Attendance/month-view?year=yyyy&month=MM
