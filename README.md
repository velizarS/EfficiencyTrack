# EfficiencyTrack

A modern and responsive web application for tracking worker productivity in a manufacturing environment.

## 💡 Project Idea

**EfficiencyTrack** is designed to digitize and simplify the process of tracking worker performance in real-time. Each worker submits their daily activity through a form, and shift leaders or managers can monitor, analyze, and manage performance efficiently.

The application provides a full admin panel, access control by roles, automated calculations for worker efficiency, and real-time overviews of daily and monthly top performers.

## 🚀 Key Features

### 👷 Worker Submissions
Workers fill in a form with:
- Unique worker code
- Operation code
- Shift
- Number of produced items
- Defects (if any)
- Time worked (in minutes)

### 🧱 Entity Management
Before submissions are possible, the following data must be created:
- **Workers** – with a unique code, name, assigned department, and shift leader
- **Departments** – used to group workers and operations
- **Routings** – operations with a code, description, fixed zone, time per unit (in minutes), and associated department

### 📊 Automatic Calculations
- Operation effectiveness per submission
- Daily effectiveness per worker (aggregated from all entries that day)
- **Top 10 workers of the day** (only 100% or higher)
- **Top 10 workers of the month**

### 🔐 Roles & Access
| Role            | Access |
|------------------|--------|
| `Admin`          | Full access to everything |
| `Manager`        | Manage users and data |
| `UnitResponsible`| View department data |
| `ShiftLeader`    | View/edit data of their assigned workers |
| `User` (Worker)  | Submit entries, view own effectiveness and history |

### ✉️ Email Notifications
Every morning, the system sends an email to each **Shift Leader** listing **all workers without a submission for the previous day**. This encourages accountability and daily reporting.

### 💻 UI & UX
- Responsive layout for both desktop and mobile
- Dark/light theme toggle
- Clean and modern interface using Bootstrap 5

## ⚙️ Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- Microsoft Identity (Role-based access)
- AutoMapper
- LINQ
- Bootstrap 5
- Microsoft Azure (Web App Hosting)

## 🌐 Live Demo

👉 [Try the live app on Azure](https://efficiencytrack.azurewebsites.net/)

> Some features require authentication and role-based access.

## 🛠️ Getting Started

To run locally:

1. Clone the repository:
```bash
git clone https://github.com/velizarS/efficiencytrack.git
