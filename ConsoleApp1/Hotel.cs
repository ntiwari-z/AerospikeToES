using System.Text.Json.Serialization;

public class Hotel
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
}