# 🏗 Application Architecture

## Layers

- **Web Layer** (`EfficiencyTrack`)  
  ASP.NET Core MVC controllers and views. Handles routing, user interactions, and authorization.

- **Service Layer** (`EfficiencyTrack.Services`)  
  Contains business logic and calculations (e.g., efficiency logic).

- **Data Layer** (`EfficiencyTrack.Data`)  
  Contains DbContext, EF Core migrations, and database models.

## Key Components

- `DailyWorkerEfficiencyCheckService` – checks missing records and sends daily emails.
- `EntryService` – calculates operation and daily effectiveness.
- Role-based access using `Authorize` and service-level validation.
