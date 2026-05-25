# Health Portal MVP — Product Specification

**Version:** 1.0  
**Status:** MVP (Phase 1)  
**Target Release:** Week 6  
**Platform:** Angular 21 PWA (responsive, mobile-first)  
**Backend:** .NET 10 Modular Monolith  
**Database:** MongoDB  

---

## Executive Summary

MVP V1:
A simple patient portal enabling online appointment booking and health records access, paired with a doctor portal for appointment management and patient context before visits.

**Core Value Proposition:**
- Patients: Book appointments online, access and log personal health records (optional for V1 wearable data tracking), View Lab results
- Doctor: See patient schedule, access patient history and health records, reduce admin calls, Share Lab results


---

## Product Overview

### Patient Portal

Patients can:
- View upcoming and past appointments
- Book new appointments online
- View their health records (medications, conditions, allergies, past visit notes)
- Send messages to their doctor
- Receive appointment reminders

### Doctor Portal

Dr. Demers can:
- View today's schedule and patient list
- Access complete patient charts before appointments
- View pending messages from patients
- Add notes after appointments
- Track patient alerts and no-shows

### Shared Features

- Secure login (demo mode: fake credentials)
- Health records database (patient data)
- Appointment scheduling system
- Basic messaging (non-real-time, refresh to see new messages)
- Mobile-responsive design

---

## Detailed Feature Specification

## MODULE 1: AUTHENTICATION & NAVIGATION

### Feature 1.1: Login Page

**User Stories:**
- AS a patient, I WANT to log in with credentials SO THAT I can access my health records
- AS a doctor, I WANT to log in with credentials SO THAT I can access my schedule and patient charts

For Demo and MVP V1: Use fake credentials (john@patient.com / password123, dr@admedica.com / password123)

**Acceptance Criteria:**
- [ ] Login form accepts email/password
- [ ] Form validates email format
- [ ] Form validates password length (min 6 characters)
- [ ] Success: User directed to dashboard
- [ ] Failure: Error message displays ("Invalid credentials")
- [ ] Demo mode: Use fake credentials (john@patient.com / password123, dr@admedica.com / password123)
- [ ] Login persists across page refresh (session token)
- [ ] Logout button clears session

**Design:**
- Minimalist form (email, password, login button)
- Admedica branding (logo, color scheme)
- Mobile-responsive (works on phone/tablet)
- No 2FA (Phase 2)

**Data Stored:**
- User ID
- Email
- Role (patient / doctor)
- Session token

---

### Feature 1.2: Main Navigation / Header

**User Stories:**
- AS a patient, I WANT clear navigation SO THAT I can find what I need quickly
- AS a doctor, I WANT to see my current view and switch between sections

**Acceptance Criteria - Patient:**
- [ ] Header shows patient name
- [ ] Header shows current section (Dashboard, Appointments, Health Records, Messages)
- [ ] Navigation menu links: Dashboard, Book Appointment, View Appointments, Health Records, Messages, Logout
- [ ] Active section is highlighted
- [ ] Menu collapses on mobile (hamburger icon)
- [ ] Logout button clears session and returns to login

**Acceptance Criteria - Doctor:**
- [ ] Header shows "Dr. Annie Demers"
- [ ] Navigation menu links: Dashboard, Patient Search, Logout
- [ ] Active section is highlighted
- [ ] Menu collapses on mobile

**Design:**
- Clean header bar (white/light background)
- Logo on left, user info on right
- Sidebar or top nav for menu items
- Mobile: hamburger menu

---

## MODULE 2: PATIENT DASHBOARD

### Feature 2.1: Dashboard Overview

**User Story:**
- AS a patient, I WANT a quick overview of my health status and upcoming appointments SO THAT I know what I need to do

**Acceptance Criteria:**
- [ ] Dashboard loads within 2 seconds
- [ ] Shows patient's name
- [ ] Displays "Next Appointment" card prominently
- [ ] Displays "Unread Messages" count
- [ ] Displays "Medication Refills" status
- [ ] Mobile-responsive layout
- [ ] No errors on load

**Metrics Displayed:**

1. **Next Appointment Card**
   - Doctor name: "Dr. Annie Demers"
   - Date and time: "Tuesday, January 14, 2025 at 2:00 PM"
   - Clinic location/phone: "Admedica, (555) 123-4567"
   - Action buttons: [Cancel Appointment] [Reschedule]

