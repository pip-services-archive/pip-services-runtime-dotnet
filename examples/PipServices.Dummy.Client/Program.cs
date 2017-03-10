using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Runtime.Config;

namespace PipServices.Dummy.Clients
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Config = ComponentConfig.FromTuples(
                "endpoint.protocol", "http",
                "endpoint.host", "localhost",
                "endpoint.port", 3000
                );

            var client = new DummyWcfClient(Config);
            client.Open();

            var task = TestMethods(client);
            task.Wait();
        }

        private static async Task TestMethods(DummyWcfClient client)
        {
            var dummies = await client.GetDummiesAsync(null, null, null, CancellationToken.None);

            var dummy1 = new DummyObject
            {
                Key = "123",
                Content = "ABC"
            };
            var dummy2 = await client.CreateDummyAsync(null, dummy1, CancellationToken.None);

            var dummy3 = await client.GetDummyByIdAsync(null, dummy2.Id, CancellationToken.None);

            dummy3.Content = "Updated Content ABC";
            var dummy4 = await client.UpdateDummyAsync(null, dummy3.Id, dummy3, CancellationToken.None);

            await client.DeleteDummyAsync(null, dummy4.Id, CancellationToken.None);
        }
    }
}