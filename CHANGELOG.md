<a name="1.1.0"></a>
### 1.1.0 (2016-08-21)

#### Features
* **Events** - Added support for asyncronous events
* **Context** - Added application context to share references across all components in container

#### Breaking Changes
* Link method now takes context and components parameters

<a name="1.0.0"></a>
### 1.0.0 (2016-06-25)

#### Features
* **Commands** - Command pattern implementation for business logic components
* **Validation** - Data validation schemas
* **Direct Client** - Direct clients for in-process calls
* **Component Descriptor** - Descriptor mechanism to identify and locate components and their factories
* **Config** - New, more structured microservice configuration

#### Breaking Changes
* Complete rewamp of original ad-hoc design
* New package structure. Added packages: data, config, errors, boot, commands, validation
* Config components were renamed to BootConfig 
* Util package was renamed to Portability. REST utilities were moved under Services
* Build process entirely reimplemented

<a name="0.0.1"></a>
### 0.0.1 (2016-06-07)

#### Features
* **build** Abstract component definition, lifecycle management and microservice build (composition)
* **config** Microservice configuration using direct object and plain json files
* **log** Logging with null and console output
* **counters** Performance counters with null and log output
* **db** Data access via file and mongodb
* **deps** Client dependencies via seneca and REST
* **api** API services vis seneca and REST
* **unit tests** Unit tests using Mocha and Matcha

#### Breaking Changes
No breaking changes since this is the first version

#### Bug Fixes
No fixes in this version

