using HelpDesk.Api.Incidents.Events;
using HelpDesk.Api.Services;
using Marten;

namespace HelpDesk.Api.Incidents.Endpoints.TierOne;

public record ContactRecordRequest(string Note);

[ApiExplorerSettings(GroupName = "Tier One Support")]
public class CommandsController(IDocumentSession session, IProvideUserInformation userInfo) : ControllerBase
{
 
    [HttpPost("/tierone/submitted-incidents/{incidentId:guid}/contact-records")]
    public async Task<ActionResult> AddContactRecordForIncident(
        Guid incidentId,
        [FromBody] ContactRecordRequest request)
    {
        var info = await userInfo.GetUserInfoAsync();
        // log an event to the event log.
        var evt = new IncidentContactRecorded(incidentId, info.UserId, request.Note);
        session.Events.Append(incidentId, evt);
        await session.SaveChangesAsync();

        return Ok(evt);
    }

}




