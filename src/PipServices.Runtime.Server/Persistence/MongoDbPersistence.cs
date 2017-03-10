using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Persistence
{
    public abstract class MongoDbPersistence<TE> : AbstractPersistence
        where TE : IIdentifiable
    {
        private readonly string _collectionName;

        protected MongoDbPersistence(string collectionName, ComponentDescriptor descriptor)
            : base(descriptor)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            _collectionName = collectionName;
        }

        public static DynamicMap DefaultConfig { get; } = DynamicMap.FromTuples(
            "connection.host", "localhost",
            "connection.port", 27017,
            "options.server.pollSize", 4,
            "options.server.socketOptions.keepAlive", 1,
            "options.server.socketOptions.connectTimeoutMS", 5000,
            "options.server.auto_reconnect", true,
            "options.max_page_size", 100,
            "options.debug", true
            );

        protected MongoClient Connection { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public IMongoCollection<TE> Collection { get; private set; }
        protected int MaxPageSize { get; private set; }

        public override void Configure(ComponentConfig config)
        {
            CheckNewStateAllowed(State.Configured);

            config = config.WithDefaults(DefaultConfig);
            var connection = config.Connection;
            var options = config.Options;

            if (connection == null)
                throw new ConfigError(this, "NoConnection", "Database connection is not set");

            if (connection.Type != "mongodb")
                throw new ConfigError(this, "WrongConnectionType", "MongoDb is the only supported connection type");

            if (connection.Host == null)
                throw new ConfigError(this, "NoConnectionHost", "Connection host is not set");

            if (connection.Port == 0)
                throw new ConfigError(this, "NoConnectionPort", "Connection port is not set");

            if (connection.Database == null)
                throw new ConfigError(this, "NoConnectionDatabase", "Connection database is not set");

            base.Configure(config);

            MaxPageSize = options.GetInteger("max_page_size");
        }

        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            var connection = _config.Connection;
            var uri = connection.Uri;
            var options = _config.Options;

            Trace(null, "Connecting to mongodb at " + uri);

            //var mongoUrl = new MongoUrl(uri);
            //var databaseName = mongoUrl.DatabaseName;

            if (string.IsNullOrWhiteSpace(connection.Database))
                throw new ConfigError(this, "NoDatabaseName", "Database name is not specified in MongoDb connection");

            try
            {
                //var settings = MongoClientSettings.FromUrl(mongoUrl);
                var settings = new MongoClientSettings();

                settings.Server = new MongoServerAddress(connection.Host, connection.Port);

                settings.MaxConnectionPoolSize = options.GetInteger("server.pollSize");
                settings.ConnectTimeout =
                    new TimeSpan(options.GetInteger("server.socketOptions.connectTimeoutMS")*
                                 TimeSpan.TicksPerMillisecond);
                settings.SocketTimeout =
                    new TimeSpan(options.GetInteger("server.socketOptions.socketTimeoutMS")*TimeSpan.TicksPerMillisecond);
                settings.SocketTimeout =
                    new TimeSpan(options.GetInteger("server.socketOptions.socketTimeoutMS")*TimeSpan.TicksPerMillisecond);

                Connection = new MongoClient(settings);
                Database = Connection.GetDatabase(connection.Database);
                Collection = Database.GetCollection<TE>(_collectionName);

                base.Open();
            }
            catch (Exception)
            {
                throw new ConnectionError(this, "ConnectFailed", "Connection to mongodb failed");
            }
        }

        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            base.Close();
        }

        public override void Clear()
        {
            Database.DropCollectionAsync(_collectionName);
        }

        public virtual async Task<DataPage<TE>> GetPageAsync<TT>(string correlationId, FilterParams filter,
            PagingParams paging, Func<TE, object> sort, CancellationToken cancellationToken) where TT : Terms<TE>, new()
        {
            if (paging == null)
                paging = new PagingParams();

            var termsFilter = new TT();
            var filterDef = termsFilter.GetFilterDefinition(filter);
            var finder = Collection.Find(filterDef);
            finder = paging.ModifyFinder(finder);

            var result = await finder.ToListAsync(cancellationToken);

            var totalCount = await Collection.CountAsync(new BsonDocument(), null, cancellationToken);

            // Apply sorting
            if (sort != null)
                result = result.OrderBy(sort).ToList();

            return new DataPage<TE>((int) totalCount, result);
        }

        public virtual async Task<DataPage<TE>> GetPageAsync(string correlationId, PagingParams paging,
            Func<TE, object> sort,
            CancellationToken cancellationToken)
        {
            if (paging == null)
                paging = new PagingParams();

            var filterDef = new BsonDocument();
            var finder = Collection.Find(filterDef);
            finder = paging.ModifyFinder(finder);

            var result = await finder.ToListAsync(cancellationToken);

            var totalCount = await Collection.CountAsync(new BsonDocument(), null, cancellationToken);

            // Apply sorting
            if (sort != null)
                result = result.OrderBy(sort).ToList();

            return new DataPage<TE>((int) totalCount, result);
        }

        public virtual async Task<DataPage<TS>> GetPageAsync<TS, TT>(string correlationId, FilterParams filter,
            PagingParams paging,
            Func<TE, object> sort,
            Func<TE, TS> select, CancellationToken cancellationToken)
            where TT : Terms<TE>, new()
        {
            var page = await GetPageAsync<TT>(correlationId, filter, paging, sort, cancellationToken);
            var total = page.Total;
            var items = page.Data.Select(select);

            return new DataPage<TS>(total, items);
        }

        public virtual async Task<IEnumerable<TE>> GetListAsync<TT>(string correlationId, FilterParams filter,
            Func<TE, object> sort,
            CancellationToken cancellationToken)
            where TT : Terms<TE>, new()
        {
            var termsFilter = new TT();
            var filterDef = termsFilter.GetFilterDefinition(filter);
            var finder = Collection.Find(filterDef);

            var result = await finder.ToListAsync(cancellationToken);

            // Apply sorting
            if (sort != null)
                result = result.OrderBy(sort).ToList();

            return result.ToArray();
        }

        public virtual async Task<IEnumerable<TS>> GetListAsync<TS, TT>(string correlationId, FilterParams filter,
            Func<TE, object> sort,
            Func<TE, TS> select, CancellationToken cancellationToken)
            where TT : Terms<TE>, new()
        {
            var items = await GetListAsync<TT>(correlationId, filter, sort, cancellationToken);
            var projectedItems = items.Select(select);
            return projectedItems.ToArray();
        }

        public virtual async Task<TE> GetRandomAsync(string correlationId, CancellationToken cancellationToken)
        {
            var totalCount = await Collection.CountAsync(new BsonDocument(), null, cancellationToken);
            if (totalCount == 0)
                return default(TE);

            var random = new Random();
            var paging = new PagingParams(random.Next(0, (int) totalCount), 1, true);

            var items = await GetPageAsync(correlationId, paging, null, cancellationToken);
            var item = items.Data.FirstOrDefault();

            return item;
        }

        public virtual async Task<TE> GetByIdAsync(string correlationId, string id, CancellationToken cancellationToken)
        {
            var builder = Builders<TE>.Filter;

            var filter = builder.Eq(x => x.Id, id);

            return await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TE> CreateAsync(string correlationId, TE item, CancellationToken cancellationToken)
        {
            item.Id = item.Id ?? CreateUuid();

            await Collection.InsertOneAsync(item, null, cancellationToken);

            return item;
        }

        public virtual async Task<TE> UpdateAsync(string correlationId, string id, DynamicMap newValues,
            CancellationToken cancellationToken)
        {
            var item = await GetByIdAsync(correlationId, id, cancellationToken);
            if (item == null)
                return default(TE);

            newValues.AssignTo(item);

            var filter = Builders<TE>.Filter.Eq(x => x.Id, id);

            var options = new FindOneAndReplaceOptions<TE>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = false
            };

            item = await Collection.FindOneAndReplaceAsync(filter, item, options, cancellationToken);

            return item;
        }

        public virtual async Task<TE> UpdateAsync(string correlationId, string id, object newValues,
            CancellationToken cancellationToken)
        {
            return await UpdateAsync(correlationId, id, Converter.ToNullableMap(newValues), cancellationToken);
        }

        public virtual async Task<TE> DeleteAsync(string correlationId, string id, CancellationToken cancellationToken)
        {
            var filter = Builders<TE>.Filter.Eq(x => x.Id, id);

            var options = new FindOneAndDeleteOptions<TE>();

            return await Collection.FindOneAndDeleteAsync(filter, options, cancellationToken);
        }
    }
}