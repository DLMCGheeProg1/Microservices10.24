namespace HelpDesk.Api.Incidents.Events;

public record IncidentContactRecorded(Guid Id, Guid TierOneTechId, string Note);