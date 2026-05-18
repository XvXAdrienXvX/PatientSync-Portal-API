# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**PatientSync Portal API** — a .NET 10 Web API serving a patient and doctor portal.
Architecture: **Vertical Slice + Modular Monolith** using MediatR, FluentValidation, EF Core, and Minimal APIs.

---

## Commands

```powershell
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run (development)
dotnet run --project src/PatientSync.API

# Run with hot reload
dotnet watch --project src/PatientSync.API

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~MyTestClassName"

# Run a single test method
dotnet test --filter "FullyQualifiedName=Namespace.Class.MethodName"

# Lint / format
dotnet format

# Apply EF Core migrations (run per module DbContext)
dotnet ef database update --project src/Modules/Patients/Patients.Module --startup-project src/PatientSync.API -- --context PatientsDbContext
dotnet ef database update --project src/Modules/Appointments/Appointments.Module --startup-project src/PatientSync.API -- --context AppointmentsDbContext
dotnet ef database update --project src/Modules/Doctor/Doctor.Module --startup-project src/PatientSync.API -- --context DoctorsDbContext

# Add a new migration (specify the module project and its DbContext)
dotnet ef migrations add <MigrationName> --project src/Modules/Patients/Patients.Module --startup-project src/PatientSync.API -- --context PatientsDbContext
```

---

## Solution Structure

```
PatientSync.API (solution)
│
├── src/
│   │
│   ├── PatientSync.API/                        # Entry point — thin host, wires modules only
│   │   ├── Program.cs                          # DI registration + endpoint mapping per module
│   │   ├── appsettings.json
│   │   └── PatientSync.API.csproj
│   │
│   └── Modules/
│       │
│       ├── Patients/
│       │   ├── Patients.Module/                # Internal module logic (all classes internal)
│       │   │   ├── Features/                   # Vertical slices
│       │   │   │   ├── GetPatient/
│       │   │   │   │   └── GetPatient.cs       # Query + Handler + Endpoint in one file
│       │   │   │   ├── CreatePatient/
│       │   │   │   │   └── CreatePatient.cs    # Command + Handler + Validator + Endpoint
│       │   │   │   ├── GetHealthRecord/
│       │   │   │   │   └── GetHealthRecord.cs
│       │   │   │   └── UpdateProfile/
│       │   │   │       └── UpdateProfile.cs
│       │   │   ├── Data/
│       │   │   │   ├── PatientsDbContext.cs    # internal DbContext
│       │   │   │   └── Migrations/
│       │   │   ├── Domain/
│       │   │   │   ├── Patient.cs              # internal entity
│       │   │   │   ├── Medication.cs           # internal entity
│       │   │   │   └── Allergy.cs              # internal entity
│       │   │   └── PatientsModule.cs           # DI registration + endpoint mapping
│       │   │
│       │   └── Patients.Module.Contracts/      # Public surface only
│       │       ├── IPatientService.cs          # public interface
│       │       ├── PatientDto.cs               # public DTOs
│       │       └── Events/
│       │           └── PatientCreatedEvent.cs  # integration events
│       │
│       ├── Appointments/
│       │   ├── Appointments.Module/
│       │   │   ├── Features/
│       │   │   │   ├── BookAppointment/
│       │   │   │   │   └── BookAppointment.cs
│       │   │   │   ├── GetAvailableSlots/
│       │   │   │   │   └── GetAvailableSlots.cs
│       │   │   │   ├── CancelAppointment/
│       │   │   │   │   └── CancelAppointment.cs
│       │   │   │   └── GetPatientAppointments/
│       │   │   │       └── GetPatientAppointments.cs
│       │   │   ├── Data/
│       │   │   │   └── AppointmentsDbContext.cs
│       │   │   ├── Domain/
│       │   │   │   └── Appointment.cs
│       │   │   └── AppointmentsModule.cs
│       │   │
│       │   └── Appointments.Module.Contracts/
│       │       ├── IAppointmentService.cs
│       │       ├── AppointmentDto.cs
│       │       └── Events/
│       │           └── AppointmentBookedEvent.cs
│       │
│       └── Doctor/
│           ├── Doctor.Module/
│           │   ├── Features/
│           │   │   ├── GetDoctorDashboard/
│           │   │   │   └── GetDoctorDashboard.cs
│           │   │   ├── GetPatientChart/
│           │   │   │   └── GetPatientChart.cs
│           │   │   └── AddVisitNotes/
│           │   │       └── AddVisitNotes.cs
│           │   ├── Data/
│           │   │   └── DoctorsDbContext.cs
│           │   ├── Domain/
│           │   │   └── Doctor.cs
│           │   └── DoctorsModule.cs
│           │
│           └── Doctor.Module.Contracts/
│               ├── IDoctorService.cs
│               └── DoctorDto.cs
│
└── tests/
    ├── PatientSync.Tests.Unit/
    ├── PatientSync.Tests.Integration/
    └── PatientSync.Tests.Architecture/        # NetArchTest boundary enforcement
```

---

## Architecture

### Pattern: Vertical Slice + Modular Monolith

**Macro level:** Modular Monolith — modules are independently bounded (Patients, Appointments, Doctor).
**Micro level:** Vertical Slice Architecture — code is organized by feature, not by layer.

### Core Rules

1. **Modules never reference each other's internal projects.**
   - `Doctor.Module` may reference `Patients.Module.Contracts` — never `Patients.Module` directly.
   - The `internal` keyword enforces this at compile time.

2. **Each module owns its own DbContext and schema.**
   - `PatientsDbContext` lives inside `Patients.Module` (internal).
   - Database schemas are separated: `patients.*`, `appointments.*`, `doctors.*`.

