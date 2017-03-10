using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Persistence
{
    public abstract class FilePersistence<T> : AbstractPersistence where T : IIdentifiable
    {
        private DynamicMap DefaultConfig { get; } = DynamicMap.FromTuples(
            "options.max_page_size", 100
            );

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        //private readonly object _syncObject = new object();

        protected IEnumerable<T> InitialData;
        protected ImmutableList<T> Items;
        protected int MaxPageSize;

        protected string Path;

        private Random _random;

        protected FilePersistence(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        public override void Configure(ComponentConfig config)
        {
            CheckNewStateAllowed(State.Configured);

            config = config.WithDefaults(DefaultConfig);
            var options = config.Options;

            if (options.HasNot("path"))
                throw new ConfigError(this, "NoPath", "Data file path is not set");

            base.Configure(config);

            Path = options.GetString("path");
            MaxPageSize = options.GetInteger("max_page_size");
            InitialData = (IEnumerable<T>) options.Get("data");
        }

        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            if (InitialData != null)
            {
                _lock.EnterWriteLock();

                try
                {
                    Items = ImmutableList.CreateRange(InitialData);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            else Load();

            base.Open();
        }

        public virtual async Task OpenAsync(CancellationToken cancellationToken)
        {
            CheckNewStateAllowed(State.Opened);

            if (InitialData != null)
            {
                _lock.EnterWriteLock();

                try
                {
                    Items = ImmutableList.CreateRange(InitialData);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            else await LoadAsync(cancellationToken);

            base.Open();
        }

        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            Save();

            base.Close();
        }

        public virtual async Task CloseAsync(CancellationToken cancellationToken)
        {
            CheckNewStateAllowed(State.Closed);

            await Task.Run(() => Save(), cancellationToken);

            base.Close();
        }

        public virtual void Load()
        {
            Trace(null, "Loading data from file at " + Path);

            _lock.EnterWriteLock();

            try
            {
                // If doesn't exist then consider empty data
                if (!File.Exists(Path))
                {
                    Items = ImmutableList.Create<T>();
                    return;
                }

                using (var reader = new StreamReader(Path))
                {
                    var json = reader.ReadToEnd();
                    Items = ImmutableList.CreateRange(JsonConvert.DeserializeObject<T[]>(json)) ?? ImmutableList.Create<T>();
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public virtual async Task LoadAsync(CancellationToken cancellationToken)
        {
            Trace(null, "Loading data from file at " + Path);

            _lock.EnterWriteLock();

            try
            {
                // If doesn't exist then consider empty data
                if (!File.Exists(Path))
                {
                    Items = ImmutableList.Create<T>();
                    return;
                }

                using (var reader = new StreamReader(Path))
                {
                    var json = await reader.ReadToEndAsync();
                    Items = ImmutableList.CreateRange(JsonConvert.DeserializeObject<T[]>(json)) ?? ImmutableList.Create<T>();
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public virtual void Save()
        {
            Trace(null, "Saving data to file at " + Path);

            _lock.EnterWriteLock();

            try
            {
                using (var writer = new StreamWriter(Path))
                {
                    var json = JsonConvert.SerializeObject(Items.ToArray(), Formatting.Indented);
                    writer.Write(json);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public override void Clear()
        {
            _lock.EnterWriteLock();

            try
            {
                Items = ImmutableList.Create<T>();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            Save();
        }

        public virtual async Task ClearAsync(CancellationToken cancellationToken)
        {
            _lock.EnterWriteLock();

            try
            {
                Items = ImmutableList.Create<T>();
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await Task.Run(() => Save(), cancellationToken);
        }


        public DataPage<T> GetPage(string correlationId, Func<T, bool> filter, PagingParams paging, Func<T, object> sort)
        {
            _lock.EnterReadLock();

            try
            {
                IEnumerable<T> items = Items.ToArray();

                // Apply filter
                if (filter != null)
                    items = items.Where(filter).ToArray();

                // Extract a page
                paging = paging ?? new PagingParams();
                var skip = paging.GetSkip(-1);
                var take = paging.GetTake(MaxPageSize);

                var total = items.Count();
                if (skip > 0)
                    items = items.Skip(skip);
                items = items.Take(take);

                // Apply sorting
                if (sort != null)
                    items = items.OrderBy(sort);

                return new DataPage<T>(total, items);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public DataPage<TS> GetPage<TS>(string correlationId, Func<T, bool> filter, PagingParams paging,
            Func<T, object> sort, Func<T, TS> select)
        {
            var page = GetPage(correlationId, filter, paging, sort);
            var total = page.Total;
            var items = page.Data.Select(select);

            return new DataPage<TS>(total, items);
        }

        public List<T> GetList(string correlationId, Func<T, bool> filter, Func<T, object> sort)
        {
            _lock.EnterReadLock();

            try
            {
                IEnumerable<T> items = Items.ToArray();

                // Apply filter
                if (filter != null)
                    items = items.Where(filter);

                // Apply sorting
                if (sort != null)
                    items = items.OrderBy(sort);

                return items.ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<TS> GetList<TS>(string correlationId, Func<T, bool> filter, Func<T, object> sort, Func<T, TS> select)
        {
            var items = GetList(correlationId, filter, sort).Select(select);
            return items.ToList();
        }

        protected async Task<T> GetByIdAsync(string correlationId, string id, CancellationToken cancellationToken)
        {
            var item = await Task.Run(() =>
            {
                _lock.EnterReadLock();

                try
                {
                    return Items.Find(v => v.Id == id);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }, cancellationToken);

            return item;
        }

        protected T GetRandom(string correlationId)
        {
            _lock.EnterReadLock();

            try
            {
                if (Items.Count == 0)
                    return default(T);

                if (_random == null)
                    _random = new Random();

                var item = Items[_random.Next(0, Items.Count)];

                return item;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        protected async Task<T> CreateAsync(string correlationId, T item, CancellationToken cancellationToken)
        {
            _lock.EnterWriteLock();

            try
            {
                item.Id = item.Id ?? CreateUuid();
                Items = Items.Add(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await Task.Run(() => Save(), cancellationToken);

            return item;
        }

        protected async Task<T> ReplaceAsync(string correlationId, string id, T newItem, CancellationToken cancellationToken)
        {
            _lock.EnterUpgradeableReadLock();

            try
            {
                var index = Items.FindIndex(v => v.Id == id);
                if (index < 0)
                    return default(T);

                newItem.Id = id;

                _lock.EnterWriteLock();

                try
                {
                    Items = Items.Replace(Items[index], newItem);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            await Task.Run(() => Save(), cancellationToken);

            return newItem;
        }

        protected async Task<T> UpdateAsync(string correlationId, string id, DynamicMap newValues, CancellationToken cancellationToken)
        {
            _lock.EnterWriteLock();

            T item;

            try
            {
                item = Items.Find(v => v.Id == id);
                if (item == null)
                    return default(T);

                newValues.AssignTo(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            await Task.Run(() => Save(), cancellationToken);

            return item;
        }

        protected async Task<T> UpdateAsync(string correlationId, string id, object newValues, CancellationToken cancellationToken)
        {
            return await UpdateAsync(correlationId, id, Converter.ToNullableMap(newValues), cancellationToken);
        }

        protected async Task<T> DeleteAsync(string correlationId, string id, CancellationToken cancellationToken)
        {
            _lock.EnterUpgradeableReadLock();

            T item;

            try
            {
                var index = Items.FindIndex(v => v.Id == id);
                if (index < 0) return default(T);

                item = Items[index];

                _lock.EnterWriteLock();

                try
                {
                    Items = Items.RemoveAt(index);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            await Task.Run(() => Save(), cancellationToken);

            return item;
        }
    }
}