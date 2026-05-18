# MongoDB Schema Design for PatientSync

## Design Principles

1. **Document-oriented** — denormalize aggressively (no joins in MongoDB)
2. **Minimize storage** — only store what's needed, no redundant data
3. **Fast queries** — design documents for the queries you'll run
4. **Vertical slices** — each module owns its collections
5. **No references between modules** — communicate via Contracts only

---

## Database & Collections Per Module

```
patientsync_db (single database)
├── patients.patients                    (Patients module)
├── patients.medications
├── patients.allergies
│
├── appointments.appointments            (Appointments module)
│
├── doctors.doctors                      (Doctor module)
│
├── messaging.messages                   (Messaging module)
│
└── auth.users                           (Authentication module)
```

---

## MODULE 1: PATIENTS

### Collection: `patients.patients`

```javascript
{
  _id: ObjectId,
  
  // Identity
  userId: UUID,                           // references auth.users
  email: "john@example.com",              // unique index
  firstName: "John",
  lastName: "Doe",
  dateOfBirth: ISODate("1985-03-15"),    // calculate age on read
  phone: "+1234567890",
  
  // Current state
  status: "active",                       // active | inactive | archived
  doctorId: ObjectId,                     // assigned doctor (denormalized, update on Doctor change)
  
  // Metadata
  createdAt: ISODate,
  updatedAt: ISODate
}
```

**Indexes:**
```javascript
db.patients.createIndex({ userId: 1 }, { unique: true })
db.patients.createIndex({ email: 1 }, { unique: true })
db.patients.createIndex({ doctorId: 1 })
```

**Size:** ~500 bytes per document
**Expected documents:** 100–10,000

---

### Collection: `patients.medications`

```javascript
{
  _id: ObjectId,
  
  patientId: ObjectId,                    // foreign key to patients.patients
  
  // Drug info
  name: "Lisinopril",
  dosage: "10mg",
  frequency: "Once daily",
  reason: "Hypertension",
  
  // Lifecycle
  startedDate: ISODate("2019-01-15"),
  endedDate: null,                        // null if still active
  status: "active",                       // active | inactive | stopped
  
  // Metadata
  createdAt: ISODate,
  updatedAt: ISODate
}
```

**Indexes:**
```javascript
db.medications.createIndex({ patientId: 1 })
```

**Size:** ~300 bytes per document
**Expected documents:** 100–50,000 (average 3–5 per patient)

---

### Collection: `patients.allergies`

```javascript
{
  _id: ObjectId,
  
  patientId: ObjectId,                    // foreign key
  
  // Allergy info
  substance: "Penicillin",
  reactionType: "Anaphylaxis",            // Hives | Swelling | Anaphylaxis | GI Issues | Rash
  severity: "severe",                     // mild | moderate | severe (CRITICAL FOR DOCTOR)
  
  // Metadata
  documentedDate: ISODate("2015-05-20"),
  createdAt: ISODate
}
```

**Indexes:**
```javascript
db.allergies.createIndex({ patientId: 1 })
db.allergies.createIndex({ severity: 1 })
```

**Size:** ~200 bytes per document
**Expected documents:** 100–20,000 (average 1–2 per patient)

---

## MODULE 2: APPOINTMENTS

### Collection: `appointments.appointments`

```javascript
{
  _id: ObjectId,
  
  // Booking info
  patientId: ObjectId,                    // foreign key to patients.patients
  doctorId: ObjectId,                     // foreign key to doctors.doctors
  
  // Schedule
  scheduledAt: ISODate("2025-01-14T14:00:00Z"),
  duration: 30,                           // minutes
  
  // Visit info
  chiefComplaint: "Annual checkup",
  status: "scheduled",                    // scheduled | completed | cancelled | no-show
  
  // Doctor notes (filled after visit)
  visitNotes: {
    assessment: "All vitals normal. BP controlled.",
    plan: "Continue current medications.",
    deniedAt: ISODate("2025-01-14T14:30:00Z")
  } || null,
  
  // Cancellation info
  cancelledAt: null,                      // ISODate if cancelled
  cancelledBy: "patient",                 // patient | doctor
  cancellationReason: null,               // "Patient request" | "Doctor emergency" | etc
  
  // Metadata
  createdAt: ISODate,
  updatedAt: ISODate
}
```

**Indexes:**
```javascript
db.appointments.createIndex({ patientId: 1, scheduledAt: -1 })
db.appointments.createIndex({ doctorId: 1, scheduledAt: -1 })
db.appointments.createIndex({ status: 1 })
db.appointments.createIndex({ scheduledAt: 1 })  // for availability queries
```

**Size:** ~600 bytes per document
**Expected documents:** 1,000–100,000 (depends on clinic volume)

