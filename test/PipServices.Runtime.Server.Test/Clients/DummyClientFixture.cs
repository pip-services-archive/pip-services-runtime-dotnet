using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Clients;
using PipServices.Dummy.Data;

namespace PipServices.Runtime.Clients
{
    public class DummyClientFixture
    {
        private readonly DummyObject DUMMY1 = new DummyObject
        {
            Key = "Key 1",
            Content = "Content 1"
        };

        private readonly DummyObject DUMMY2 = new DummyObject
        {
            Key = "Key 2",
            Content = "Content 2"
        };

        private readonly IDummyClient _client;

        public DummyClientFixture(IDummyClient client)
        {
            Assert.IsNotNull(client);
            _client = client;
        }

        public async Task TestCrudOperations(CancellationToken cancellationToken)
        {
            // Create one dummy
            var dummy1 = await _client.CreateDummyAsync(null, DUMMY1, cancellationToken);

            Assert.IsNotNull(dummy1);
            Assert.IsNotNull(dummy1.Id);
            Assert.AreEqual(DUMMY1.Key, dummy1.Key);
            Assert.AreEqual(DUMMY1.Content, dummy1.Content);

            // Create another dummy
            var dummy2 = await _client.CreateDummyAsync(null, DUMMY2, cancellationToken);

            Assert.IsNotNull(dummy2);
            Assert.IsNotNull(dummy2.Id);
            Assert.AreEqual(DUMMY2.Key, dummy2.Key);
            Assert.AreEqual(DUMMY2.Content, dummy2.Content);

            // Get all dummies
            var dummies = await _client.GetDummiesAsync(null, null, null, cancellationToken);
            Assert.IsNotNull(dummies);
            Assert.AreEqual(2, dummies.Data.Count());

            // Update the dummy
            dummy1.Content = "Updated Content 1";
            var dummy = await _client.UpdateDummyAsync(
                null,
                dummy1.Id,
                dummy1,
                cancellationToken
                );

            Assert.IsNotNull(dummy);
            Assert.AreEqual(dummy1.Id, dummy.Id);
            Assert.AreEqual(dummy1.Key, dummy.Key);
            Assert.AreEqual("Updated Content 1", dummy.Content);

            // Delete the dummy
            await _client.DeleteDummyAsync(null, dummy1.Id, cancellationToken);

            // Try to get deleted dummy
            dummy = await _client.GetDummyByIdAsync(null, dummy1.Id, cancellationToken);
            Assert.IsNull(dummy);
        }

        public async Task TestMultithreading(CancellationToken cancellationToken)
        {
            var dummies = new List<DummyObject>();

            for (var i = 0; i < 100; i++)
            {
                dummies.Add(new DummyObject() {Key = "Key " + i, Content = "Content " + i});
            }

            var count = 0;
            dummies.AsParallel().ForAll(async x =>
            {
                var dummy = await _client.CreateDummyAsync(null, x, cancellationToken);
                Interlocked.Increment(ref count);
            });

            while (count < 100)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
            }

            var dummiesResponce = await _client.GetDummiesAsync(null, null, null, cancellationToken);
            Assert.IsNotNull(dummies);
            Assert.AreEqual(100, dummiesResponce.Data.Count());
        }
    }
}