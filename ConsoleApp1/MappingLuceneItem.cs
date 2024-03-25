using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;


public class MappingLuceneItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("geoCode")]
    public MappingGeoCode GeoCode
    {
        get
        {
            return MostMatchedGeoCode();
        }
    }

    private MappingGeoCode MostMatchedGeoCode()
    {
        //Creating groups based on equality comparer and selecting the value whose count is maximum
        //Comparing on the basis of first 4 digits after decimal
        return Data.Select(d => d.Pgc)
                    .Where(x => x != null)
                    .GroupBy(g => g, new MappingGeoCode())
                    .OrderByDescending(o => o.Count())
                    .FirstOrDefault()?.Key;
    }

    [JsonPropertyName("data")]
    public List<Datum> Data { get; set; }
}

public class Datum
{
    [JsonPropertyName("phi")]
    public string Phi { get; set; }

    [JsonPropertyName("pm")]
    public string Pm { get; set; }

    [JsonPropertyName("cn")]
    public string Cn { get; set; }

    [JsonPropertyName("sc")]
    public string Sc { get; set; }

    [JsonPropertyName("cc")]
    public string Cc { get; set; }

    [JsonPropertyName("chc")]
    public string Chc { get; set; }

    [JsonPropertyName("chn")]
    public string Chn { get; set; }

    [JsonPropertyName("sr")]
    public double Sr { get; set; }

    [JsonPropertyName("d")]
    public double D { get; set; }

    [JsonPropertyName("dc")]
    public string Dc { get; set; }

    [JsonPropertyName("sn")]
    public string Sn { get; set; }

    [JsonPropertyName("pgc")]
    public MappingGeoCode Pgc { get; set; }

    [JsonPropertyName("lut")]
    public DateTime Lut { get; set; }
}

public class MappingGeoCode : IEqualityComparer<MappingGeoCode>
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    public bool Equals(MappingGeoCode x, MappingGeoCode y)
    {
        return Trim(x?.Lat) == Trim(y?.Lat) && Trim(x?.Lon) == Trim(y?.Lon);
    }

    public int GetHashCode([DisallowNull] MappingGeoCode obj)
    {
        return Trim(obj?.Lat).GetHashCode() ^ Trim(obj?.Lon).GetHashCode();
    }

    //Comparing the first four digits after the decimal
    private static double Trim(double? input)
    {
        if (input == null)
            return 0;

        var output = Math.Truncate(10000 * input.Value) / 10000;
        return output;
    }
}