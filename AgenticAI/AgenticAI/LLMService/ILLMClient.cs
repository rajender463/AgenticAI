using System;

namespace AgenticAI.OpenAIService;

public interface ILLMClient
{
    Task<string> AskAsync(string prompt, CancellationToken cancellationToken = default);
}
