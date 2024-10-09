using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Hr.Api.Hiring;

public class CommandController(IDocumentSession session, IMartenOutbox outbox) : ControllerBase
{
    [HttpPost("/hiring/employees")]
    public async Task<ActionResult> HireEmployeeAsync([FromBody] HireEmployeeRequest request, CancellationToken ct)
    {
        var entity = new EmployeeEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            EmployeeId = request.EmployeeId
        };
        session.Store(entity);
        await outbox.PublishAsync(new EmployeeHired(entity.Id, entity.Name, entity.EmployeeId));
        await session.SaveChangesAsync(ct);
        
        
        return Ok(entity);
    }
    
    [HttpGet("/hiring/employees")]
    public async Task<ActionResult> GetEmployeesAsync(CancellationToken ct)
    {
        var employees = await session.Query<EmployeeEntity>().ToListAsync(ct);
        return Ok(employees);
    }
    [HttpGet("/hiring/employees/{id:guid}")]
    public async Task<ActionResult> GetEmployeeAsync(Guid id, CancellationToken ct)
    {
        var employee = await session.LoadAsync<EmployeeEntity>(id, ct);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }
    
    [HttpPost("/hiring/employees/{id:guid}/salary")]
    public async Task<ActionResult> SetSalaryAsync(Guid id, [FromBody] decimal salary, CancellationToken ct)
    {
        var employee = await session.LoadAsync<EmployeeEntity>(id, ct);
        if (employee == null)
            return NotFound();
        var oldSalary = employee.Salary ?? 0;
        employee.Salary = salary;
        session.Store(employee);
        await outbox.PublishAsync(new EmployeeSalaryAssigned(employee.Id, oldSalary, salary));
        await session.SaveChangesAsync(ct);
        return Ok(employee);
    }
}

public record HireEmployeeRequest(string Name, string EmployeeId);

public record EmployeeHired(Guid Id, string Name, string EmployeeId);

public record EmployeeSalaryAssigned(Guid Id, decimal Previous, decimal New); 
public class EmployeeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public decimal? Salary { get; set; } = null;
}