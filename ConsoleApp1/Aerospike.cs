using Aerospike.Client;

public static class AerospikeData
{

    private readonly static MsgPackSerializer _serializer = new();

    public static void Start()
    {
        var client = new AerospikeClient("ip", 3000);

        string namespaceName = "ns";
        string setName = "set";

        // Create a delegate for the scan callback
        ScanCallback scanCallback = Callback;

        // Create a Scan object with a callback
        client.ScanAll(null, namespaceName, setName, scanCallback);

        // Close the Aerospike client
        client.Close();
    }

    private static void Callback(Key key, Record record)
    {
        try
        {
            Hotel hotel = _serializer.DeSerialize<Hotel>((byte[])record.bins["basiccontent"]);

            ElasticSearchData.UpdateMappings([TranslateToLuceneItem(hotel)]);
        }
        catch (Exception)
        {
            //supress
        }
    }

    public static MappingLuceneItem TranslateToLuceneItem(Hotel hotel)
    {
        return new MappingLuceneItem
        {
            Id = hotel.Id,
            Name = hotel.Name
        };
    }
}
