using HelpDesk.Api.Incidents.ReadModels;
using HelpDesk.Api.Services;
using Marten;

namespace HelpDesk.Api.Incidents.Endpoints.UserIncidents;
[ApiExplorerSettings(GroupName = "User Incidents")]
public class QueryController(IQuerySession session, IProvideUserInformation userProvider) : ControllerBase
{
    [HttpGet("/user/incidents", Name = "GetUserIncidents")]
    public async Task<ActionResult> GetUserIncidentsAsync()
    {
        var user = await userProvider.GetUserInfoAsync();
        var xx = await session.Query<Incident>().Where(s => s.UserId == user.UserId).ToListAsync();
       // var incidents = await session.LoadManyAsync<UserIncident>(user.UserId);
        return Ok(xx);
    }
    [HttpGet("/user/incidents/{incidentId:guid}")]
    public async Task<ActionResult> GetIncidentForCatalogItemAsync( Guid incidentId)
    {
        var user = await userProvider.GetUserInfoAsync();
        var readModel = await session.Query<Incident>().Where(s => s.UserId == user.UserId).SingleOrDefaultAsync(c => c.Id == incidentId);
        
        
        if (readModel == null)
        {
            return NotFound();
        }
        // This'll return a 404, but if you really wanted to, you could leave the userId out of the predicate
        // and then punch them for not minding their own business
        // if (user.UserId != readModel.UserId)
        // {
        //     return Forbid();
        // }
        return Ok(readModel);
    }
}