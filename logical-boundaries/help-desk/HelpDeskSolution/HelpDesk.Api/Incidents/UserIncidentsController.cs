using HelpDesk.Api.Services;
using Marten;
using Marten.Events;

namespace HelpDesk.Api.Incidents;

public class UserIncidentsController(IProvideUserInformation userInfoProvider, IDocumentSession session) : ControllerBase
{
    [HttpPost("/user/authorized-software/{catalogId:guid}/incidents")]

    public async Task<ActionResult> AddUserIncidentAsync(
        Guid catalogId,
        [FromBody] UserIncidentRequestModel request)
    {

        // TODO: if the catalog id doesn't exist for this user or whatever, return a 404.
        // if it has to be approved for this particular user, it should be a 403 


        // todo: validate the request model, if it is bad, return a 400.
        // todo: mark this controller or method as requiring authorization, and THAT will return 401 or 403
        var employeeId = await userInfoProvider.GetUserInfoAsync();
        var evt = new EmployeeLoggedIncident(Guid.NewGuid(), employeeId.UserId, catalogId, request.Description);

        // ??? Save it? 
        session.Events.StartStream(evt.Id, evt); // A stream is a bunch of related things that happen to something over time.
        await session.SaveChangesAsync();

        return Ok(evt);
    }

    [HttpGet("/user/authorized-software/{catalogId:guid}/incidents/{incidentId:guid}")]
    public async Task<ActionResult> GetIncidentForCatalogItemAsync(Guid catalogId, Guid incidentId)
    {
        // Todo - verify the catalogId??

        // might be a little inefficient - it is going to read ALL the events in that stream every time
        // and create this.
        var readModel = session.LoadAsync<IncidentReadModel>(incidentId);
        if (readModel == null)
        {
            return NotFound();
        }
        return Ok(readModel);
    }

    [HttpDelete("user/incidents/{incidentId:guid}")]
    public async Task<ActionResult> CancelIncidentAsync(Guid incidentId)
    {
        // Decide - can they do this.
        //   - only the user that created this issue can delete - 403
        //   - don't mark one as cancelled if it doesn't exist.
        //   - it can only be deleted if it is in the PendingTeir1 review state, after that
        //     ?? Maybe the tier1 can delete it? ask? but we'll return a http Conflict response.

        // always return a 204. No content. 
        session.Events.Append(incidentId, new EmployeeCancelledIncident(incidentId));
        await session.SaveChangesAsync();
        return NoContent();
    }
}


// Model
public record UserIncidentRequestModel(string Description);

// Read Models

public enum IncidentStatus { PendingTier1Review }
public class IncidentReadModel
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CatalogId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset Created { get; set; }
    public IncidentStatus Status { get; set; }

    public static IncidentReadModel Create(IEvent<EmployeeLoggedIncident> evt)
    {
        return new IncidentReadModel
        {
            Id = evt.Id,
            Description = evt.Data.Description,
            CatalogId = evt.Data.SoftwareId,
            UserId = evt.Data.EmployeeId,
            Created = evt.Timestamp,
            Status = IncidentStatus.PendingTier1Review
        };

    }

    public bool ShouldDelete(EmployeeCancelledIncident evt)
    {
        return true;
    }


}