2. **Unread Messages Card**
   - Count: "1 new message"
   - Preview of latest message (first 50 characters)
   - "From: Dr. Annie Demers"
   - Link: [View All Messages]

3. **Medication Refills Card**
   - List of active medications with days remaining
   - Color coding: Red (<7 days), Yellow (7-30 days), Green (>30 days)
   - Example: "Lisinopril - 3 days left [Request Refill]"
   - Example: "Metformin - 2 weeks left ✓"

4. **Quick Action Buttons (Bottom)**
   - [Book Appointment]
   - [View Health Records]
   - [Send Message]
   - [Download Records]

**Design:**
- Card-based layout
- Cards stack on mobile, 2-column on desktop
- Clear visual hierarchy (appointment at top)
- Color coding for status (red = urgent, green = good)

**Data Required:**
- Patient's next appointment (date, time, doctor name)
- Unread message count
- Active medications with refill dates
- Recent lab results summary

---

## MODULE 3: APPOINTMENT BOOKING

### Feature 3.1: Book Appointment

**User Story:**
- AS a patient, I WANT to book an appointment online SO THAT I don't have to call the clinic

**Acceptance Criteria:**
- [ ] Patients see available appointment slots
- [ ] Slots shown as calendar (month/week view)
- [ ] Only available times are shown (not already booked)
- [ ] Patient selects date and time
- [ ] Form asks for chief complaint (text input)
- [ ] Confirmation message after booking
- [ ] Appointment appears on patient's dashboard
- [ ] Confirmation email/SMS notification (optional for MVP)
- [ ] Booking validates: No past dates, no double-booking

**Flow:**
1. Patient clicks "Book Appointment"
2. Calendar opens showing Dr. Demers' schedule (next 30 days)
3. Available slots shown in green (booked slots grayed out)
4. Patient selects a slot
5. Form appears: "Chief Complaint" (text input)
6. Patient types reason for visit (e.g., "Annual checkup")
7. Patient clicks "Confirm Booking"
8. Confirmation message: "Your appointment is booked for Tuesday, Jan 14 at 2:00 PM"
9. Appointment listed on patient's dashboard and doctor's schedule

**Design:**
- Calendar interface (mobile-friendly)
- Clear slot availability (color-coded)
- Simple form (one field: chief complaint)
- Confirmation screen

**Data Stored:**
- Appointment ID
- Patient ID
- Doctor ID
- Appointment date/time
- Chief complaint
- Status (scheduled / completed / cancelled)

**Validation Rules:**
- Cannot book past dates
- Cannot book slots already taken
- Chief complaint required (min 10 characters)
- Cannot double-book same patient

---

### Feature 3.2: View Appointments

**User Story:**
- AS a patient, I WANT to see my upcoming and past appointments SO THAT I can remember when I'm scheduled

**Acceptance Criteria:**
- [ ] Upcoming appointments shown at top
- [ ] Past appointments shown below
- [ ] Each appointment shows: Date, Time, Doctor name, Chief complaint
- [ ] Upcoming appointments have [Cancel] [Reschedule] buttons
- [ ] Past appointments show "Completed" status
- [ ] Clicking appointment shows full details
- [ ] No appointments message: "You have no upcoming appointments. [Book Now]"
- [ ] Responsive on mobile

**Display Format:**
```
Upcoming Appointments:
├─ Tuesday, Jan 14, 2025 at 2:00 PM
│  Dr. Annie Demers - Annual Checkup
│  [Cancel] [Reschedule]
│
└─ (No more upcoming appointments)

Past Appointments:
├─ Monday, Oct 15, 2024
│  Dr. Annie Demers - Annual Physical
│  Status: Completed
│  [View Notes]
└─ ...
```

**Data Required:**
- Patient's appointment list (filtered by date)
- Doctor name
- Appointment status
- Chief complaint
- Doctor's notes (if completed)

---

### Feature 3.3: Cancel/Reschedule Appointment

**User Story:**
- AS a patient, I WANT to cancel or reschedule my appointment SO THAT I can change my plans

**Acceptance Criteria:**
- [ ] Cancel button opens confirmation dialog
- [ ] Dialog confirms: "Cancel appointment with Dr. Demers on [date]?"
- [ ] Patient confirms cancellation
- [ ] Appointment status changes to "Cancelled"
- [ ] Appointment removed from "Upcoming" list
- [ ] Doctor sees cancellation on her schedule
- [ ] Reschedule button opens calendar (same as booking)
- [ ] Patient selects new date/time
- [ ] Confirmation message

