# Clean Domain Objects + Infrastructure Data Objects

## The Problem with Annotations in Domain

```csharp
// ❌ BAD: Domain coupled to MongoDB
[Collection("patients")]
public class Patient
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("firstName")]
    public string FirstName { get; set; }
    
    [BsonElement("email")]
    [Indexed]
    public string Email { get; set; }
    
    // Now Patient depends on MongoDB
    // Can't test without MongoDB
    // Can't switch to SQL without refactoring
}
```

## The Solution: Domain + Data Objects + Mapper

```csharp
// ✅ GOOD: Clean domain, persistence details in infrastructure
public class Patient  // Domain object (no annotations)
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string Email { get; private set; }
    // ... pure C#, zero dependencies
}

// Infrastructure layer
public class PatientDO  // Data Object (MongoDB details here)
{
    [BsonId]
    public ObjectId _id { get; set; }
    [BsonElement("firstName")]
    public string FirstName { get; set; }
    // ... MongoDB annotations only here
}

// Mapper: converts between domain ↔ infrastructure
public class PatientMapper
{
    public static PatientDO ToDataObject(Patient domain) { ... }
    public static Patient ToDomain(PatientDO data) { ... }
}
```

---

## Best Practice: Domain-Driven Design (DDD)

### Layer 1: Domain (Pure C#, Zero Dependencies)

```csharp
// Modules/Authentication/Auth.Module/Domain/User.cs
namespace PatientSync.Modules.Auth.Domain;

public class User
{
    // Properties are private setters (immutable after creation)
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Role { get; private set; }  // "patient" | "doctor" | "admin"
    public string Status { get; private set; } // "active" | "inactive" | "suspended"
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public int LoginCount { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool EmailVerified { get; private set; }
    public DateTime? EmailVerifiedAt { get; private set; }

    // Factory method (creation logic)
    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string role)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LoginCount = 0,
            EmailVerified = false
        };
    }

    // Domain logic (behavior)
    public void RecordLogin()
    {
        LoginCount++;
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerified = true;
        EmailVerifiedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = "suspended";
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive => Status == "active";
}
```

**Key characteristics:**
- ✅ Zero external dependencies
- ✅ Immutable (properties private set)
- ✅ Domain logic encapsulated (RecordLogin, VerifyEmail, etc.)
- ✅ Testable (no mocking needed)
- ✅ Can use with any database

---

### Layer 2: Infrastructure Data Object

```csharp
// Modules/Authentication/Auth.Module/Infrastructure/Persistence/UserDO.cs
namespace PatientSync.Modules.Auth.Infrastructure.Persistence;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

/// <summary>
/// UserDO = Data Object
/// This class is ONLY for MongoDB persistence.
/// Never use this in domain logic or contracts.
/// </summary>
[BsonIgnoreExtraElements]
public class UserDO
{
    [BsonId]
    public ObjectId _id { get; set; }
    
    [BsonElement("userId")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    
    [BsonElement("email")]
    public string Email { get; set; } = null!;
    
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;
    
    [BsonElement("firstName")]
    public string FirstName { get; set; } = null!;
    
    [BsonElement("lastName")]
    public string LastName { get; set; } = null!;
    
    [BsonElement("role")]
    public string Role { get; set; } = null!; // patient | doctor | admin
    
    [BsonElement("status")]
    public string Status { get; set; } = "active"; // active | inactive | suspended
    
    [BsonElement("loginCount")]
    public int LoginCount { get; set; }
    
    [BsonElement("lastLoginAt")]
    public DateTime? LastLoginAt { get; set; }
    
    [BsonElement("emailVerified")]
    public bool EmailVerified { get; set; }
    
    [BsonElement("emailVerifiedAt")]
    public DateTime? EmailVerifiedAt { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
```

**Key characteristics:**
- ✅ All MongoDB annotations here (never in domain)
- ✅ Matches MongoDB schema exactly
- ✅ Used ONLY in repository
- ✅ Never exposed to other layers

---

### Layer 3: Mapper (Converts Domain ↔ Data Object)

