using System;
using Google.GenAI;
using Google.GenAI.Types;

namespace AgenticAI.OpenAIService;

public class GeminiClient : ILLMClient
{
    private readonly Client _client;
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
            contents: prompt,
            config: new GenerateContentConfig
            {
                MaxOutputTokens = 200,
                Temperature = 0.7f
            }
        );

        return response.Text ?? string.Empty;
    }
}