**Design:**
- Modal dialog for confirmation
- Simple yes/no buttons
- Reschedule uses same calendar interface as booking

---

## MODULE 4: HEALTH RECORDS

### Feature 4.1: View Health Records

**User Story:**
- AS a patient, I WANT to view my health records SO THAT I know my medical history and current status

**Acceptance Criteria:**
- [ ] Patient can see their complete health record
- [ ] Record organized by section (tabs or scrollable sections)
- [ ] All sections load without errors
- [ ] Mobile-responsive layout
- [ ] Data displays clearly (not raw database format)

**Sections:**

1. **My Information**
   - Name, Date of Birth, Age
   - Contact info (phone, email)
   - Address
   - Allergies (highlighted in red if any)

2. **Medical Conditions**
   - List of active conditions (e.g., "Hypertension", "Type 2 Diabetes")
   - Status for each (e.g., "Controlled", "Active")
   - Diagnosis date

   Example:
   ```
   Active Conditions:
   ├─ Hypertension (Diagnosed: 2019)
   │  Status: Controlled
   └─ Type 2 Diabetes (Diagnosed: 2015)
      Status: Well-controlled
   ```

3. **Medications**
   - Name, dosage, frequency
   - Reason/indication
   - Start date
   - Example: "Lisinopril 10mg - Once daily - For Hypertension - Since 2019"

4. **Allergies & Adverse Reactions**
   - Substance name
   - Reaction type (e.g., "Hives", "Anaphylaxis")
   - Severity (mild, moderate, severe)
   - Date documented

   Example:
   ```
   Penicillin - Type: Antibiotic - Reaction: Hives, throat swelling
   ```

5. **Visit History**
   - Date, doctor name, chief complaint, summary of visit
   - Clickable to view full notes
   - Most recent first

   Example:
   ```
   Oct 15, 2024 - Dr. Annie Demers
   Chief Complaint: Annual Physical
   Summary: "All vitals normal. BP controlled. Continue current medications."
   Status: Completed
   ```

6. **Lab Results** (if available)
   - Test name, result value, normal range, date
   - Color-coded: Green (normal), Yellow (borderline), Red (abnormal)
   - Example: "HbA1c: 6.8% (normal <5.7%) - Oct 15, 2024"

**Data Stored:**
- Patient demographics
- Medical conditions list
- Active medications
- Allergies
- Appointment history with visit notes
- Lab results (if available)

**Design:**
- Tab-based interface (one tab per section)
- Cards for each item
- Mobile-responsive (tabs become scrollable on mobile)
- Clear typography (not crowded)

---

### Feature 4.2: Download/Print Records

**User Story:**
- AS a patient, I WANT to download or print my records SO THAT I can share them with another doctor

**Acceptance Criteria:**
- [ ] "Download PDF" button generates complete health record as PDF
- [ ] PDF includes all sections (demographics, conditions, meds, allergies, visits, labs)
- [ ] PDF is readable and professional-looking
- [ ] Print button opens browser print dialog
- [ ] User can print to PDF or physical printer
- [ ] Download/print includes timestamp

**Design:**
- Simple buttons at top of health records page
- PDF should match the on-screen layout
- Professional formatting (Admedica logo, clean design)

---

## MODULE 5: MESSAGING (PATIENT SIDE)

### Feature 5.1: View Messages

**User Story:**
- AS a patient, I WANT to see messages from my doctor SO THAT I can understand medical advice or test results

**Acceptance Criteria:**
- [ ] Patient sees list of all messages from Dr. Demers
- [ ] Messages show: Date, Preview (first 100 chars), "Read" status
- [ ] Unread messages highlighted or marked
- [ ] Clicking message shows full content
- [ ] Most recent message first
- [ ] No messages message: "No messages yet. Send a message to your doctor."

**Display Format:**
```
Messages:
├─ Jan 10, 2025 - "Your lab results are back. Everything looks..." [Unread]
├─ Jan 5, 2025 - "Please call the clinic to schedule your follow-up" [Read]
└─ Dec 28, 2024 - "Your prescription is ready at the pharmacy" [Read]
```

