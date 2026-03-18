using TripleSService.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddOpenApi("v1", options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Hyland Document Management API",
            Version = "v1",
            Description = "A comprehensive API for Hyland document management with query metering and disconnected mode support"
        };
        return Task.CompletedTask;
    });
});

// Ensure logging is properly configured
builder.Services.AddLogging();

// Configure Hyland connection
builder.Services.Configure<HylandConnectionConfiguration>(options =>
{
    options.AppServerUrl = builder.Configuration.GetConnectionString("HylandAppServer") 
        ?? "https://localhost/AppServer/Service.asmx";
    options.Username = builder.Configuration["Hyland:Username"] ?? "MANAGER";
    options.Password = builder.Configuration["Hyland:Password"] ?? "";
    options.DataSource = builder.Configuration["Hyland:DataSource"] ?? "OnBase";
    options.UseQueryMetering = builder.Configuration.GetValue<bool>("Hyland:UseQueryMetering", true);
    options.UseDisconnectedMode = builder.Configuration.GetValue<bool>("Hyland:UseDisconnectedMode", true);
    options.MaxQueriesPerHour = builder.Configuration.GetValue<int>("Hyland:MaxQueriesPerHour", 1000);
    options.MinConnections = builder.Configuration.GetValue<int>("Hyland:MinConnections", 2);
    options.MaxConnections = builder.Configuration.GetValue<int>("Hyland:MaxConnections", 10);
});

// Register Hyland services with proper dependency injection
builder.Services.AddSingleton<IHylandConnectionFactory>(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IOptions<HylandConnectionConfiguration>>().Value;
    var logger = serviceProvider.GetRequiredService<ILogger<HylandConnectionFactory>>();
    return new HylandConnectionFactory(config, logger);
});
builder.Services.AddScoped<IDocumentService, DocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // Add a simple endpoint to serve swagger UI
    app.MapGet("/", () => Results.Redirect("/openapi/v1.json"))
        .ExcludeFromDescription();
    
    app.MapGet("/swagger", () => 
        Results.Content("""
        <!DOCTYPE html>
        <html>
        <head>
            <title>Hyland Document Management API</title>
            <link rel="stylesheet" type="text/css" href="https://unpkg.com/swagger-ui-dist@3.52.5/swagger-ui.css" />
        </head>
        <body>
            <div id="swagger-ui"></div>
            <script src="https://unpkg.com/swagger-ui-dist@3.52.5/swagger-ui-bundle.js"></script>
            <script>
                SwaggerUIBundle({
                    url: '/openapi/v1.json',
                    dom_id: '#swagger-ui',
                    presets: [
                        SwaggerUIBundle.presets.apis,
                        SwaggerUIBundle.presets.standalone
                    ]
                });
            </script>
        </body>
        </html>
        """, "text/html"))
        .ExcludeFromDescription();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();
