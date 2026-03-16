using System;

namespace AgenticAI.MyTools;

public class WeatherTool
{
    public string GetWeather(string city)
    {
        return $"Weather in {city} is 25°C";
    }
}
