using Patients.Module.Domain;

namespace Patients.Module.Infrastructure.Persistence;

public static class PatientsMapper
{
    public static PatientDO ToDataObject(Patient domain)
    {
        return new PatientDO
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

    public static Patient ToDomain(PatientDO data)
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

    public static MedicationDO ToDataObject(Medication domain)
    {
        return new MedicationDO
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

    public static Medication ToDomain(MedicationDO data)
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

    public static AllergyDO ToDataObject(Allergy domain)
    {
        return new AllergyDO
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

    public static Allergy ToDomain(AllergyDO data)
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
