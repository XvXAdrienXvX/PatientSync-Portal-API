// ============================================================
// PatientSync Portal — MongoDB Atlas Setup Script
// Run with: mongosh "<YOUR_ATLAS_CONNECTION_STRING>" atlas-setup.js
//
// Creates patientsync_db with all collections, JSON Schema
// validators, and indexes derived from each module's DOs.
// ============================================================

const db = db.getSiblingDB("patientsync_db");

// ────────────────────────────────────────────────────────────
// HELPER
// ────────────────────────────────────────────────────────────
function createCollectionWithValidator(name, schema) {
  const existing = db.getCollectionNames();
  if (existing.includes(name)) {
    print(`  [skip]   collection '${name}' already exists`);
    return;
  }
  db.createCollection(name, {
    validator: { $jsonSchema: schema },
    validationLevel: "moderate",   // allow updates to pre-existing docs
    validationAction: "error"
  });
  print(`  [create] collection '${name}'`);
}

// ============================================================
// MODULE: AUTHENTICATION
// Collection: auth.users
// Source: Authentication.Module › Domain/Users.cs + UsersDO.cs
//         + .claude/schema/auth-users.json (enriched)
// ============================================================
print("\n[AUTH]");

createCollectionWithValidator("auth.users", {
  bsonType: "object",
  required: ["_id", "email", "passwordHash", "role", "firstName", "lastName", "status", "emailVerified", "createdAt", "updatedAt"],
  additionalProperties: false,
  properties: {
    _id:             { bsonType: "string", description: "Guid as string" },
    email:           { bsonType: "string", description: "Unique user email" },
    passwordHash:    { bsonType: "string", description: "Bcrypt hash — never store plain text" },
    role:            { bsonType: "string", enum: ["patient", "doctor", "admin"] },
    firstName:       { bsonType: "string" },
    lastName:        { bsonType: "string" },
    lastLoginAt:     { bsonType: ["date", "null"] },
    loginCount:      { bsonType: "int", minimum: 0 },
    status:          { bsonType: "string", enum: ["active", "inactive", "suspended"] },
    emailVerified:   { bsonType: "bool" },
    emailVerifiedAt: { bsonType: ["date", "null"] },
    createdAt:       { bsonType: "date" },
    updatedAt:       { bsonType: "date" }
  }
});

db["auth.users"].createIndex({ email: 1 },  { unique: true, name: "uq_users_email" });
db["auth.users"].createIndex({ role: 1 },   { name: "idx_users_role" });
db["auth.users"].createIndex({ status: 1 }, { name: "idx_users_status" });
print("  [index]  auth.users: email(unique), role, status");

// ============================================================
// MODULE: PATIENTS
// Collections: patients.patients | patients.allergies | patients.medications
// Source: Patients.Module › Domain/ + Infrastructure/Persistence/
// ============================================================
print("\n[PATIENTS]");

createCollectionWithValidator("patients.patients", {
  bsonType: "object",
  required: ["_id", "userId", "dateOfBirth", "status", "createdAt", "updatedAt"],
  additionalProperties: false,
  properties: {
    _id:         { bsonType: "string", description: "Guid as string" },
    userId:      { bsonType: "string", description: "References auth.users._id" },
    dateOfBirth: { bsonType: "date" },
    status:      { bsonType: "string", enum: ["active", "inactive", "archived"] },
    doctorId:    { bsonType: ["string", "null"], description: "References doctors.doctors._id" },
    createdAt:   { bsonType: "date" },
    updatedAt:   { bsonType: "date" }
  }
});

db["patients.patients"].createIndex({ userId: 1 },   { unique: true, name: "uq_patients_userId" });
db["patients.patients"].createIndex({ doctorId: 1 }, { name: "idx_patients_doctorId" });
db["patients.patients"].createIndex({ status: 1 },   { name: "idx_patients_status" });
print("  [index]  patients.patients: userId(unique), doctorId, status");

// ──────────────────────────────────────────────
createCollectionWithValidator("patients.allergies", {
  bsonType: "object",
  required: ["_id", "patientId", "substance", "reactionType", "severity", "loggedAt", "createdAt", "updatedAt"],
  additionalProperties: false,
  properties: {
    _id:          { bsonType: "string" },
    patientId:    { bsonType: "string", description: "References patients.patients._id" },
    substance:    { bsonType: "string" },
    reactionType: { bsonType: "string" },
    severity:     { bsonType: "string", enum: ["mild", "moderate", "severe"] },
    loggedAt:     { bsonType: "date" },
    createdAt:    { bsonType: "date" },
    updatedAt:    { bsonType: "date" }
  }
});

db["patients.allergies"].createIndex({ patientId: 1 },            { name: "idx_allergies_patientId" });
db["patients.allergies"].createIndex({ patientId: 1, severity: 1 }, { name: "idx_allergies_patient_severity" });
print("  [index]  patients.allergies: patientId, patientId+severity");

