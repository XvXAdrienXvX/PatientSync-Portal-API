# Skip DTOs: Use CQRS Commands/Queries in Contracts

## The Question

**Instead of:**
```csharp
// Contracts
public record PatientDto(Guid Id, string FirstName, string LastName, string Email);
public interface IPatientService
{
    Task<PatientDto> GetPatientAsync(Guid patientId);
}

// Features
public record GetPatientQuery(Guid PatientId) : IRequest<PatientDto>;
```

**Can we do this:**
```csharp
// Contracts
public record GetPatientQuery(Guid PatientId) : IRequest<PatientDto>;

// Features use the same query from Contracts
```

## The Direct Answer

**Yes. Completely valid. Simpler in many ways.**

---

## Approach 1: Traditional (DTOs + Service Interface)

```csharp
// Contracts (public API)
public record PatientDto(Guid Id, string FirstName, string LastName);
public interface IPatientService
{
    Task<PatientDto> GetPatientAsync(Guid patientId);
}

// Features (internal)
public record GetPatientQuery(Guid PatientId) : IRequest<PatientDto>;
```

**Pros:**
- ✅ Clear separation: "What the module can do" (interface) vs "How it does it" (query)
- ✅ Can refactor queries without changing contracts
- ✅ Interface is stable even if implementation changes
- ✅ Decouples API from internal CQRS

**Cons:**
- ❌ Extra layer: query → handler → DTO conversion
- ❌ More files to maintain (interface + query + DTO)
- ❌ For simple reads, feels over-engineered

---

## Approach 2: Simplified (CQRS Queries/Commands in Contracts)

```csharp
// Contracts (public API)
public record GetPatientQuery(Guid PatientId) : IRequest<PatientDto>;
public record PatientDto(Guid Id, string FirstName, string LastName);

// Features use the same query from Contracts
public class GetPatientQueryHandler : IRequestHandler<GetPatientQuery, PatientDto> { }
```

**Pros:**
- ✅ No service interfaces needed
- ✅ No DTO-to-response conversion
- ✅ Simpler code, less boilerplate
- ✅ Direct: API dispatches query → handler returns DTO
- ✅ Clear: contracts show exactly what queries/commands exist
- ✅ Easier to trace: feature name matches contract name

**Cons:**
- ❌ Tighter coupling: contracts expose internal CQRS structure
- ❌ Harder to refactor queries (external code references them)
- ❌ Less flexibility for future changes

---

## Honest Assessment for Your MVP

**Use Approach 2 (CQRS in Contracts).**

Here's why:
- Your MVP is simple CRUD with no complex service orchestration
- You don't need "business facade" layer (that's what `IPatientService` provides)
- Direct query dispatch from API is fine
- Refactoring cost is low (just rename query in two places)
- Less code = fewer bugs = faster development

**You can migrate to Approach 1 later if:**
- Multiple queries return the same DTO (need a facade)
- Business logic spans queries (need service orchestration)
- You need to hide internal CQRS from external callers
- You're building an API that external teams consume

---

## Architecture: Approach 2 (Recommended for MVP)

```
Angular HTTP Request
    ↓
API Endpoint
    ↓
mediator.Send(Query from Contracts)  ← Query is public, in .Contracts
    ↓
Handler (in Features)
    ↓
returns DTO (from Contracts)  ← DTO is also in .Contracts
    ↓
HTTP Response
```

### Folder Structure

```
Modules/Patients/
├── Patients.Module.Contracts/           ← PUBLIC surface
│   ├── Queries/
│   │   ├── GetPatient.cs               ← Query + DTO together
│   │   ├── GetPatientChart.cs
│   │   └── GetHealthRecord.cs
│   ├── Commands/
│   │   ├── CreatePatient.cs            ← Command + Request DTO
│   │   ├── UpdateProfile.cs
│   │   └── AssignDoctor.cs
│   └── Events/
│       └── PatientCreatedEvent.cs
│
└── Patients.Module/                    ← INTERNAL implementation
    ├── Features/
    │   ├── GetPatient/
    │   │   └── GetPatient.cs           ← Handler only (imports from Contracts)
    │   ├── CreatePatient/
    │   │   └── CreatePatient.cs        ← Handler + Validator
    │   └── GetPatientChart/
    │       └── GetPatientChart.cs
    ├── Domain/
    │   └── Patient.cs
    ├── Infrastructure/
    │   ├── UserRepository.cs
    │   └── PatientMapper.cs
    └── PatientsModule.cs
```

---

## Complete Example: GetPatient Feature

### 1. Contracts (Public)

```csharp
// Modules/Patients/Patients.Module.Contracts/Queries/GetPatient.cs
namespace PatientSync.Modules.Patients.Contracts.Queries;

public record GetPatientQuery(Guid PatientId) : IRequest<GetPatientResponse>;

public record GetPatientResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string[] Allergies
);
```

That's it. One file. Query + DTO together.

### 2. Feature Handler (Internal)

