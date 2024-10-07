using HelpDesk.Api.Catalog.ReadModels;
using HelpDesk.Api.SoftwareCenter;
using Marten;
using Marten.Events.Aggregation;

namespace HelpDesk.Api.Catalog.Endpoints;

public class CatalogController(IDocumentSession session) : ControllerBase
{
    [HttpGet("/catalog")]
    [ApiExplorerSettings(GroupName = "Catalog")]
    public async Task<ActionResult> GetFullCatalogAsync()
    {

        var response = await session.Query<CatalogItem>().ToListAsync();
        return Ok(response);
    }
}


