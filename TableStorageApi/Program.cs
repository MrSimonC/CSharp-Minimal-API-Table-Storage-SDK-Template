using Azure.Data.Tables;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var tableConnString = builder.Configuration["TableStorageConnectionString"] ?? throw new NullReferenceException("need conn string");
var tableName = "ProgramStatus";
var partitionKey = "ProgramStatus";
var tableClient = new TableClient(tableConnString, tableName);

app.MapGet("/", () => Results.Json(GetStatuses(tableClient, tableName)));
app.MapPost("/", (StatusRecord record) => AddEntityToTable(tableClient, record, partitionKey));

app.Run();

static void AddEntityToTable(TableClient tableClient, StatusRecord record, string partitionKey)
{
    var entity = new TableEntity(partitionKey, Guid.NewGuid().ToString())
    {
        { "Program", record.Program },
        { "Date", record.Date },
        { "Message", record.Message }
    };
    _ = tableClient.AddEntity(entity);
}

static List<StatusRecord> GetStatuses(TableClient tableClient, string partitionKey)
{
    var queryResultsFilter = tableClient.Query<TableEntity>(filter: $"PartitionKey eq '{partitionKey}'");
    var result = new List<StatusRecord>();
    foreach (var item in queryResultsFilter)
    {
        result.Add(new StatusRecord(item.GetString("Program"), item.GetDateTime("Date") ?? DateTime.MinValue, item.GetString("Message")));
    }

    return result;
}

public record StatusRecord(string Program, DateTime Date, string Message);