---

## MODULE 3: DOCTOR

### Collection: `doctors.doctors`

```javascript
{
  _id: ObjectId,
  
  userId: UUID,                           // references auth.users
  email: "annie@admedica.com",            // unique
  firstName: "Annie",
  lastName: "Demers",
  
  // Practice info
  specialization: "General Practice",
  licenseNumber: "GP12345",
  
  // Availability (default weekly schedule)
  availability: [
    {
      dayOfWeek: "Monday",                // 0-6 (Monday-Sunday)
      startTime: "09:00",                 // HH:mm
      endTime: "17:00",
      slotDuration: 30                    // minutes per appointment
    },
    { dayOfWeek: "Tuesday", startTime: "09:00", endTime: "17:00", slotDuration: 30 },
    // ... etc
  ],
  
  // Current state
  status: "active",                       // active | on-leave | inactive
  
  // Metadata
  createdAt: ISODate,
  updatedAt: ISODate
}
```

**Indexes:**
```javascript
db.doctors.createIndex({ userId: 1 }, { unique: true })
db.doctors.createIndex({ email: 1 }, { unique: true })
```

**Size:** ~800 bytes per document
**Expected documents:** 1–100 (small MVP)

---

## MODULE 4: MESSAGING

### Collection: `messaging.messages`

```javascript
{
  _id: ObjectId,
  
  // Participants
  senderId: ObjectId,                     // from auth.users (doctor or patient)
  recipientId: ObjectId,                  // to auth.users (doctor or patient)
  
  senderRole: "doctor",                   // doctor | patient (denormalized for quick filtering)
  recipientRole: "patient",
  
  // Message content
  subject: null,                          // optional subject for thread
  content: "Your lab results are back. Everything looks good.",
  
  // Message state
  status: "unread",                       // unread | read | archived
  readAt: null,                           // ISODate when read
  
  // Metadata
  createdAt: ISODate,
  updatedAt: ISODate,
  isDeleted: false                        // soft delete (don't remove for audit)
}
```

**Indexes:**
```javascript
db.messages.createIndex({ recipientId: 1, status: 1, createdAt: -1 })  // inbox + unread filter
db.messages.createIndex({ senderId: 1, createdAt: -1 })                 // sent items
db.messages.createIndex({ senderId: 1, recipientId: 1, createdAt: -1 }) // conversation thread
```

**Size:** ~400 bytes per document
**Expected documents:** 10,000–1,000,000 (depends on usage)

---

## MODULE 5: AUTHENTICATION

### Collection: `auth.users`

```javascript
{
  _id: ObjectId,
  
  // Identity
  email: "john@example.com",              // unique index
  passwordHash: "$2b$12$...",             // bcrypt hash (never store plain password)
  
  // Role
  role: "patient",                        // patient | doctor | admin
  
  // Profile
  firstName: "John",
  lastName: "Doe",
  
  // Session
  lastLoginAt: ISODate,
  loginCount: 42,
  
  // Status
  status: "active",                       // active | inactive | suspended
  emailVerified: true,
  emailVerifiedAt: ISODate,
  
  // Metadata
  createdAt: ISODate,
  updatedAt: ISODate
}
```

**Indexes:**
```javascript
db.users.createIndex({ email: 1 }, { unique: true })
db.users.createIndex({ role: 1 })
db.users.createIndex({ status: 1 })
```

**Size:** ~400 bytes per document
**Expected documents:** 100–10,000

---

## Storage Estimate (Small MVP)

```
Patients:
  10 patients × 500 bytes =                5 KB
  30 medications × 300 bytes =             9 KB
  15 allergies × 200 bytes =               3 KB
  Subtotal: 17 KB

Appointments:
  100 appointments × 600 bytes =           60 KB

Doctors:
  1 doctor × 800 bytes =                   0.8 KB

Messaging:
  1,000 messages × 400 bytes =             400 KB

Authentication:
  12 users × 400 bytes =                   4.8 KB

TOTAL: ~486 KB
```

**MongoDB Atlas Free Tier:** 512 MB — easily fits 1000 patients + full history.

---

## Module-Specific Queries (Fast & Indexed)

### Patients Module

```javascript
// Get patient chart for doctor (before appointment)
db.patients.findOne({ _id: ObjectId })
db.medications.find({ patientId: ObjectId }).toArray()
db.allergies.find({ patientId: ObjectId }).toArray()

// Get upcoming appointment for patient dashboard
db.appointments.findOne({
  patientId: ObjectId,
  status: "scheduled",
  scheduledAt: { $gte: new Date() }
}, { sort: { scheduledAt: 1 } })
```

### Appointments Module

