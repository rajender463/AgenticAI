using System;

namespace AgenticAI.MyTools;

public class TimeTool
{
    public string GetTime(string city)
    {
        return $"Current time in {city} is {DateTime.UtcNow}";
    }
}