// ──────────────────────────────────────────────
createCollectionWithValidator("patients.medications", {
  bsonType: "object",
  required: ["_id", "patientId", "name", "dosage", "frequency", "reason", "startedDate", "status", "createdAt", "updatedAt"],
  additionalProperties: false,
  properties: {
    _id:         { bsonType: "string" },
    patientId:   { bsonType: "string", description: "References patients.patients._id" },
    name:        { bsonType: "string" },
    dosage:      { bsonType: "string" },
    frequency:   { bsonType: "string" },
    reason:      { bsonType: "string" },
    startedDate: { bsonType: "date" },
    endedDate:   { bsonType: ["date", "null"] },
    status:      { bsonType: "string", enum: ["active", "inactive"] },
    createdAt:   { bsonType: "date" },
    updatedAt:   { bsonType: "date" }
  }
});

db["patients.medications"].createIndex({ patientId: 1 },          { name: "idx_medications_patientId" });
db["patients.medications"].createIndex({ patientId: 1, status: 1 }, { name: "idx_medications_patient_status" });
print("  [index]  patients.medications: patientId, patientId+status");

// ============================================================
// MODULE: APPOINTMENTS
// Collections: appointments.appointments | appointments.lab_orders
// Source: Appointments.Module › Domain/ + Infrastructure/Persistence/
// Note: VisitNotesDO is embedded inside AppointmentDO (not a standalone collection)
// ============================================================
print("\n[APPOINTMENTS]");

createCollectionWithValidator("appointments.appointments", {
  bsonType: "object",
  required: ["_id", "patientId", "doctorId", "scheduledAt", "duration", "chiefComplaint", "status", "createdAt", "updatedAt"],
  additionalProperties: false,
  properties: {
    _id:                { bsonType: "string" },
    patientId:          { bsonType: "string", description: "References patients.patients._id" },
    doctorId:           { bsonType: "string", description: "References doctors.doctors._id" },
    scheduledAt:        { bsonType: "date" },
    duration:           { bsonType: "int", minimum: 5, description: "Duration in minutes" },
    chiefComplaint:     { bsonType: "string" },
    status:             { bsonType: "string", enum: ["scheduled", "completed", "cancelled", "no-show"] },
    // Embedded VisitNotes — only present when status = "completed"
    visitNotes: {
      oneOf: [
        { bsonType: "null" },
        {
          bsonType: "object",
          required: ["assessment", "plan"],
          additionalProperties: false,
          properties: {
            assessment: { bsonType: "string" },
            plan:        { bsonType: "string" },
            deniedAt:   { bsonType: ["date", "null"] }
          }
        }
      ]
    },
    cancelledAt:         { bsonType: ["date", "null"] },
    cancelledBy:         { bsonType: ["string", "null"] },
    cancellationReason:  { bsonType: ["string", "null"] },
    createdAt:           { bsonType: "date" },
    updatedAt:           { bsonType: "date" }
  }
});

db["appointments.appointments"].createIndex({ patientId: 1, scheduledAt: -1 }, { name: "idx_appt_patient_date" });
db["appointments.appointments"].createIndex({ doctorId: 1, scheduledAt: -1 },  { name: "idx_appt_doctor_date" });
db["appointments.appointments"].createIndex({ status: 1 },                      { name: "idx_appt_status" });
db["appointments.appointments"].createIndex({ scheduledAt: 1 },                 { name: "idx_appt_scheduledAt" });
print("  [index]  appointments.appointments: patientId+date, doctorId+date, status, scheduledAt");

// ──────────────────────────────────────────────
// lab_orders — Source: LabOrder.cs + LabOrderDO.cs + .claude/schema/lab-order.json
createCollectionWithValidator("appointments.lab_orders", {
  bsonType: "object",
  required: ["orderId", "patientId", "doctorId", "testType", "orderedDate", "status"],
  additionalProperties: false,
  properties: {
    orderId:              { bsonType: "string", description: "Primary key — Guid as string" },
    patientId:            { bsonType: "string", description: "References patients.patients._id" },
    doctorId:             { bsonType: "string", description: "References doctors.doctors._id" },
    testType:             { bsonType: "string", enum: ["CBC", "Metabolic Panel", "TSH"] },
    orderedDate:          { bsonType: "date" },
    status:               { bsonType: "string", enum: ["Ordered", "Results Received"] },
    resultsPdfUrl:        { bsonType: ["string", "null"] },
    doctorNotes:          { bsonType: ["string", "null"] },
    resultsReceivedDate:  { bsonType: ["date", "null"] }
  }
});