```csharp
// Modules/Authentication/Auth.Module/Infrastructure/Persistence/UserMapper.cs
namespace PatientSync.Modules.Auth.Infrastructure.Persistence;

using PatientSync.Modules.Auth.Domain;

public static class UserMapper
{
    /// <summary>
    /// Converts domain User → MongoDB UserDO (for saving)
    /// </summary>
    public static UserDO ToDataObject(User domain)
    {
        return new UserDO
        {
            UserId = domain.Id,
            Email = domain.Email,
            PasswordHash = domain.PasswordHash,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Role = domain.Role,
            Status = domain.Status,
            LoginCount = domain.LoginCount,
            LastLoginAt = domain.LastLoginAt,
            EmailVerified = domain.EmailVerified,
            EmailVerifiedAt = domain.EmailVerifiedAt,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    /// <summary>
    /// Converts MongoDB UserDO → domain User (for reading)
    /// </summary>
    public static User ToDomain(UserDO data)
    {
        return new User
        {
            Id = data.UserId,
            Email = data.Email,
            PasswordHash = data.PasswordHash,
            FirstName = data.FirstName,
            LastName = data.LastName,
            Role = data.Role,
            Status = data.Status,
            LoginCount = data.LoginCount,
            LastLoginAt = data.LastLoginAt,
            EmailVerified = data.EmailVerified,
            EmailVerifiedAt = data.EmailVerifiedAt,
            CreatedAt = data.CreatedAt,
            UpdatedAt = data.UpdatedAt
        };
    }
}
```

---

### Layer 4: Repository (Only Place Using UserDO)

```csharp
// Modules/Authentication/Auth.Module/Infrastructure/Persistence/UserRepository.cs
namespace PatientSync.Modules.Auth.Infrastructure.Persistence;

using PatientSync.Modules.Auth.Domain;
using MongoDB.Driver;

internal interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
    Task UpdateAsync(User user, CancellationToken ct);
}

internal class UserRepository : IUserRepository
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<UserDO> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _database = database;
        _collection = database.GetCollection<UserDO>("auth.users");
    }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        var filter = Builders<UserDO>.Filter.Eq(u => u.UserId, userId);
        var data = await _collection.Find(filter).FirstOrDefaultAsync(ct);
        
        return data is null ? null : UserMapper.ToDomain(data);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var filter = Builders<UserDO>.Filter.Eq(u => u.Email, email);
        var data = await _collection.Find(filter).FirstOrDefaultAsync(ct);
        
        return data is null ? null : UserMapper.ToDomain(data);
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        var data = UserMapper.ToDataObject(user);
        await _collection.InsertOneAsync(data, cancellationToken: ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        var data = UserMapper.ToDataObject(user);
        var filter = Builders<UserDO>.Filter.Eq(u => u.UserId, user.Id);
        
        await _collection.ReplaceOneAsync(
            filter,
            data,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken: ct);
    }
}
```

**Key characteristics:**
- ✅ ONLY place that uses UserDO
- ✅ Returns User (domain) not UserDO
- ✅ Interface is domain-focused (User, not UserDO)
- ✅ Mapper called on read and write

---

## How It Flows: The Complete Picture

```
Angular HTTP Request
    ↓
Feature Endpoint
    ↓
Feature Handler
    ↓
IUserRepository.GetByIdAsync(userId)  ← domain type in interface
    ↓
UserRepository.GetByIdAsync()
    ↓
MongoDB query returns UserDO
    ↓
UserMapper.ToDomain(userDO)  ← converts to domain
    ↓
returns User (domain)
    ↓
Feature Handler uses User
    ↓
Feature Handler calls user.RecordLogin()  ← domain logic
    ↓
Feature Handler calls IUserRepository.UpdateAsync(user)
    ↓
UserRepository.UpdateAsync(user)
    ↓
UserMapper.ToDataObject(user)  ← converts to persistence
    ↓
MongoDB update with UserDO
    ↓
Done
```

---

## Complete Example: RegisterUser Feature