**Data Stored:**
- Message ID
- Sender (Dr. Demers)
- Recipient (patient)
- Content
- Date sent
- Read status

---

### Feature 5.2: Send Message to Doctor

**User Story:**
- AS a patient, I WANT to send a message to my doctor SO THAT I can ask questions without calling

**Acceptance Criteria:**
- [ ] Patient sees message compose form
- [ ] Form has text area for message content
- [ ] Form has "Subject" field (optional)
- [ ] Send button sends message
- [ ] Confirmation message: "Message sent to Dr. Demers"
- [ ] Message appears in message history (from patient perspective)
- [ ] Doctor sees message on her dashboard

**Design:**
- Simple text form (subject optional, message required)
- Send button (disabled if message empty)
- Character count (optional, max 1000 chars)
- Mobile-responsive

**Validation:**
- Message required (min 10 characters)
- Max 1000 characters

**Data Stored:**
- Message ID
- Sender (patient)
- Recipient (Dr. Demers)
- Subject (optional)
- Content
- Date sent
- Read status (for doctor)

---

## MODULE 6: DOCTOR DASHBOARD

### Feature 6.1: Dashboard Overview

**User Story:**
- AS Dr. Demers, I WANT a quick overview of my day SO THAT I can see my schedule and any urgent issues

**Acceptance Criteria:**
- [ ] Dashboard loads within 2 seconds
- [ ] Shows "Today's Schedule" prominently
- [ ] Shows "Unread Messages" and patient alerts
- [ ] Shows pending lab results (if any)
- [ ] Mobile-responsive layout
- [ ] Urgent items appear first (if any)

**Metrics Displayed:**

1. **Today's Schedule Card** (CRITICAL)
   - List of appointments with times and patient names
   - Chief complaint for each
   - Status: Scheduled, Completed, Cancelled, No-show
   - "View Chart" link for each patient
   - Example:
   ```
   Today's Schedule (Thursday, Jan 14):
   ├─ 9:00 AM - John Smith: Annual Checkup [View Chart]
   ├─ 10:30 AM - Sarah Johnson: Cough [View Chart]
   ├─ 2:00 PM - Michael Brown: Knee Pain [View Chart]
   └─ 3:30 PM - Lisa Chen: CANCELLED
   
   Summary: Scheduled: 3 | Completed: 0 | No-shows: 0
   ```

2. **Urgent Messages/Alerts Card** (CRITICAL if any)
   - Flagged messages from patients (e.g., chest pain, urgent)
   - Color-coded by priority: Red (urgent), Yellow (normal)
   - "Respond" button
   - Example: "John Smith (1 hour ago): 'Chest pain, shortness of breath' [Urgent Response]"

3. **Pending Lab Results Card** (if any)
   - List of labs awaiting review
   - Test name, patient name, date ordered, expected date
   - "View" button
   - Example: "Sarah Johnson - CBC - Ordered Jan 10 - Expected Today [View]"

4. **Quick Action Buttons (Bottom)**
   - [Patient Search]
   - [View Lab Results]
   - [Messages]

**Design:**
- Card-based layout (schedule at top, largest)
- Urgent items appear at top in red
- Clear visual hierarchy
- Mobile-responsive

**Data Required:**
- Today's appointments (from database)
- Unread messages count
- Patient alerts (urgent/flagged)
- Pending lab results

---

### Feature 6.2: Patient Search

**User Story:**
- AS Dr. Demers, I WANT to search for a patient SO THAT I can find their chart quickly

**Acceptance Criteria:**
- [ ] Search bar accepts patient name or ID
- [ ] Results show matching patients (name, DOB, phone)
- [ ] Clicking result opens patient chart
- [ ] Search is case-insensitive
- [ ] No results message if not found
- [ ] Fast search (returns results in <1 second)

**Design:**
- Simple search bar (search by name or ID)
- Results dropdown (click to select)
- Opens patient chart on selection

**Data Query:**
- Search patients table by name (contains) or ID (exact)
- Return: Name, DOB, ID, phone

---

## MODULE 7: PATIENT CHART (DOCTOR VIEW)

### Feature 7.1: View Patient Chart

**User Story:**
- AS Dr. Demers, I WANT to see a patient's complete chart SO THAT I have all their information before an appointment

