# Tier One Events


## Operation `IncidentContactRecorded`

This is invoked using an HTTP POST to `/tierone/submitted-incidents/{incidentId:guid}/contact-records`

### Operands:
- Id: Generated for each incident (this is a new "stream" or "topic")
- TechId: a REFERENCE to an employee that is in the TierOne support role.
- Note: What the tech wants to annotate about this.

## Operation: `IncidentResolved`

We could do this as a HTTP DELETE '/tierone/incidents/{id}`
We could do this as an HTTP POST `/tierone/resolved-incidents` 
We could do this as an HTTP PUT `/tierone/resolved-incidents/{id}`
(Post means "consider adding this to this collection" (put this in this bucket)), or "submit this entity for processing"


This incident is handled. We no longer need to display this as a list of incidents for anyone.
### Operands
- Id of the incident
- TechId - of the tech resolving it.
- Note: closing comments, why is this being resolved etc.



## Operation: `IncidentElevated`
This incident will no longer be an "incident" from the POV of the employee or the tech, but will become an Issue
HTTP Post `/tierone/elevated-incidents/high-priority'
HTTP Post `/tierone/elevated-incidents/low-priority
### Operands
- IsHighPriority
- EmployeeId (who logged this)
- TechId
- IncidentId
- Description
- Comments

