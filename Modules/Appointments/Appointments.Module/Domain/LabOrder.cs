using System;

namespace Appointments.Module.Domain
{
    internal record LabOrder
    {
        public Guid OrderId { get; init; }
        public Guid PatientId { get; init; }
        public Guid DoctorId { get; init; }

        public string TestType { get; init; } = default!;
        public DateTime OrderedDate { get; init; }
        public string Status { get; init; } = default!;

        public string? ResultsPdfUrl { get; init; }
        public string? DoctorNotes { get; init; }
        public DateTime? ResultsReceivedDate { get; init; }
    }
}
