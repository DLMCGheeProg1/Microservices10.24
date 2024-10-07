using HelpDesk.Api.SoftwareCenter;
using Marten;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace HelpDesk.Api.Catalog;

public class CatalogController(IDocumentSession session) : ControllerBase
{
    [HttpGet("/catalog")]
    public async Task<ActionResult> GetFullCatalogAsync()
    {

        var response = await session.Query<SoftwareCatalogItem>().ToListAsync();
        return Ok(response);
    }
}


public class SoftwareCatalogItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;


}

public class SoftwareCatalogItemProjection : SingleStreamProjection<SoftwareCatalogItem>
{


    public SoftwareCatalogItemProjection()
    {
       
        DeleteEvent<CatalogItemRetired>(e => true);
    }

    public void Apply(CatalogItemAdded @event, SoftwareCatalogItem view)
    {
        view.Id = @event.Id;
        view.Title = @event.Title;
        view.Description = @event.Description;
    }

 
}