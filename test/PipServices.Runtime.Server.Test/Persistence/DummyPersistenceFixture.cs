using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PipServices.Dummy.Data;
using PipServices.Dummy.Persistence;

namespace PipServices.Runtime.Persistence
{
    public class DummyPersistenceFixture
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

        private readonly IDummyPersistence _db;

        public DummyPersistenceFixture(IDummyPersistence db)
        {
            Assert.IsNotNull(db);
            _db = db;
        }

        public async Task TestCrudOperations(CancellationToken cancellationToken)
        {
            // Create one dummy
            var dummy1 = await _db.CreateDummyAsync(null, DUMMY1, cancellationToken);

            Assert.IsNotNull(dummy1);
            Assert.IsNotNull(dummy1.Id);
            Assert.AreEqual(DUMMY1.Key, dummy1.Key);
            Assert.AreEqual(DUMMY1.Content, dummy1.Content);

            // Create another dummy
            var dummy2 = await _db.CreateDummyAsync(null, DUMMY2, cancellationToken);

            Assert.IsNotNull(dummy2);
            Assert.IsNotNull(dummy2.Id);
            Assert.AreEqual(DUMMY2.Key, dummy2.Key);
            Assert.AreEqual(DUMMY2.Content, dummy2.Content);

            // Get all dummies
            var dummies = await _db.GetDummiesAsync(null, null, null, cancellationToken);
            Assert.IsNotNull(dummies);
            Assert.AreEqual(2, dummies.Data.Count());

            // Update the dummy
            var dummy = await _db.UpdateDummyAsync(
                null,
                dummy1.Id,
                new
                {
                    content = "Updated Content 1"
                },
                cancellationToken
                );

            Assert.IsNotNull(dummy);
            Assert.AreEqual(dummy1.Id, dummy.Id);
            Assert.AreEqual(dummy1.Key, dummy.Key);
            Assert.AreEqual("Updated Content 1", dummy.Content);

            // Delete the dummy
            await _db.DeleteDummyAsync(null, dummy1.Id, cancellationToken);

            // Try to get deleted dummy
            dummy = await _db.GetDummyByIdAsync(null, dummy1.Id, cancellationToken);
            Assert.IsNull(dummy);
        }

        public async Task TestMultithreading(CancellationToken cancellationToken)
        {
            const int itemNumber = 50;

            var dummies = new List<DummyObject>();

            for (var i = 0; i < itemNumber; i++)
            {
                dummies.Add(new DummyObject() { Id = i.ToString(), Key = "Key " + i, Content = "Content " + i });
            }

            var count = 0;
            dummies.AsParallel().ForAll(async x =>
            {
                await _db.CreateDummyAsync(null, x, cancellationToken);
                Interlocked.Increment(ref count);
            });

            while (count < itemNumber)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
            }

            var dummiesResponce = await _db.GetDummiesAsync(null, null, null, cancellationToken);
            Assert.IsNotNull(dummies);
            Assert.AreEqual(itemNumber, dummiesResponce.Data.Count());
            Assert.AreEqual(itemNumber, dummiesResponce.Total);

            dummies.AsParallel().ForAll(async x =>
            {
                var updatedContent = "Updated Content " + x.Id;

                // Update the dummy
                var dummy = await _db.UpdateDummyAsync(
                    null,
                    x.Id,
                    new
                    {
                        content = updatedContent
                    },
                    cancellationToken
                    );

                Assert.IsNotNull(dummy);
                Assert.AreEqual(dummy.Id, dummy.Id);
                Assert.AreEqual(dummy.Key, dummy.Key);
                Assert.AreEqual(updatedContent, dummy.Content);
            });

            count = 0;
            dummies.AsParallel().ForAll(async x =>
            {
                // Delete the dummy
                await _db.DeleteDummyAsync(null, x.Id, cancellationToken);

                // Try to get deleted dummy
                var dummy = await _db.GetDummyByIdAsync(null, x.Id, cancellationToken);
                Assert.IsNull(dummy);

                Interlocked.Increment(ref count);
            });

            while (count < itemNumber)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken);
            }

            dummiesResponce = await _db.GetDummiesAsync(null, null, null, cancellationToken);
            Assert.IsNotNull(dummies);
            Assert.AreEqual(0, dummiesResponce.Data.Count());
            Assert.AreEqual(0, dummiesResponce.Total);
        }
    }
}