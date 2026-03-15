using AgenticAI;
using AgenticAI.OpenAIService;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ILLMClient, GeminiClient>();
// builder.Services.AddSingleton<ILLMClient, GptAIClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/ask", async (AskRequest req, ILLMClient llmClient) =>
{
    var response = await llmClient.AskAsync(req.Prompt);
    return Results.Ok(new { response });
});

app.Run();

record AskRequest(string Prompt);