using System;

namespace Appointments.Module.Domain;

public class VisitNotes
{
    public VisitNotes(string assessment, string plan, DateTime? deniedAt)
    {
        Assessment = assessment;
        Plan = plan;
        DeniedAt = deniedAt;
    }

    public string Assessment { get; internal set; }
    public string Plan { get; internal set; }
    public DateTime? DeniedAt { get; internal set; }

    internal static VisitNotes Rehydrate(string assessment, string plan, DateTime? deniedAt)
    {
        return new VisitNotes(assessment, plan, deniedAt);
    }
}
