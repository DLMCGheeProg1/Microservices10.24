namespace HelpDesk.Api.TierOneSupport.Events;

public record IncidentContactRecorded(Guid Id, Guid TierOneTechId, string Note);

public record IncidentResolved(Guid Id, Guid TierOneTechId);

public record HighPriorityIncidentElevated(Guid Id, Guid TierOneTechId);
public record LowPriorityIncidentElevated(Guid Id, Guid TierOneTechId);
//- IsHighPriority

//- TechId
//- IncidentId
//- Description
//- Comments
