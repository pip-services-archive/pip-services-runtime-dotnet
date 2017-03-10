# <img src="https://github.com/pip-services/pip-services/blob/master/design/Logo.png" alt="Pip.Services Logo" style="max-width:30%"> <br/> Microservices runtime for .NET

This framework is a part of [Pip.Services](https://github.com/pip-services/pip-services) project.
It provides foundation for **composable microservices** using .NET technology stack.
Each microservice is constructed from components created following 
[user-defined configuration](https://github.com/pip-services/pip-services/blob/master/usage/Configuration.md). 

The current implementation is **experimental** and not ready for production. Pip.Services team welcomes interested
individuals and groups to bring it to ready state.

The conceptual architecture of the framework is presented on the diagram below. For more details , please, refer to 
[Pip.Services Reference Architecture](https://github.com/pip-services/pip-services/blob/master/design/Architecture.md) document.

<div style="border: 1px solid #ccc">
  <img src="https://github.com/pip-services/pip-services-runtime-python/blob/master/doc/Overview.png" alt="Runtime Architecture" style="display:block;">
</div>

The main goal of the framework is to enable flexible microservice structure that can adjust based on the
user requirements. It allows to run microservices on virtually any platform, with any infrastructural services and storages
while protecting core business logic from rework due to changes in technologies and deployment configurations.

All functionality of the runtime is split into several packages:

<div style="border: 1px solid #ccc">
  <img src="https://github.com/pip-services/pip-services-runtime-python/blob/master/doc/Packages.png" alt="Runtime Packages" style="display:block;">
</div>

- **Core** - key microservice abstractions and components
- **Portability** - cross-language and cross-platform abstractions
- **Errors** - microservice error definitions
- **Config** - microservice configuration
- **Data** - data handling patterns
- **Validation** - data validation
- **Commands** - command pattern
- **Build** - microservice component factories and builder
- **Run** - microservice container, runners and wrappers
- **Discovery** - discovery components: Null, Memory, File, REST, Etcd, Consul, ...
- **Boot** - bootstrap configuration readers: File, REST, Etcd, Consul, ...
- **Logs** - logger components: Null, Console, REST, Logstash, ...
- **Counters** - performance counters: Null, Log, REST, ...
- **Cache** - transient cache components: Null, Memory, Memcached, Redis, ...
- **Persistence** - abstract persistent components: Memory, File, MongoDB, Cassandra, MySQL, SQL Server, ...
- **Logic** - abstract business logic components: controllers and decorators
- **Clients** - abstract client components: Direct, REST, Seneca, Lambda, Thrift, ...
- **Services** - abstact service components: REST, Seneca, Thrift, ...
- **Addons** - abstract and specific addons: Seneca, Random Shutdown, Status, Node Registry, ... 

Quick Links:

* [Downloads](doc/Downloads.md)
* [Reference Architecture](https://github.com/pip-services/pip-services/blob/master/design/ReferenceArchitecture.md)
* [API Reference](http://htmlpreview.github.io/?https://github.com/pip-services/pip-services-runtime-node/blob/master/doc/api/index.html)
* [Configuration](https://github.com/pip-services/pip-services/blob/master/usage/Configuration.md)
* [Building and Testing](doc/Development.md)
* [Contributing](doc/Development.md/#contrib)

## Acknowledgements

The initial implementation is done by **Sergey Seroukhov**. Pip.Services team is looking for volunteers to 
take ownership over Python implementation in the project.
