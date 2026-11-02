using inizproject.Services;
using System.Text.Json;
using inizproject.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IGoogleSearchService, GoogleSearchService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/search", async (string q, IGoogleSearchService searchService) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        return Results.BadRequest(new { error = "Ключевое слово не может быть пустым" });
    }

    var results = await searchService.SearchAsync(q);

    var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(results, new JsonSerializerOptions { WriteIndented = true });

    return Results.File(jsonBytes, "application/json", "google_search_results.json");
});

app.Run();