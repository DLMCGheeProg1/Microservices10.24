using Marten;

namespace HelpDesk.Api.SoftwareCenter;

[Route("software-center/catalog")]
public class SoftwareCenterController(IDocumentSession session) : ControllerBase
{
    [HttpPost("catalog")]

    public async Task<ActionResult> AddItemToCatalogAsync([FromBody] CatalogItemRequestModel request)
    {
        // whatever rules we have about this.. can't think of anything now, but validate the input.
        var @event = new CatalogItemAdded(Guid.NewGuid(), request.Title, request.Description);
        session.Events.StartStream(@event.Id, @event);
        await session.SaveChangesAsync();
        return Ok(@event);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCatalogItemAsync(Guid id)
    {
        // whatever rules (more on this tomorrow...)
        var @event = new CatalogItemRetired(id);
        session.Events.Append(@event.Id, @event);
        await session.SaveChangesAsync();
        return NoContent();
    }


}

// Models (Commands?)

public record CatalogItemRequestModel(string Title, string Description);

// Events
public record CatalogItemAdded(Guid Id, string Title, string Description);

public record CatalogItemRetired(Guid Id);

// Read Models


