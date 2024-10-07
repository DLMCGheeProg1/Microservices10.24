using HelpDesk.Api.Incidents.Events;
using HelpDesk.Api.Incidents.ReadModels;
using HelpDesk.Api.Services;
using Marten;

namespace HelpDesk.Api.Incidents.Endpoints.UserIncidents;
public record UserIncidentRequestModel(string Description);
[ApiExplorerSettings(GroupName = "User Incidents")]
public class CommandsController(IProvideUserInformation userInfoProvider, IDocumentSession session) : ControllerBase
{
    [HttpPost("/user/catalog/{catalogId:guid}/incidents")]

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

 

    [HttpDelete("user/incidents/{incidentId:guid}")]
    public async Task<ActionResult> CancelIncidentAsync(Guid incidentId)
    {
        // Decide - can they do this.
        //   - only the user that created this issue can delete - 403
        //   - don't mark one as cancelled if it doesn't exist.
        //   - it can only be deleted if it is in the PendingTeir1 review state, after that
        //     ?? Maybe the tier1 can delete it? ask? but we'll return a http Conflict response.

        // always return a 204. No content. 

   
        var rl = await session.Events.FetchForWriting<Incident>(incidentId);
        var savedIncident = rl.Aggregate;
        if (savedIncident == null)
        {

            return NotFound();
        }
        if (savedIncident.Status != IncidentStatus.PendingTier1Review)
        {

            return StatusCode(409);
        }

        session.Events.Append(incidentId, new EmployeeCancelledIncident(incidentId));
        await session.SaveChangesAsync(); // only append this if the version of the read model is the same as when I read it originally
        return NoContent();
    }
}