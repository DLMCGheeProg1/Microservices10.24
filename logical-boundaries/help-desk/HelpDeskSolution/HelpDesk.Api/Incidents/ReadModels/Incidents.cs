using HelpDesk.Api.Incidents.Events;
using Marten.Events;

namespace HelpDesk.Api.Incidents.ReadModels;


public enum IncidentStatus { PendingTier1Review, CustomerContacted }
public class Incident
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CatalogId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Created { get; set; }
    public IncidentStatus Status { get; set; }
    
}

public class IncidentSnapshot : Incident

{
    public static IncidentSnapshot Create(IEvent<EmployeeLoggedIncident> evt)
    {
        return new IncidentSnapshot
        {
            Id = evt.Id,
            Description = evt.Data.Description,
            CatalogId = evt.Data.SoftwareId,
            UserId = evt.Data.EmployeeId,
            Created = evt.Timestamp,
            Status = IncidentStatus.PendingTier1Review
        };

    }
    public void Apply(IncidentContactRecorded evt)
    {
        Status = IncidentStatus.CustomerContacted;
    }

    public bool ShouldDelete(EmployeeCancelledIncident evt)
    {
        return Status == IncidentStatus.PendingTier1Review; // the suspenders to your belt.
    }
}