```javascript
// Get available slots for doctor on date
db.appointments.find({
  doctorId: ObjectId,
  scheduledAt: {
    $gte: new Date("2025-01-14"),
    $lt: new Date("2025-01-15")
  },
  status: { $ne: "cancelled" }
}).toArray()

// Get today's appointments for doctor
db.appointments.find({
  doctorId: ObjectId,
  scheduledAt: {
    $gte: new Date().setHours(0, 0, 0, 0),
    $lt: new Date().setHours(23, 59, 59, 999)
  }
}).toArray()
```

### Messaging Module

```javascript
// Get unread messages (patient/doctor dashboard)
db.messages.find({
  recipientId: ObjectId,
  status: "unread"
}).sort({ createdAt: -1 }).limit(10)

// Get conversation thread
db.messages.find({
  $or: [
    { senderId: ObjectId, recipientId: ObjectId },
    { senderId: ObjectId, recipientId: ObjectId }
  ]
}).sort({ createdAt: -1 }).limit(50)
```

---

## C# Entity Models (for EF Core with MongoDB provider)

```csharp
// Patients.Module/Domain/Patient.cs
[Collection("patients")]
public class Patient
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("userId")]
    public Guid UserId { get; set; }
    
    [BsonElement("email")]
    public string Email { get; set; } = null!;
    
    [BsonElement("firstName")]
    public string FirstName { get; set; } = null!;
    
    [BsonElement("lastName")]
    public string LastName { get; set; } = null!;
    
    [BsonElement("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }
    
    [BsonElement("phone")]
    public string? Phone { get; set; }
    
    [BsonElement("status")]
    public string Status { get; set; } = "active";
    
    [BsonElement("doctorId")]
    public ObjectId? DoctorId { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

// Appointments.Module/Domain/Appointment.cs
[Collection("appointments")]
public class Appointment
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("patientId")]
    public ObjectId PatientId { get; set; }
    
    [BsonElement("doctorId")]
    public ObjectId DoctorId { get; set; }
    
    [BsonElement("scheduledAt")]
    public DateTime ScheduledAt { get; set; }
    
    [BsonElement("duration")]
    public int Duration { get; set; } = 30;
    
    [BsonElement("chiefComplaint")]
    public string ChiefComplaint { get; set; } = null!;
    
    [BsonElement("status")]
    public string Status { get; set; } = "scheduled"; // scheduled | completed | cancelled | no-show
    
    [BsonElement("visitNotes")]
    public VisitNotes? VisitNotes { get; set; }
    
    [BsonElement("cancelledAt")]
    public DateTime? CancelledAt { get; set; }
    
    [BsonElement("cancelledBy")]
    public string? CancelledBy { get; set; }
    
    [BsonElement("cancellationReason")]
    public string? CancellationReason { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class VisitNotes
{
    [BsonElement("assessment")]
    public string Assessment { get; set; } = null!;
    
    [BsonElement("plan")]
    public string Plan { get; set; } = null!;
    
    [BsonElement("deniedAt")]
    public DateTime? DeniedAt { get; set; }
}
```

---

## Migration from SQL (PostgreSQL) to MongoDB

If you already have PostgreSQL data:

1. **Denormalize joins** — combine related tables into single documents
2. **Aggregate data** — pre-calculate counts, sums, etc. at write time
3. **Remove normalization** — store doctor name + ID in Appointment (not just ID)
4. **Simplify schemas** — drop foreign key constraints (MongoDB doesn't enforce them)

Example migration tool:
```csharp
// Pseudo-code: migrate PostgreSQL → MongoDB
var patients = dbContext.Patients.Include(p => p.Medications).Include(p => p.Allergies).ToList();
var mongoDb = mongoClient.GetDatabase("patientsync_db");

foreach (var patient in patients)
{
    var mongoPatient = new { 
        _id = ObjectId.GenerateNewId(),
        userId = patient.UserId,
        email = patient.Email,
        firstName = patient.FirstName,
        // ... map all properties
        medications = patient.Medications.Select(m => new { m.Name, m.Dosage, m.Frequency }).ToList()
    };
    
    await mongoDb.GetCollection<dynamic>("patients.patients").InsertOneAsync(mongoPatient);
}
```

---

## Notes for Your MVP

- **Single database** (`patientsync_db`) with collections grouped by module (prefix with module name)
- **No foreign key constraints** — enforce in application code
- **Denormalize heavily** — store doctor name, patient name in appointment (query speed > storage)
- **Soft deletes** — never hard delete for audit trail (use `isDeleted: true`)
- **Indexes matter** — create all recommended indexes before MVP launch
- **Storage is cheap** — MongoDB Atlas free tier = 512 MB, enough for 10,000+ patients
- **No transactions needed** — single-document ACID is sufficient for MVP