**Acceptance Criteria:**
- [ ] Chart displays patient demographics
- [ ] Chart shows all allergies (highlighted if critical)
- [ ] Chart shows active medical conditions
- [ ] Chart shows current medications
- [ ] Chart shows visit history (past 5 visits, most recent first)
- [ ] Chart shows past visit notes
- [ ] Chart loads without errors
- [ ] Chart is keyboard-accessible
- [ ] Mobile-responsive (scrollable on mobile)

**Sections:**

1. **Patient Demographics** (Header)
   - Name, DOB, Age
   - Contact info
   - **ALLERGIES (RED if any critical)**
   - Insurance (optional)

2. **Active Medical Conditions**
   - List with status (e.g., "Hypertension - Controlled")

3. **Current Medications**
   - Name, dosage, frequency, reason, start date
   - Check for drug interactions (Phase 2)

4. **Visit History** (Recent 5 visits)
   - Date, chief complaint, status (completed/scheduled)
   - [View Full Notes] link

5. **Past Visit Notes** (Latest visit)
   - Full note from most recent visit
   - Assessment and plan

**Design:**
- Tab-based interface OR collapsible sections
- Patient name/DOB at top (always visible)
- **ALLERGIES HIGHLIGHTED IN RED (CRITICAL)**
- Scrollable on mobile
- "Back to Schedule" button to return to dashboard

**Data Required:**
- Patient ID
- All fields from patient health records

---

### Feature 7.2: Add Visit Notes

**User Story:**
- AS Dr. Demers, I WANT to document a visit SO THAT the chart is updated after I see a patient

**Acceptance Criteria:**
- [ ] Form appears after appointment completed
- [ ] Form has text area for notes
- [ ] Notes saved to patient chart
- [ ] Notes timestamp recorded (date, time, doctor name)
- [ ] Previous notes remain visible
- [ ] Form has character limit (optional, max 2000)
- [ ] Success message: "Notes saved"

**Design:**
- Simple text form
- Optional structured template (Phase 2)
- Save button
- Cancel button returns to dashboard

**Data Stored:**
- Visit note ID
- Patient ID
- Doctor ID
- Note text
- Timestamp
- Appointment ID (link to appointment)

**Validation:**
- Notes required (min 20 characters)
- Max 2000 characters

---

## MODULE 8: DOCTOR MESSAGING

### Feature 8.1: View Messages from Patients

**User Story:**
- AS Dr. Demers, I WANT to see messages from patients SO THAT I can respond to questions

**Acceptance Criteria:**
- [ ] Dr. Demers sees list of all messages from patients
- [ ] Messages show: Date, Patient name, Preview (first 50 chars), "Read" status
- [ ] Unread messages highlighted
- [ ] Clicking message shows full content
- [ ] Most recent first
- [ ] "Mark as Read" option
- [ ] "Delete" option (soft delete, not hard)

**Design:**
- List of messages with preview
- Unread count badge
- Mobile-responsive

---

### Feature 8.2: Reply to Message

**User Story:**
- AS Dr. Demers, I WANT to reply to patient messages SO THAT patients get answers to their questions

**Acceptance Criteria:**
- [ ] Reply form shows patient message above
- [ ] Reply text area for typing response
- [ ] Send button sends reply
- [ ] Success message: "Reply sent"
- [ ] Reply appears in conversation thread
- [ ] Patient sees notification of new message

**Design:**
- Full message visible above reply form
- Text area for response
- Send button
- Mobile-responsive

**Data Stored:**
- Reply stored as new message
- Linked to original message (conversation thread)
- Timestamp

---

## MODULE 9: APPOINTMENT MANAGEMENT (DOCTOR VIEW)

### Feature 9.1: Mark Appointment Completed

**User Story:**
- AS Dr. Demers, I WANT to mark an appointment as completed SO THAT my schedule shows accurate status

**Acceptance Criteria:**
- [ ] "Complete" button on appointment in dashboard
- [ ] Clicking completes appointment
- [ ] Status changes to "Completed" on schedule
- [ ] Appointment moves to "Completed" section
- [ ] Timestamp recorded

**Design:**
- Button on each appointment in today's schedule
- Confirmation: "Mark Jan 14 2:00 PM appointment as completed?"

---

### Feature 9.2: View No-Show/Cancellation Status

**User Story:**
- AS Dr. Demers, I WANT to see no-shows and cancellations SO THAT I know my schedule accuracy

