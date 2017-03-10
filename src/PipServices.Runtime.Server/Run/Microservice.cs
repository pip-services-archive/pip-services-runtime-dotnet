using System.Collections.Generic;
using PipServices.Runtime.Build;
using PipServices.Runtime.Config;
using PipServices.Runtime.Logs;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Run
{
    public class Microservice
    {
        private ComponentSet _components;
        private readonly IComponentFactory _factory;
        private readonly string _name;
        private readonly DynamicMap _context = new DynamicMap();

        public Microservice(string name, IComponentFactory factory)
        {
            _factory = factory;
            _name = name;
        }

        public Microservice(IComponentFactory factory, MicroserviceConfig config)
        {
            _factory = factory;
            Config = config;
        }

        public MicroserviceConfig Config { get; set; }

        public DynamicMap Context => _context;

        public void LoadConfig(string path)
        {
            Config = ConfigReader.Read(path);
        }

        public void Fatal(params object[] message)
        {
            LogWriter.Fatal(
                _components.GetAllByCategory(Category.Logs),
                LogFormatter.Format(LogLevel.Fatal, message)
                );
        }

        public void Error(params object[] message)
        {
            LogWriter.Error(
                _components.GetAllByCategory(Category.Logs),
                LogFormatter.Format(LogLevel.Error, message)
                );
        }

        public void Info(string message, params object[] args)
        {
            Info((object)string.Format(message, args));
        }

        public void Info(params object[] message)
        {
            LogWriter.Info(
                _components.GetAllByCategory(Category.Logs),
                LogFormatter.Format(LogLevel.Info, message)
                );
        }

        public void Trace(params object[] message)
        {
            LogWriter.Trace(
                _components.GetAllByCategory(Category.Logs),
                LogFormatter.Format(LogLevel.Trace, message)
                );
        }

        private void Build()
        {
            _components = Builder.Build(_factory, Config);
        }

        private void Link()
        {
            Trace("Initializing " + _name + " microservice");
            LifeCycleManager.Link(_context, _components);
        }

        private void Open()
        {
            Trace("Opening " + _name + " microservice");
            LifeCycleManager.Open(_components);
            Info("Microservice " + _name + " started");
        }

        public void Start()
        {
            Build();
            Link();
            Open();
        }

        public void StartWithConfig(MicroserviceConfig config)
        {
            Config = config;
            Start();
        }

        public void StartWithConfigFile(string configPath)
        {
            LoadConfig(configPath);
            Start();
        }

        public void Stop()
        {
            Trace("Closing " + _name + " microservice");
            LifeCycleManager.ForceClose(_components);
            Info("Microservice " + _name + " closed");
        }

        public IEnumerable<IComponent> GetComponentByCategory(string category)
        {
            return _components.GetAllByCategory(category);
        }
    }
}