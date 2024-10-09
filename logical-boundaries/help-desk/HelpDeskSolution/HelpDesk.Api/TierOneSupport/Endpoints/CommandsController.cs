using HelpDesk.Api.TierOneSupport.Events;
using HelpDesk.Api.User.Services;
using Marten;

namespace HelpDesk.Api.TierOneSupport.Endpoints;

public record ContactRecordRequest(string Note);

[ApiExplorerSettings(GroupName = "Tier One Support")]
public class CommandsController(IDocumentSession session, IProvideUserInformation userInfo) : ControllerBase
{

    //HTTP Post `/tierone/elevated-incidents/high-priority'
    //HTTP Post `/tierone/elevated-incidents/low-priority

    [HttpPost("/tierone/high-priority-incidents")]
    public async Task<ActionResult> ElevateIncidentHighPriority([FromBody] ElevateIncidentRequestModel request)
    {
        var info = await userInfo.GetUserInfoAsync();
        var evt = new HighPriorityIncidentElevated(request.Id, info.UserId);
        session.Events.Append(request.Id, evt);
        await session.SaveChangesAsync();
        return Ok();
    }
    [HttpPost("/tierone/low-priority-incidents")]
    public async Task<ActionResult> ElevateIncidentLowPriority([FromBody] ElevateIncidentRequestModel request)
    {
        var info = await userInfo.GetUserInfoAsync();
        var evt = new LowPriorityIncidentElevated(request.Id, info.UserId);
        session.Events.Append(request.Id, evt);
        await session.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("/tierone/submitted-incidents/{incidentId:guid}")]
    public async Task<ActionResult> ResolveIncidentAsync(Guid incidentId, CancellationToken ct)
    {

        var uInfo = await userInfo.GetUserInfoAsync();
        var evt = new IncidentResolved(incidentId, uInfo.UserId);
        session.Events.Append(incidentId, evt);
        await session.SaveChangesAsync();
        return NoContent();
    }



    [HttpPost("/tierone/submitted-incidents/{incidentId:guid}/contact-records")]
    public async Task<ActionResult> AddContactRecordForIncident(
        Guid incidentId,
        [FromBody] ContactRecordRequest request, CancellationToken ct)
    {
        var info = await userInfo.GetUserInfoAsync();
        // log an event to the event log.
        var evt = new IncidentContactRecorded(incidentId, info.UserId, request.Note);
        session.Events.Append(incidentId, evt);
        await session.SaveChangesAsync(ct);

        return Ok(evt);
    }
}

public record ElevateIncidentRequestModel
{
    public Guid Id { get; set; }
    public int Version { get; set; }


}