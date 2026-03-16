using System.IO;
using System.Text;
using System.Text.Json;
using AgenticAI.MyTools;
using Google.GenAI;
using Google.GenAI.Types;

namespace AgenticAI.OpenAIService;

public class GeminiClient : ILLMClient
{
    private readonly Client _client;
     private readonly WeatherTool _weather = new();
    private readonly TimeTool _time = new();
    public GeminiClient(IConfiguration configuration)
    {
        // Reads GOOGLE_API_KEY from environment automatically if not supplied directly.
        // You can also store it in configuration and pass it explicitly if you prefer.
        var apiKey = configuration["Gemini:ApiKey"];
        _client = new Client(apiKey: apiKey);
    }

    public async Task<string> AskAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var response = await _client.Models.GenerateContentAsync(
            model: "gemini-2.5-flash",
            contents: new List<Content> { new Content { Parts = new List<Part> { new Part { Text = prompt } } } },
            config: new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts =
                    [
                        new Part
                        {
                            Text = "Only call tools when the user explicitly asks for weather or time. Otherwise answer normally."
                        }
                    ]
                },
                MaxOutputTokens = 200,
                Temperature = 0.7f,
                Tools = new List<Tool>
                {
                    new Tool
                    {
                        FunctionDeclarations = new List<FunctionDeclaration>
                        {
                            new FunctionDeclaration
                            {
                                Name = "get_weather",
                                Description = "Get weather by city",
                                Parameters = new Schema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, Schema>
                                    {
                                        ["city"] = new Schema { Type = "string" }
                                    },
                                    Required = new List<string> { "city" }
                                }
                            },
                            new FunctionDeclaration
                            {
                                Name = "get_time",
                                Description = "Get current time by city",
                                Parameters = new Schema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, Schema>
                                    {
                                        ["city"] = new Schema { Type = "string" }
                                    },
                                    Required = new List<string> { "city" }
                                }
                            }
                        }
                    }
                }
            }
        );

        var functionCall = response.Candidates?
            .FirstOrDefault()?
            .Content?
            .Parts?
            .FirstOrDefault()?
            .FunctionCall;

        if (functionCall is null)
            return response.Text ?? "";

        var city = GetArgumentValue(functionCall.Args, "city");

        return functionCall.Name switch
        {
            "get_weather" => _weather.GetWeather(city),
            "get_time" => _time.GetTime(city),
            _ => "Unknown tool"
        };
    
    }

    private static string GetArgumentValue(object? args, string key)
    {
        if (args is null)
            return string.Empty;

        // If already JsonElement
        if (args is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Object &&
                jsonElement.TryGetProperty(key, out var value))
            {
                return value.GetString() ?? string.Empty;
            }
        }

        // Convert object to JSON safely, then parse
        var json = JsonSerializer.Serialize(args);
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind == JsonValueKind.Object &&
            doc.RootElement.TryGetProperty(key, out var prop))
        {
            return prop.GetString() ?? string.Empty;
        }

        return string.Empty;
    }
}