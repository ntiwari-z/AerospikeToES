using Nest;

public class ElasticSearchData
{
    private static readonly ElasticClient client;
    static ElasticSearchData()
    {
        var settings = new ConnectionSettings(new Uri("https://es-content.us.prod.zentrumhub.com:9200")).DefaultIndex("hotel-mapping")
                            .BasicAuthentication("app", "zentrumxsw2XSW@")
                            .MemoryStreamFactory(Elasticsearch.Net.RecyclableMemoryStreamFactory.Default)
                            .DisableDirectStreaming()
                            .EnableApiVersioningHeader();
        client = new ElasticClient(settings);
    }
    private static ElasticClient GetClient()
    {
        var settings = new ConnectionSettings(new Uri("https://es-content.us.prod.zentrumhub.com:9200")).DefaultIndex("hotel-mapping")
                            .BasicAuthentication("app", "zentrumxsw2XSW@")
                            .MemoryStreamFactory(Elasticsearch.Net.RecyclableMemoryStreamFactory.Default)
                            .DisableDirectStreaming()
                            .EnableApiVersioningHeader();
        return new ElasticClient(settings);


    }

    //public static async Task UpdateMappings(Hotel hotel)
    //{
    //    var id = hotel.Id;
    //    var response = await client.GetAsync<MappingLuceneItem>(id);

    //    var mappings = response?.Source;

    //    MappingLuceneItem luceneItem = TranslateToLuceneItem(hotel);

    //    //New Hotel ID
    //    if (mappings == null)
    //    {
    //        await client.IndexManyAsync(new List<MappingLuceneItem> { luceneItem });
    //    }
    //    else//HotelID exists
    //    {
    //        //Check - if ProviderData exists for HotelID
    //        var exists = mappings.Data?.Any(x => x.Pm.Equals(hotel.ProviderName, StringComparison.OrdinalIgnoreCase));

    //        var newMapping = luceneItem.Data.FirstOrDefault(x => x.Pm.Equals(hotel.ProviderName, StringComparison.OrdinalIgnoreCase));
    //        //Update ProviderData
    //        if (exists == true)
    //        {
    //            return;
    //            //Check - if ProviderHotelId is changed, then update - No need of this check, we will update all details as per Vervotech data
    //            //if (!mappings.Data.Any(x => x.Pm.Equals(hotel.ProviderName, StringComparison.OrdinalIgnoreCase)
    //            //                            && x.Phi.Equals(hotel.ProviderHotelId, StringComparison.OrdinalIgnoreCase)))
    //            //{
    //            //foreach (var mapping in mappings.Data)
    //            //{
    //            //    if (mapping.Pm.Equals(hotel.ProviderName, StringComparison.OrdinalIgnoreCase))
    //            //    {
    //            //        mapping.Phi = newMapping.Phi;
    //            //        mapping.Cn = newMapping.Cn;
    //            //        mapping.Sc = newMapping.Sc;
    //            //        mapping.Cc = newMapping.Cc;
    //            //        mapping.Chc = newMapping.Chc;
    //            //        mapping.Chn = newMapping.Chn;
    //            //        mapping.Sr = newMapping.Sr;
    //            //        mapping.D = newMapping.D;
    //            //        mapping.Dc = newMapping.Dc;
    //            //        mapping.Sn = newMapping.Sn;
    //            //        mapping.Pgc = newMapping.Pgc;
    //            //        mapping.Lut = DateTime.UtcNow;
    //            //        break;
    //            //    }
    //            //}

    //            //await client.IndexManyAsync(new List<MappingLuceneItem> { mappings });
    //            //}
    //        }
    //        else//Add New ProviderData
    //        {
    //            mappings.Data.Add(newMapping);
    //            await client.IndexAsync(mappings, null);
    //        }
    //    }
    //}

    public static async Task<bool> UpdateMappings(List<MappingLuceneItem> mappings)
    {
        var response = await client.IndexManyAsync(mappings);

        if (response != null)
        {
            return response.IsValid; 
        }
        return false;
    }

    public static async Task<List<MappingLuceneItem>> GetMappings(List<Hotel> hotels)
    {
        var mappingsData = new List<MappingLuceneItem>();
        var client = GetClient();

        foreach (var item in hotels)
        {
            var response = await client.GetAsync<MappingLuceneItem>(item.Id);

            var mappings = response?.Source;
            if (mappings != null)
            {
                var exists = mappings.Data?.Any(x => x.Pm.Equals(item.ProviderName, StringComparison.OrdinalIgnoreCase));

                if (exists == false)
                {
                    MappingLuceneItem luceneItem = TranslateToLuceneItem(item);
                    mappings.Data.Add(luceneItem.Data[0]);
                    mappingsData.Add(mappings);
                }
            }
        }

        return mappingsData;
    }

    public static async Task<MappingLuceneItem> GetMapping(Hotel hotel)
    {

        var response = await client.GetAsync<MappingLuceneItem>(hotel.Id);

        var mappings = response?.Source;
        if (mappings != null && mappings.Data?.Any(x => x.Pm.Equals(hotel.ProviderName, StringComparison.OrdinalIgnoreCase)) == false)
        {
            MappingLuceneItem luceneItem = TranslateToLuceneItem(hotel);
            mappings.Data.Add(luceneItem.Data[0]);
            return mappings;
        }

        return null;
    }

    public static MappingLuceneItem TranslateToLuceneItem(Hotel hotel)
    {
        try
        {
            return new MappingLuceneItem
            {
                Id = hotel.Id,
                Data = new List<Datum>
                    {
                       new Datum
                       {
                            Phi = hotel.ProviderHotelId,
                            Pm =  hotel.ProviderName,
                            Dc =  hotel.Contact.Address.DestinationCode,
                            Cn =  hotel.Contact.Address.City.Name,
                            Sc =  hotel.Contact.Address.State.Code,
                            Sn =  hotel.Contact.Address.State.Name,
                            Cc =  hotel.Contact.Address.Country.Code,
                            Chc = hotel.ChainCode,
                            Chn = hotel.ChainName,
                            Sr =  hotel.StarRating,
                            Pgc = new MappingGeoCode
                            {
                                Lat = hotel.GeoCode.Lat,
                                Lon = hotel.GeoCode.Long
                            },
                            Lut = DateTime.UtcNow
                       }
                    }
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

}
