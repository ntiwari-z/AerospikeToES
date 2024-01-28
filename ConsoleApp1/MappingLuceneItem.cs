using System.Text.Json.Serialization;

public class MappingLuceneItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }


    [JsonPropertyName("name")]
    public string Name { get; set; }
}