```csharp
// Modules/Patients/Patients.Module/Features/GetPatient/GetPatient.cs
namespace PatientSync.Modules.Patients.Features.GetPatient;

using PatientSync.Modules.Patients.Contracts.Queries;
using PatientSync.Modules.Patients.Domain;
using PatientSync.Modules.Patients.Infrastructure.Persistence;

public static class GetPatient
{
    public static void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("/{patientId:guid}", Handle)
            .WithName("GetPatient")
            .WithOpenApi()
            .Produces<GetPatientResponse>();
    }

    private static async Task<IResult> Handle(
        Guid patientId,
        IMediator mediator,
        CancellationToken ct)
    {
        var query = new GetPatientQuery(patientId);
        var result = await mediator.Send(query, ct);
        return Results.Ok(result);
    }

    // ─── Internal CQRS ────────────────────────────────────────────────────

    private class Handler : IRequestHandler<GetPatientQuery, GetPatientResponse>
    {
        private readonly IPatientRepository _repository;

        public Handler(IPatientRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPatientResponse> Handle(
            GetPatientQuery query,
            CancellationToken ct)
        {
            var patient = await _repository.GetByIdAsync(query.PatientId, ct);
            if (patient is null)
                throw new NotFoundException(nameof(Patient), query.PatientId);

            var allergies = await _repository.GetAllergiesAsync(patient.Id, ct);

            return new GetPatientResponse(
                patient.Id,
                patient.FirstName,
                patient.LastName,
                patient.Email,
                allergies.Select(a => $"{a.Substance} ({a.Severity})").ToArray()
            );
        }
    }
}
```

### 3. Cross-Module Communication (Doctors Calls Patients)

```csharp
// Modules/Doctors/Doctor.Module/Features/GetPatientChart/GetPatientChart.cs
using PatientSync.Modules.Patients.Contracts.Queries;  // ← Import from Contracts

public class Handler : IRequestHandler<GetPatientChartQuery, GetPatientChartResponse>
{
    private readonly IMediator _mediator;

    public async Task<GetPatientChartResponse> Handle(
        GetPatientChartQuery query,
        CancellationToken ct)
    {
        // Call Patients module query directly via mediator
        var patientResult = await _mediator.Send(
            new GetPatientQuery(query.PatientId),
            ct
        );

        // Use the DTO directly
        return new GetPatientChartResponse(
            patientResult.Id,
            $"{patientResult.FirstName} {patientResult.LastName}",
            patientResult.Allergies
        );
    }
}
```

No `IPatientService` interface needed. Just import the query from `Contracts` and send it.

---

## Complete Example: CreatePatient Feature

### 1. Contracts (Public)

```csharp
// Modules/Patients/Patients.Module.Contracts/Commands/CreatePatient.cs
namespace PatientSync.Modules.Patients.Contracts.Commands;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone
) : IRequest<CreatePatientResponse>;

public record CreatePatientResponse(
    Guid PatientId,
    string FirstName,
    string LastName,
    string Email
);
```

### 2. Feature Handler (Internal)

```csharp
// Modules/Patients/Patients.Module/Features/CreatePatient/CreatePatient.cs
namespace PatientSync.Modules.Patients.Features.CreatePatient;

using PatientSync.Modules.Patients.Contracts.Commands;
using PatientSync.Modules.Patients.Domain;

public static class CreatePatient
{
    public static void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithName("CreatePatient")
            .WithOpenApi()
            .Produces<CreatePatientResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Handle(
        CreatePatientRequest request,
        IValidator<CreatePatientRequest> validator,
        IMediator mediator,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        var command = new CreatePatientCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone
        );

        var result = await mediator.Send(command, ct);
        return Results.Created($"/api/patients/{result.PatientId}", result);
    }

    // ─── HTTP Request DTO (local, only for HTTP binding) ───────────────────

    private record CreatePatientRequest(
        string FirstName,
        string LastName,
        string Email,
        string Phone
    );

    // ─── Validator ─────────────────────────────────────────────────────────

    private class CreatePatientValidator : AbstractValidator<CreatePatientRequest>
    {
        public CreatePatientValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?1?\d{9,15}$");
        }
    }

    // ─── Handler ───────────────────────────────────────────────────────────

    private class Handler : IRequestHandler<CreatePatientCommand, CreatePatientResponse>
    {
        private readonly IPatientRepository _repository;

        public Handler(IPatientRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreatePatientResponse> Handle(
            CreatePatientCommand command,
            CancellationToken ct)
        {
            var existingPatient = await _repository.GetByEmailAsync(command.Email, ct);
            if (existingPatient is not null)
                throw new ConflictException($"Email {command.Email} already exists");

            var patient = Patient.Create(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Phone
            );

            await _repository.AddAsync(patient, ct);

            return new CreatePatientResponse(
                patient.Id,
                patient.FirstName,
                patient.LastName,
                patient.Email
            );
        }
    }
}
```

---

## Contract Organization: Simple Structure

