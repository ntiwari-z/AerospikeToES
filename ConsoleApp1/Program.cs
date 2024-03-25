namespace Utility
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            bool success;
            int startCount = 0;
            do
            {
                (success, startCount) = await Aerospike.Start(startCount, "sabre");
                if (!success)
                    Console.WriteLine("retry");
            }
            while (!success);
            Console.WriteLine("Done!");
        }
    }
}