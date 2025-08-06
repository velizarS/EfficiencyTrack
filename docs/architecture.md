## 🧱 Project Architecture Overview

This document describes the architecture of the **EfficiencyTrack** application. 
It follows a layered structure separating concerns clearly across different class libraries and responsibilities.

---

### 🔷 1. `Common` (Class Library)

Contains application-wide constants and utilities.

* `EfficiencyAppConstants.cs`

---

### 🔷 2. `EfficiencyTrack.Data` (Class Library)

Handles all persistence and data modeling logic.

#### 📁 Configuration

* Entity Framework model configuration for fluent API.

#### 📁 Data

* `ApplicationDbContext` setup
* EF Core Migrations

#### 📁 Identity

* `ApplicationUser`: custom identity user class

#### 📁 Models

Domain models:

* `BaseEntity`
* `Shift`
* `SentEfficiencyReports`
* `Routing`
* `Feedback`
* `Entry`
* `Employee`
* `EmailSettings`
* `Department`
* `DailyEfficiency`

---

### 🔷 3. `EfficiencyTrack.Services` (Class Library)

Contains business logic and service layer abstraction.

#### 📁 DTOs

* `DailyEfficiencyDTO`
* `EntryDTO`
* `TopEfficiencyDTO`

#### 📁 Helpers

* `DailyWorkerEfficiencyCheckService`
* `DuplicateDepartmentException`
* `GreetingsService`
* `SmtpEmailSender`
* `ValidationResult`

#### 📁 Interfaces

Defines service contracts:

* `ICrudService`
* `IDailyEfficiencyService`
* `IDepartmentService`
* `IEmailService`
* `IEmployeeService`
* `IEntryService`
* `IGreetingService`
* `IRoutingService`

#### 📁 Implementation

Concrete implementations:

* `CrudService`
* `DailyEfficiencyService`
* `DepartmentService`
* `EmployeeService`
* `EntryService`
* `FeedbackService`
* `RoutingService`

---

### 🔷 4. `EfficiencyTrack.Tests` (Test Project)

Contains unit tests for services and helpers.

#### 📁 ServicesTests

* **HelperTests**

  * `DailyWorkerEfficiencyCheckServiceTests`
  * `EntryValidatorTests`
  * `GreetingsServiceTests`

* **MainServiceTests**

  * `DailyEfficiencyServiceTests`
  * `DepartmentServiceTests`
  * `EmployeeServiceTests`
  * `FeedbackServiceTests`
  * `RoutingServiceTests`

---

### 🔷 5. `EfficiencyTrack` (Main ASP.NET Core Web App)

#### 📁 Properties & Static Files

* `launchSettings.json`, `wwwroot/`

#### 📁 Areas

Optional structure for admin or identity areas (if used).

#### 📁 Controllers

* `AdminController`
* `BaseCrudController`
* `DailyEfficiencyController`
* `DepartmentController`
* `EmployeeController`
* `EntryController`
* `FeedbackController`
* `HomeController`
* `RoutingController`
* `ShiftController`

#### 📁 ViewModels

Organized by feature:

* `AdminViewModels`

  * `UserRoleViewModel`

* `DailyEfficiencyViewModels`

  * `DailyBaseViewModel`
  * `DailyDetailViewModel`
  * `DailyEfficiencyViewModel`
  * `DailyEfficiencyListViewModel`

* `DepartmentViewModels`, `EmployeeViewModels`, `EntryViewModels`, `FeedbackViewModels`, `RoutingViewModels`, `ShiftViewModels`

  * Each includes: `Create`, `Edit`, `Detail`, `List`, `View`, `Base` models

#### 📁 Views


---

