using Patients.Module.Domain;

namespace Patients.Module.Infrastructure.Persistence;

public static class PatientsMapper
{
    public static PatientDAO ToDataObject(Patient domain)
    {
        return new PatientDAO
        {
            Id = domain.Id,
            UserId = domain.UserId,
            DateOfBirth = domain.DateOfBirth,
            Status = domain.Status,
            DoctorId = domain.DoctorId,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Patient ToDomain(PatientDAO data)
    {
        return Patient.Rehydrate(
            data.Id,
            data.UserId,
            data.DateOfBirth,
            data.Status,
            data.DoctorId,
            data.CreatedAt,
            data.UpdatedAt);
    }

    public static MedicationDAO ToDataObject(Medication domain)
    {
        return new MedicationDAO
        {
            Id = domain.Id,
            PatientId = domain.PatientId,
            Name = domain.Name,
            Dosage = domain.Dosage,
            Frequency = domain.Frequency,
            Reason = domain.Reason,
            StartedDate = domain.StartedDate,
            EndedDate = domain.EndedDate,
            Status = domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Medication ToDomain(MedicationDAO data)
    {
        return Medication.Rehydrate(
            data.Id,
            data.PatientId,
            data.Name,
            data.Dosage,
            data.Frequency,
            data.Reason,
            data.StartedDate,
            data.EndedDate,
            data.Status,
            data.CreatedAt,
            data.UpdatedAt);
    }

    public static AllergyDAO ToDataObject(Allergy domain)
    {
        return new AllergyDAO
        {
            Id = domain.Id,
            PatientId = domain.PatientId,
            Substance = domain.Substance,
            ReactionType = domain.ReactionType,
            Severity = domain.Severity,
            LoggedAt = domain.LoggedAt,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Allergy ToDomain(AllergyDAO data)
    {
        return Allergy.Rehydrate(
            data.Id,
            data.PatientId,
            data.Substance,
            data.ReactionType,
            data.Severity,
            data.LoggedAt,
            data.CreatedAt,
            data.UpdatedAt);
    }
}