```csharp
// Modules/Patients/Patients.Module.Contracts/Queries/GetPatient.cs
public record GetPatientQuery(Guid PatientId) : IRequest<GetPatientResponse>;
public record GetPatientResponse(Guid Id, string FirstName, string LastName, string Email);

// Modules/Patients/Patients.Module.Contracts/Queries/GetPatientChart.cs
public record GetPatientChartQuery(Guid PatientId) : IRequest<GetPatientChartResponse>;
public record GetPatientChartResponse(Guid Id, string FullName, string[] Allergies, string[] Medications);

// Modules/Patients/Patients.Module.Contracts/Commands/CreatePatient.cs
public record CreatePatientCommand(string FirstName, string LastName, string Email) : IRequest<CreatePatientResponse>;
public record CreatePatientResponse(Guid PatientId, string Email);

// Modules/Patients/Patients.Module.Contracts/Commands/AssignDoctor.cs
public record AssignDoctorCommand(Guid PatientId, Guid DoctorId) : IRequest<Unit>;
```

**One file per operation.** Query/command + response together.

---

## API Entry Point (Program.cs)

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register all MediatR handlers across all modules
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssemblies(
        typeof(PatientSync.Modules.Patients.PatientsModule).Assembly,
        typeof(PatientSync.Modules.Appointments.AppointmentsModule).Assembly,
        typeof(PatientSync.Modules.Doctor.DoctorsModule).Assembly,
        typeof(PatientSync.Modules.Messaging.MessagingModule).Assembly
    );
});

// Register modules
builder.Services
    .AddPatientsModule(builder.Configuration)
    .AddAppointmentsModule(builder.Configuration)
    .AddDoctorsModule(builder.Configuration)
    .AddMessagingModule(builder.Configuration);

var app = builder.Build();

// Map endpoints
app.MapPatientsEndpoints();
app.MapAppointmentsEndpoints();
app.MapDoctorsEndpoints();
app.MapMessagingEndpoints();

app.Run();
```

API entry point never directly references queries/commands. It just calls `app.MapModuleEndpoints()`.

---

## Benefits of Simplified Approach (CQRS in Contracts)

| Aspect | Traditional (DTO + Service) | Simplified (CQRS in Contracts) |
|---|---|---|
| **Files per feature** | 4+ (interface, DTO, query, handler) | 2 (contracts, handler) |
| **Contract clarity** | "What module can do" (interface) | "Exact queries available" (command/query) |
| **Cross-module calls** | `await patientService.GetPatientAsync()` | `await mediator.Send(new GetPatientQuery())` |
| **Refactoring queries** | Easy (interface hides it) | Harder (external code references query) |
| **Boilerplate** | High (service wrapper) | Low (direct dispatch) |
| **MVP fit** | Overkill | Perfect |
| **Can migrate later** | N/A | Yes, add service facade when needed |

---

## When to Keep Service Interfaces (Approach 1)

Keep `IPatientService` if you need:

1. **Business orchestration** across queries
   ```csharp
   public interface IPatientService
   {
       Task<CompletePatientProfileDto> GetCompleteProfileAsync(Guid patientId)
       {
           var patient = await mediator.Send(new GetPatientQuery(patientId));
           var appointments = await mediator.Send(new GetPatientAppointmentsQuery(patientId));
           var medications = await mediator.Send(new GetPatientMedicationsQuery(patientId));
           
           return new CompletePatientProfileDto(patient, appointments, medications);
       }
   }
   ```

2. **External API consumers** (third parties calling your API)
   ```
   // You want to hide implementation details from external callers
   // If third party calls GetPatientQuery directly, you can't refactor it
   // But if they call IPatientService, you can change queries internally
   ```

3. **Different response shapes** for different use cases
   ```csharp
   public interface IPatientService
   {
       Task<PatientDto> GetPatientAsync(Guid id);              // full data
       Task<PatientSummaryDto> GetPatientSummaryAsync(Guid id); // minimal data
       Task<PatientChartDto> GetPatientChartAsync(Guid id);     // doctor view
   }
   ```

**For your MVP:** None of these apply yet. Use simplified approach.

---

## Migration Path (If You Need It Later)

If you start with Approach 2 and later need Approach 1:

```csharp
// Step 1: Create interface (wraps existing queries)
public interface IPatientService
{
    Task<GetPatientResponse> GetPatientAsync(Guid patientId)
        => _mediator.Send(new GetPatientQuery(patientId));
}

// Step 2: Update cross-module calls to use interface
// Before: await _mediator.Send(new GetPatientQuery(id));
// After: await _patientService.GetPatientAsync(id);

// Step 3: Rest of code remains unchanged
```

It's a one-afternoon refactor. Not a big deal.

---

## Summary: Recommendation for Your MVP

**Use Approach 2: CQRS Commands/Queries in Contracts**

✅ **Fewer files** — one file per operation  
✅ **Simpler contracts** — exactly what queries exist  
✅ **Faster development** — less boilerplate  
✅ **Easy cross-module calls** — `mediator.Send(new GetPatientQuery(...))`  
✅ **Can upgrade later** — add service facade when needed  

For a health portal with 4 modules and ~20 features, you'll write **~40% less code** compared to the traditional approach.

**When to migrate to Approach 1:** When you need business orchestration or external API stability (probably Year 2+).

**For now: Keep it simple. Ship faster.**
