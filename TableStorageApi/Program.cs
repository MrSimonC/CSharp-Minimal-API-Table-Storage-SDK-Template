using Azure.Data.Tables;
using Middleware; // needed for app.UseApiKeyAuthentication()
using TableStorageApi.Models;

const string tableName = "ProgramStatus";
const string partitionKey = "StatusUpdate"; 
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
// app.UseApiKeyAuthentication(); // optionally use X-API-KEY authentication referencing value in Environment Variable API_KEY
app.Urls.Add("http://0.0.0.0:8080");

var tableConnString = builder.Configuration["TableStorageConnectionString"];
var tableClient = new TableClient(tableConnString, tableName);

app.MapGet("/", () => Results.Json(tableClient.Query<StatusEntry>()));
app.MapPost("/", (StatusEntry entry) => {
    entry.PartitionKey = partitionKey;
    tableClient.AddEntity(entry);
});

app.Run();