db["appointments.lab_orders"].createIndex({ patientId: 1 },                     { name: "idx_lab_patientId" });
db["appointments.lab_orders"].createIndex({ doctorId: 1 },                      { name: "idx_lab_doctorId" });
db["appointments.lab_orders"].createIndex({ patientId: 1, orderedDate: -1 },    { name: "idx_lab_patient_date" });
db["appointments.lab_orders"].createIndex({ status: 1 },                        { name: "idx_lab_status" });
print("  [index]  appointments.lab_orders: patientId, doctorId, patientId+date, status");

// ============================================================
// MODULE: DOCTOR
// Collection: doctors.doctors
// Source: Doctor.Module › Domain/Doctor.cs + DoctorAvailability.cs
//         + Infrastructure/Persistence/DoctorDO.cs + DoctorAvailabilityDO.cs
//
// NOTE: DoctorDO does not store email/firstName/lastName — those fields
// live in auth.users. The schema below includes them as optional denormalized
// fields. Populate them at creation time so dashboards avoid a second lookup.
// ============================================================
print("\n[DOCTOR]");

createCollectionWithValidator("doctors.doctors", {
  bsonType: "object",
  required: ["_id", "userId", "specialization", "licenseNumber", "availability", "status", "createdAt", "updatedAt"],
  additionalProperties: false,
  properties: {
    _id:            { bsonType: "string" },
    userId:         { bsonType: "string", description: "References auth.users._id" },
    // Denormalized from auth.users — avoids lookup on every dashboard render
    email:          { bsonType: ["string", "null"] },
    firstName:      { bsonType: ["string", "null"] },
    lastName:       { bsonType: ["string", "null"] },
    specialization: { bsonType: "string" },
    licenseNumber:  { bsonType: "string" },
    availability: {
      bsonType: "array",
      items: {
        bsonType: "object",
        required: ["dayOfWeek", "startTime", "endTime", "slotDuration"],
        additionalProperties: false,
        properties: {
          dayOfWeek:    { bsonType: "int", minimum: 0, maximum: 6, description: "0=Sun … 6=Sat" },
          startTime:    { bsonType: "string", description: "HH:mm" },
          endTime:      { bsonType: "string", description: "HH:mm" },
          slotDuration: { bsonType: "int", minimum: 5, description: "Minutes per slot" }
        }
      }
    },
    status:    { bsonType: "string", enum: ["active", "on-leave", "inactive"] },
    createdAt: { bsonType: "date" },
    updatedAt: { bsonType: "date" }
  }
});

db["doctors.doctors"].createIndex({ userId: 1 }, { unique: true, name: "uq_doctors_userId" });
db["doctors.doctors"].createIndex({ status: 1 }, { name: "idx_doctors_status" });
print("  [index]  doctors.doctors: userId(unique), status");

// ============================================================
// MODULE: MESSAGING
// Collection: messaging.messages
// Source: Messaging.Module › Domain/Message.cs + MessageDO.cs
// ============================================================
print("\n[MESSAGING]");

createCollectionWithValidator("messaging.messages", {
  bsonType: "object",
  required: ["_id", "senderId", "recipientId", "senderRole", "recipientRole", "content", "status", "isDeleted", "createdAt", "updatedAt"],
  additionalProperties: false,
  properties: {
    _id:           { bsonType: "string" },
    senderId:      { bsonType: "string", description: "References auth.users._id" },
    recipientId:   { bsonType: "string", description: "References auth.users._id" },
    senderRole:    { bsonType: "string", enum: ["patient", "doctor", "admin"] },
    recipientRole: { bsonType: "string", enum: ["patient", "doctor", "admin"] },
    subject:       { bsonType: ["string", "null"] },
    content:       { bsonType: "string" },
    status:        { bsonType: "string", enum: ["unread", "read", "archived"] },
    readAt:        { bsonType: ["date", "null"] },
    isDeleted:     { bsonType: "bool", description: "Soft delete — never hard-delete for audit" },
    createdAt:     { bsonType: "date" },
    updatedAt:     { bsonType: "date" }
  }
});

db["messaging.messages"].createIndex({ recipientId: 1, status: 1, createdAt: -1 }, { name: "idx_msg_inbox" });
db["messaging.messages"].createIndex({ senderId: 1, createdAt: -1 },               { name: "idx_msg_sent" });
db["messaging.messages"].createIndex(
  { senderId: 1, recipientId: 1, createdAt: -1 },
  { name: "idx_msg_thread" }
);
print("  [index]  messaging.messages: recipientId+status+date (inbox), senderId+date (sent), thread");

// ============================================================
// DONE
// ============================================================
print("\n✓ PatientSync Atlas setup complete.");
print("  Database : patientsync_db");
print("  Collections created:");
[
  "auth.users",
  "patients.patients",
  "patients.allergies",
  "patients.medications",
  "appointments.appointments",
  "appointments.lab_orders",
  "doctors.doctors",
  "messaging.messages"
].forEach(c => print("    - " + c));