```csharp
// Modules/Authentication/Auth.Module/Features/RegisterUser/RegisterUser.cs
namespace PatientSync.Modules.Auth.Features.RegisterUser;

using PatientSync.Modules.Auth.Domain;
using PatientSync.Modules.Auth.Contracts;

public static class RegisterUser
{
    public record Request(
        string Email,
        string Password,
        string FirstName,
        string LastName
    );

    public record Response(
        Guid UserId,
        string Email,
        string FullName
    );

    public static void MapEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("/register", Handle)
            .WithName("RegisterUser")
            .WithOpenApi()
            .Produces<Response>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Request request,
        IValidator<Request> validator,
        IMediator mediator,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Results.BadRequest(validationResult.Errors);

        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName
        );

        var result = await mediator.Send(command, ct);
        return Results.Created($"/api/auth/user/{result.UserId}", result);
    }

    // ─── Internal CQRS ───────────────────────────────────────────────────
    private record RegisterUserCommand(
        string Email,
        string Password,
        string FirstName,
        string LastName
    ) : IRequest<Response>;

    private class RegisterUserValidator : AbstractValidator<Request>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Must contain uppercase")
                .Matches("[a-z]").WithMessage("Must contain lowercase")
                .Matches("[0-9]").WithMessage("Must contain number");
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        }
    }

    private class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Response>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _hasher;

        public RegisterUserHandler(IUserRepository userRepository, IPasswordHasher hasher)
        {
            _userRepository = userRepository;
            _hasher = hasher;
        }

        public async Task<Response> Handle(RegisterUserCommand command, CancellationToken ct)
        {
            // Check if email already exists
            var existing = await _userRepository.GetByEmailAsync(command.Email, ct);
            if (existing is not null)
                throw new ConflictException($"Email {command.Email} already registered");

            // Create domain object (pure C#, no MongoDB)
            var passwordHash = _hasher.Hash(command.Password);
            var user = User.Create(
                command.Email,
                passwordHash,
                command.FirstName,
                command.LastName,
                role: "patient"
            );

            // Save to repository (mapper handles conversion)
            await _userRepository.AddAsync(user, ct);

            return new Response(
                user.Id,
                user.Email,
                $"{user.FirstName} {user.LastName}"
            );
        }
    }
}
```

---

## Example: LoginUser Feature (Uses RecordLogin Domain Logic)

```csharp
// Modules/Authentication/Auth.Module/Features/LoginUser/LoginUser.cs
private class LoginUserHandler : IRequestHandler<LoginUserCommand, Response>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _hasher;

    public async Task<Response> Handle(LoginUserCommand command, CancellationToken ct)
    {
        // Get user from repository (returns domain User)
        var user = await _userRepository.GetByEmailAsync(command.Email, ct);
        if (user is null || !_hasher.Verify(command.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        // Call domain logic (RecordLogin is a domain method)
        user.RecordLogin();

        // Save updated user back to repository
        await _userRepository.UpdateAsync(user, ct);

        // Return response
        return new Response(user.Id, user.Email, user.FirstName);
    }
}
```

---

## Why This Is Best Practice

| Aspect | Coupled to MongoDB | Clean Domain |
|---|---|---|
| **Testing** | Mock MongoDB | Mock only IUserRepository interface |
| **Switching DB** | Rewrite all domain | Only change UserDO + mapper + repo |
| **Domain Logic** | Not possible | RecordLogin(), Suspend(), etc. |
| **Readability** | Annotations clutter | Clear, pure C# |
| **Contracts** | Domain = persistence | Domain ≠ persistence |
| **Single Responsibility** | Entity does everything | Entity = domain, DO = persistence |

---

## File Structure

```
Modules/Authentication/Auth.Module/
├── Domain/
│   └── User.cs                          ← Pure C#, zero dependencies
│
├── Features/
│   ├── RegisterUser/
│   │   └── RegisterUser.cs              ← Uses domain User
│   └── LoginUser/
│       └── LoginUser.cs                 ← Calls user.RecordLogin()
│
├── Infrastructure/
│   └── Persistence/
│       ├── UserDO.cs                    ← MongoDB annotations
│       ├── UserMapper.cs                ← Domain ↔ Data conversion
│       └── UserRepository.cs            ← Only place using UserDO
│
└── AuthModule.cs                        ← DI registration
```

---

## DI Registration

```csharp
// Modules/Authentication/Auth.Module/AuthModule.cs
public static class AuthModule
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AuthModule).Assembly));
        services.AddValidatorsFromAssembly(typeof(AuthModule).Assembly);
        
        return services;
    }
}
```

---

## Summary

**Clean Domain Pattern:**
- ✅ Domain objects are PURE C#, zero external dependencies
- ✅ Data Objects (DO) live in Infrastructure with all MongoDB annotations
- ✅ Mapper converts between domain ↔ infrastructure
- ✅ Repository interface returns domain objects, not DOs
- ✅ Only repository implementation touches DOs
- ✅ Completely testable without MongoDB
- ✅ Can switch databases by only changing DO + mapper + repo

This is DDD (Domain-Driven Design) done right.
