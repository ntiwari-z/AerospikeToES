using Nest;

public class ElasticSearchData
{
    private static ElasticClient GetClient()
    {
        var settings = new ConnectionSettings(new Uri("url")).DefaultIndex("indexname")
                            .BasicAuthentication("user", "pwd")
                            .MemoryStreamFactory(Elasticsearch.Net.RecyclableMemoryStreamFactory.Default)
                            .DisableDirectStreaming()
                            .EnableApiVersioningHeader();
        return new ElasticClient(settings);
    }

    public static void UpdateMappings(List<MappingLuceneItem> mappings)
    {
        var client = GetClient();
        client.IndexMany(mappings);
    }
}
