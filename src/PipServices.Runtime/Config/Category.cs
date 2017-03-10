namespace PipServices.Runtime.Config
{
    /// <summary>
    ///     Category of components or configuration sections that are used to configure components.
    /// </summary>
    public static class Category
    {
        /// <summary>
        ///     Undefined category
        /// </summary>
        public const string Undefined = "undefined";

        /// <summary>
        ///     Component factories
        /// </summary>
        public const string Factories = "factories";

        /// <summary>
        ///     Service discovery components
        /// </summary>
        public const string Discovery = "discovery";

        /// <summary>
        ///     Bootstrap configuration readers
        /// </summary>
        public const string Boot = "boot";

        /// <summary>
        ///     Logging components
        /// </summary>
        public const string Logs = "logs";

        /// <summary>
        ///     Performance counters
        /// </summary>
        public const string Counters = "counters";

        /// <summary>
        ///     Value caches
        /// </summary>
        public const string Cache = "cache";

        /// <summary>
        ///     Persistence components
        /// </summary>
        public const string Persistence = "persistence";

        /// <summary>
        ///     Clients to other microservices or infrastructure services
        /// </summary>
        public const string Clients = "clients";

        /// <summary>
        ///     Any business logic component - controller or decorator
        /// </summary>
        public const string BusinessLogic = "logic";

        /// <summary>
        ///     Business logic controllers
        /// </summary>
        public const string Controllers = "controllers";

        /// <summary>
        ///     Decorators to business logic controllers
        /// </summary>
        public const string Decorators = "decorators";

        /// <summary>
        ///     API services
        /// </summary>
        public const string Services = "services";

        /// <summary>
        ///     Various microservice addons / extension components
        /// </summary>
        public const string Addons = "addons";
    }
}