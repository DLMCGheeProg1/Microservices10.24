using Hr.Api.Hiring;
using Marten;
using Oakton;
using Oakton.Resources;
using Wolverine;
using Wolverine.Http;
using Wolverine.Kafka;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Host.ApplyOaktonExtensions();
var connectionString = builder.Configuration.GetConnectionString("data") ?? throw new Exception("No Connection String");
var kafkaBroker = builder.Configuration.GetConnectionString("kafka") ?? throw new Exception("No Kafka Broker");
builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString);

}).UseLightweightSessions().IntegrateWithWolverine();
builder.Host.UseWolverine(opts =>
{
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.Policies.AutoApplyTransactions();

    opts.UseKafka(kafkaBroker);
    opts.PublishMessage<EmployeeHired>().ToKafkaTopic("hr.employees");
    opts.PublishMessage<EmployeeSalaryAssigned>().ToKafkaTopic("hr.employees.salary-changes");
    opts.Services.AddResourceSetupOnStartup();

});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

return await app.RunOaktonCommands(args);