**Acceptance Criteria:**
- [ ] Dashboard shows today's no-show count
- [ ] Cancelled appointments marked as "CANCELLED"
- [ ] No-shows tracked (patient didn't show up for scheduled appointment)
- [ ] 30-day no-show rate visible (bottom of dashboard, optional)

**Design:**
- Summary at bottom of today's schedule
- Example: "Completed: 2 | No-shows: 1 | Cancellations: 1"

---

## MODULE 10: DATA & PERSISTENCE

### Feature 10.1: Sample Data / Demo Data

**Acceptance Criteria:**
- [ ] Database seeded with sample patients (3-5)
- [ ] Sample appointments scheduled for next 30 days
- [ ] Sample medications, conditions, allergies populated
- [ ] Sample visit notes in history
- [ ] Demo credentials work (john@patient.com, dr@admedica.com)

**Sample Data:**
```
Patients:
├─ John Smith (DOB: 1985-03-15)
│  ├─ Conditions: Hypertension, Type 2 Diabetes
│  ├─ Meds: Lisinopril, Metformin, Cetirizine
│  ├─ Allergies: Penicillin (anaphylaxis risk)
│  └─ Appointments: Jan 14 2:00 PM (Annual Checkup)
│
├─ Sarah Johnson (DOB: 1992-07-22)
│  ├─ Conditions: Pregnancy (28 weeks)
│  ├─ Meds: Prenatal Vitamin
│  ├─ Allergies: None
│  └─ Appointments: Jan 14 10:30 AM (Checkup)
│
└─ Michael Brown (DOB: 1978-11-05)
   ├─ Conditions: Hypertension
   ├─ Meds: Lisinopril, Atorvastatin
   ├─ Allergies: NSAIDs (GI issues)
   └─ Appointments: Jan 14 2:00 PM (Follow-up)
```

### Feature 10.2: Database Schema

**Tables:**
- Users (id, email, password_hash, role, created_at)
- Patients (id, user_id, first_name, last_name, dob, phone, email, address)
- Doctors (id, user_id, first_name, last_name, specialization, phone, email)
- MedicalConditions (id, patient_id, condition_name, status, diagnosed_date)
- Medications (id, patient_id, name, dosage, frequency, reason, started_date)
- Allergies (id, patient_id, substance, reaction, severity, documented_date)
- Appointments (id, patient_id, doctor_id, scheduled_at, chief_complaint, status, created_at)
- VisitNotes (id, appointment_id, patient_id, doctor_id, note_text, created_at)
- Messages (id, sender_id, recipient_id, subject, content, created_at, read_at)

---

## Non-Functional Requirements

### Performance
- Dashboard load time: <2 seconds
- Patient chart load time: <1 second
- Search results: <1 second
- Appointment booking: Submit within 3 seconds

### Security
- HTTPS only
- Password hashing (bcrypt)
- Session tokens expire after 24 hours (for demo, can be longer)
- Input validation on all forms
- SQL injection prevention (parameterized queries)
- No sensitive data in URLs

### Reliability
- No unhandled errors (error boundaries)
- Graceful error messages


## Out of Scope (Phase 2+)

❌ Wearable data integration  
❌ Prescription refill workflow  
❌ 2FA / Multi-factor authentication  
❌ Medical alerts / clinical decision support  
❌ Admin portal  
❌ Mobile app (PWA sufficient)  

---

## Success Criteria (MVP Complete)

- [ ] All modules functional and tested
- [ ] Patient can book appointment online without calling
- [ ] Patient can view health records
- [ ] Patient can send/receive messages
- [ ] Dr. Demers can view schedule and patient charts
- [ ] Dr. Demers can add visit notes
- [ ] Dashboard metrics display correctly
- [ ] No critical bugs
- [ ] Load times meet performance targets
- [ ] Mobile-responsive on all screen sizes
- [ ] Error handling in place (no white screens)
- [ ] Sample data seeded
- [ ] Login with demo credentials works
- [ ] Deployment ready (one-click deploy)

---

## Development Timeline

**Week 1:** Fake Data + Patient Dashboard + Navigation  
**Week 2:** Appointment Booking + Health Records  
**Week 3:** Patient Messaging + Doctor Dashboard + Patient Chart  
**Week 4:** Doctor Messaging + Visit Notes + Deploy in Demo env

---

## Version History

| Version | Date | Changes |
|---|---|---|
| 1.0 | Jan 2025 | Initial MVP spec |

