using System.Text.Json;

namespace NexusAdmin.Functions.Configuration;

public class JsonOptions
{
    public static JsonSerializerOptions Default => new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true // For more legible responses
    };
}