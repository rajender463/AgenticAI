using System;
using OpenAI;
using OpenAI.Chat;

namespace AgenticAI.OpenAIService;
public class GptClient : ILLMClient
{
    private readonly ChatClient _client;
    public GptClient(IConfiguration configuration)
    {
        // Reads OPENAI_API_KEY from environment automatically if not supplied directly.
            // You can also store it in configuration and pass it explicitly if you prefer.
            var client = new OpenAIClient(
                apiKey: configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        _client = client.GetChatClient("gpt-4o-mini");
    }

    public async Task<string> AskAsync(string prompt, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new UserChatMessage(prompt)
        };

        var chatCompletionOptions = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 200
        };
        
        var response = await _client.CompleteChatAsync(messages, chatCompletionOptions, cancellationToken: cancellationToken);

        return response.Value.Content[0].Text;
    }
}
