using System.Text;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.RateLimiting;
using Markdig;

const string PlaygroundCorsPolicyName = "PlaygroundCors";
const int MaxTextLength = 1000;
const int RateLimitPermitLimit = 30;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = GetAllowedOrigins(builder.Configuration);

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = RateLimitPermitLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(PlaygroundCorsPolicyName, policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .WithMethods("GET", "OPTIONS");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors(PlaygroundCorsPolicyName);
app.UseRateLimiter();

app.MapGet("/", () => string.Empty);

app.MapGet("/api/health", () =>
{
    return Results.Ok(new { name = "markdig", status = "ok", version = Markdown.Version });
});

app.MapGet("/api/to_html", (string text, string extension) =>
{
    try
    {
        text ??= string.Empty;
        if (text.Length > MaxTextLength)
        {
            text = text[..MaxTextLength];
        }

        var pipeline = new MarkdownPipelineBuilder().Configure(extension).Build();
        var result = Markdown.ToHtml(text, pipeline);

        return Results.Ok(new { name = "markdig", html = result, version = Markdown.Version });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { name = "markdig", html = "exception: " + GetPrettyMessageFromException(ex), version = Markdown.Version });
    }
});

app.Run();

static string[] GetAllowedOrigins(IConfiguration configuration)
{
    var configuredOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
    if (configuredOrigins is not null && configuredOrigins.Length > 0)
    {
        return configuredOrigins;
    }

    return
    [
        "https://xoofx.github.io",
        "http://localhost:4000",
        "http://127.0.0.1:4000"
    ];
}

static string GetPrettyMessageFromException(Exception exception)
{
    var builder = new StringBuilder();
    var current = exception;
    while (current != null)
    {
        builder.Append(current.Message);
        current = current.InnerException;
    }

    return builder.ToString();
}