3. **Contracts contain only the public surface.**
   - `*.Contracts` projects contain: service interfaces, DTOs, integration events.
   - `*.Contracts` projects never contain: entities, repositories, DbContexts, business logic.

4. **Program.cs is intentionally thin.**
   - Only registers module services and maps module endpoint groups.
   - Never dispatches commands/queries directly.

5. **Each feature slice is self-contained.**
   - One feature = one folder = one `.cs` file (REPR pattern).
   - Contains: Request, Response, Command or Query, Validator (if write), Handler, Endpoint.

### Request Flow

```
Angular HTTP request
    → Endpoint (inside feature slice)
    → mediator.Send(Command or Query)
    → Handler (same feature folder)
    → DbContext / Contracts service
    → HTTP Response
```

### Feature File Structure (REPR Pattern)

Each feature follows Request → Endpoint → Response:

```csharp
// Modules/Appointments/Appointments.Module/Features/BookAppointment/BookAppointment.cs
public static class BookAppointment
{
    public record Request(...);             // HTTP body from Angular
    public record Response(...);            // HTTP response to Angular
    internal record Command(...) : IRequest<Response>;   // write → Command
    internal class Validator : AbstractValidator<Command> { }
    internal class Handler : IRequestHandler<Command, Response> { }
    public static void MapEndpoint(RouteGroupBuilder group) { }  // POST /api/appointments
}

// Query feature (no Validator needed)
public static class GetPatientAppointments
{
    public record Response(...);
    internal record Query(...) : IRequest<List<Response>>;       // read → Query
    internal class Handler : IRequestHandler<Query, List<Response>> { }
    public static void MapEndpoint(RouteGroupBuilder group) { }  // GET /api/appointments/patient/{id}
}
```

### HTTP Verb → Feature Type

| Verb | Type | Example |
|---|---|---|
| `POST` | Command | `BookAppointment` |
| `PUT` / `PATCH` | Command | `RescheduleAppointment` |
| `DELETE` | Command | `CancelAppointment` |
| `GET` (single/list) | Query | `GetPatientAppointments` |

---

## Key Conventions

- **Record types** for all DTOs, requests, responses, commands, and queries.
- **`internal` by default** for all domain entities, DbContexts, repositories, and handlers.
- **`public` only** for Contracts interfaces, DTOs, integration events, and endpoint registration methods.
- **FluentValidation** for all Command validators (not Query validators — reads don't need them).
- **MediatR** for in-process command/query dispatch inside a feature.
- **Minimal APIs** (`IResult` / `Results<T>`) — no MVC controllers.
- **No generic CRUD** in Contracts interfaces — expose use-case-named methods only.
- **Module registration** via extension methods: `services.AddPatientsModule()` / `app.MapPatientsEndpoints()`.
- **One DbContext per module**, scoped to its own database schema.
- **Cross-module communication** via Contracts interfaces only — never via shared DbContext or direct entity access.

---

## Project References

```
PatientSync.API
    → Patients.Module
    → Patients.Module.Contracts
    → Appointments.Module
    → Appointments.Module.Contracts
    → Doctor.Module
    → Doctor.Module.Contracts

Patients.Module
    → Patients.Module.Contracts

Appointments.Module
    → Appointments.Module.Contracts
    → Patients.Module.Contracts      (cross-module via Contracts only)
    → Doctor.Module.Contracts        (cross-module via Contracts only)

Doctor.Module
    → Doctor.Module.Contracts
    → Patients.Module.Contracts      (cross-module via Contracts only)
    → Appointments.Module.Contracts  (cross-module via Contracts only)

*.Module.Contracts
    → (no module references — only shared primitives if needed)
```

---

## NuGet Packages (per module project)

```xml
<PackageReference Include="MediatR" Version="12.*" />
<PackageReference Include="FluentValidation" Version="11.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.*" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.*" />
```

```xml
<!-- PatientSync.API only -->
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.*" />
<PackageReference Include="Scalar.AspNetCore" Version="2.*" />
```

---

## Adding a New Feature (Checklist)

1. Identify the module it belongs to (Patients / Appointments / Doctor).
2. Create folder: `Modules/<Module>/<Module>.Module/Features/<FeatureName>/`.
3. Create `<FeatureName>.cs` with static class following REPR pattern.
4. Add `Request`, `Response` (public records).
5. Add `Command` or `Query` (internal record implementing `IRequest<T>`).
6. Add `Validator` if it is a Command (FluentValidation).
7. Add `Handler` (internal class implementing `IRequestHandler<,>`).
8. Add `MapEndpoint(RouteGroupBuilder group)` static method.
9. Register endpoint in `<Module>Module.cs` → `MapEndpoint(group)`.
10. If cross-module data is needed, inject Contracts interface — never another module's DbContext.

## Adding a New Module (Checklist)

1. Create folder: `Modules/<ModuleName>/`.
2. Create `<ModuleName>.Module` class library project.
3. Create `<ModuleName>.Module.Contracts` class library project.
4. Add `<ModuleName>Module.cs` with `Add<ModuleName>Module()` and `Map<ModuleName>Endpoints()`.
5. Add `<ModuleName>DbContext.cs` (internal) with module-scoped schema.
6. Add Contracts: `I<ModuleName>Service.cs` + DTOs.
7. Reference `<ModuleName>.Module` and `<ModuleName>.Module.Contracts` from `PatientSync.API`.
8. Register in `Program.cs`.

## Commands

- Discover the commands found in "C:/Dev/PatientSync-Portal-API/.claude/commands"
- Allow use of the commands ".claude/commands"