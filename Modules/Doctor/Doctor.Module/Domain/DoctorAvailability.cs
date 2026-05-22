using System;

namespace Doctor.Module.Domain;

public class DoctorAvailability
{
    public DoctorAvailability(int dayOfWeek, string startTime, string endTime, int slotDuration)
    {
        DayOfWeek = dayOfWeek;
        StartTime = startTime.Trim();
        EndTime = endTime.Trim();
        SlotDuration = slotDuration;
    }

    public int DayOfWeek { get; internal set; }
    public string StartTime { get; internal set; } = null!;
    public string EndTime { get; internal set; } = null!;
    public int SlotDuration { get; internal set; }

    internal static DoctorAvailability Rehydrate(int dayOfWeek, string startTime, string endTime, int slotDuration)
    {
        return new DoctorAvailability(dayOfWeek, startTime, endTime, slotDuration);
    }

    public void UpdateSchedule(string startTime, string endTime, int slotDuration)
    {
        StartTime = startTime.Trim();
        EndTime = endTime.Trim();
        SlotDuration = slotDuration;
    }
}
