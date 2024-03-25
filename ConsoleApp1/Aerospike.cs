using Aerospike.Client;
using System.Collections.Concurrent;

namespace Utility
{
    public static class Aerospike
    {

        private readonly static MsgPackSerializer _serializer = new();

        //public static void Start()
        //{
        //    var client = new AerospikeClient("ip", 3000);

        //    string namespaceName = "ns";
        //    string setName = "set";

        //    // Create a delegate for the scan callback
        //    ScanCallback scanCallback = Callback;

        //    // Create a Scan object with a callback
        //    client.ScanAll(null, namespaceName, setName, scanCallback);

        //    // Close the Aerospike client
        //    client.Close();
        //}

        public static async Task<(bool, int)> Start(int startCount, string providerName)
        {
            var client = new AerospikeClient("52.90.201.155", 3000);
            int total = 0;

            try
            {

                string namespaceName = "content";
                string setName = "hotelcontent";
                var statement = new Statement();
                statement.SetNamespace(namespaceName);
                statement.SetSetName(setName);
                statement.IndexName = "providerindex";
                statement.BinNames = new string[] { "id", "provider", "basiccontent" };
                statement.SetFilter(Filter.Equal("provider", providerName));

                var tasks = new List<Task>();
                var mappingsData = new ConcurrentBag<MappingLuceneItem>();
                int count = 0;
                int batch = 1000;
                using (var result = client.Query(null, statement))
                {
                    while (result.Next())
                    {
                        total ++;
                        if (total <= (startCount - (batch * 5))) { continue; }
                        var record = result.Record;
                        var task = Task.Run(async () =>
                        {
                            if (record != null)
                            {
                                Hotel hotel = _serializer.DeSerialize<Hotel>((byte[])record.bins["basiccontent"]);

                                if (hotel != null)
                                {
                                    var mappings = await ElasticSearchData.GetMapping(hotel);
                                    if (mappings?.Data.Count > 0)
                                    {
                                        mappingsData.Add(mappings);
                                    }
                                }
                            }
                        });
                        tasks.Add(task);

                        if (tasks.Count == batch)
                        {
                            await Task.WhenAll(tasks);
                            if (!mappingsData.IsEmpty)
                            {
                                await ElasticSearchData.UpdateMappings(mappingsData.ToList());

                                count += mappingsData.Count;
                                Console.WriteLine("Added: " + count);
                            }

                            Console.WriteLine("Total: " + total);

                            tasks.Clear();
                            mappingsData.Clear();
                        }
                    }
                }
                // Close the Aerospike client
                client.Close();
            }
            catch (Exception)
            {
                return (false, total);
            }
            finally
            {
                // Close the Aerospike client
                client.Close();
            }
            return (true, 0);
        }

        //private static void Callback(Key key, Record record)
        //{
        //    try
        //    {
        //        Hotel hotel = _serializer.DeSerialize<Hotel>((byte[])record.bins["basiccontent"]);

        //        ElasticSearchData.UpdateMappings(new List<MappingLuceneItem> { TranslateToLuceneItem(hotel) });
        //    }
        //    catch (Exception)
        //    {
        //        //supress
        //    }
        //}
    }
}