using System.Net;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;



public class Hotel
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("relevanceScore")] public double RelevanceScore { get; set; }

    [JsonPropertyName("providerId")] public string ProviderId { get; set; }

    [JsonPropertyName("providerHotelId")] public string ProviderHotelId { get; set; }

    [JsonIgnore]
    public string ProviderHotelIdentifier
    {
        get
        {
            return ProviderName + "--" + ProviderHotelId;
        }
    }

    [JsonPropertyName("languageCode")] public string Language { get; set; }

    [JsonPropertyName("providerName")] public string ProviderName { get; set; }

    [JsonPropertyName("geoCode")] public GeoCode GeoCode { get; set; }

    [JsonPropertyName("neighbourhoods")] public IList<string> Neighbourhoods { get; set; }

    [JsonPropertyName("contact")] public Contact Contact { get; set; }

    [JsonPropertyName("chainCode")] public string ChainCode { get; set; }

    [JsonPropertyName("chainName")] public string ChainName { get; set; }

    [JsonPropertyName("category")] public string Category { get; set; }

    [JsonPropertyName("starRating")] public double StarRating { get; set; }

}

public class GeoCode
{
    [JsonPropertyName("lat")] public double Lat { get; set; }

    [JsonPropertyName("long")] public double Long { get; set; }
}

public class Contact
{
    [JsonPropertyName("address")] public Address Address { get; set; }

    [JsonPropertyName("phones")] public IList<string> Phones { get; set; }

    [JsonPropertyName("fax")] public IList<string> Faxes { get; set; }

    [JsonPropertyName("emails")] public IList<string> Emails { get; set; }

    [JsonPropertyName("website")] public string Website { get; set; }
}

public class Address
{
    [JsonPropertyName("line1")] public string Line1 { get; set; }

    [JsonPropertyName("line2")] public string Line2 { get; set; }

    [JsonPropertyName("destinationCode")] public string DestinationCode { get; set; }

    [JsonPropertyName("city")] public NameCode City { get; set; }

    [JsonPropertyName("state")] public NameCode State { get; set; }

    [JsonPropertyName("country")] public NameCode Country { get; set; }

    [JsonPropertyName("postalCode")] public string PostalCode { get; set; }
}

public class NameCode
{
    [JsonPropertyName("code")] public string Code { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }
}
