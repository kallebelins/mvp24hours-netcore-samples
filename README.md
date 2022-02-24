# mvp24hours-netcore-samples (v3.2.241)
Samples for quick product building using: Relational database (SQL Server, PostgreSql, MySql); NoSql database (MongoDb); Key-value database (Redis); Message Broker (RabbitMQ); Pipeline (Pipe and Filters pattern); Documentation (Swagger); Mapping (AutoMapper); Logging (NLog); Patterns for data validation (FluentValidation and Data Annotations), notification (Notification pattern) and specifications (Specification pattern), unit of work, repository, among others.
In each project there is a file with the description of the resources and implemented references. Read the "Readme.md" file in the "...WebAPI" folder.

## Study, share and contribute:
Visit: https://mvp24hours.dev/

## Templates for Visual Studio 2019 and 2022
Visit: https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/vstemplate

![Templates for Visual Studio 2019 and 2022](https://raw.githubusercontent.com/kallebelins/mvp24hours-netcore-samples/main/images/mvp24hours-netcore-samples-resume.png)

## Projects - NLayers

### Simple
N-tier project used to develop APIs where the business needs to apply simple rules.

#### Health Checks
**[Simple WebStatus](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-webstatus)**
Allows you to monitor the health of applications and tools (SqlServer, PostgreSql, MySql, RabbitMQ, MongoDB, Redis, ...).

#### Relational Database (MySql, PostgreSql, SqlServer)
**[CRUD - EF - Simple](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-crud-ef-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete. Operations are performed by transiting a database entity. If you intend to develop a product that will be consumed publicly (outside of a private network), use CRUD - EF - Complex.

**[CRUD - EF - Dapper - Simple](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-crud-ef-dapper-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination and get an item (with navigation) using Dapper. The operations to create, change and delete are performed with EF and traffic a database entity. If you intend to develop a product that will be consumed publicly (outside of a private network), use CRUD - EF - Dapper - Complex.

**[CRUD - EF - Entity Log - Simple](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-crud-ef-entitylog-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete. Operations are performed by transiting a database entity. The architecture coordinates created/by, modified/by, and removed/by log fields. It is worth mentioning that all queries will contain the removed filter, taking into account the SoftDelete reference. If you intend to develop a product that will be consumed publicly (outside of a private network), use CRUD - EF - Entity Log - Complex.

#### Database NoSql - MongoDb
**[CRUD - MongoDb - Simple](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-crud-mongodb-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete. Operations are performed by transiting a database entity. It is worth mentioning that we must create strategies for recording data since there is no relationship. If you intend to develop a product that will be consumed publicly (outside of a private network), use CRUD - MongoDb - Complex.

#### Database NoSql - Redis
**[CRUD - Redis - Simple](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-crud-redis-customer-api/CustomerAPI.WebAPI)**
Allows you to get an item, create and delete. Operations are performed by referencing a key to that key-value database. We often use this solution for cache management, as Redis is an in-memory database with incredible performance.

#### Message Broker - RabbitMQ
**[CRUD - EF - RabbitMQ - Simple](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-rabbitmq-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete. Operations are performed asynchronously from a queue managed by RabbitMQ. We use HostedService to consume RabbitMQ messages from the web project, that is, the messages will be processed while the API is running.

#### Pipeline
**[Pipeline - Simple](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/simple-pipeline-customer-api/CustomerAPI.WebAPI)**
Pipeline pattern with simple operations.

### Complex
N-tier project used to develop APIs where the business needs to apply complex rules, higher level of security, less data traffic, validation of sensitive data and separation of responsibilities or consumption by other technologies and projects.

#### Relational Database (MySql, PostgreSql, SqlServer)
**[CRUD - EF - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-crud-ef-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete.

**[CRUD - EF - Dapper - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-crud-ef-dapper-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination and get an item (with navigation) using Dapper. The operations to create, change and delete are performed with EF.

**[CRUD - EF - Only Entity - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-crud-ef-only-entity-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete. We do not create object for traffic and mapping. We use the database entity itself.

**[CRUD - EF - Entity Log - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-crud-ef-entitylog-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete.

#### Database NoSql - MongoDb
**[CRUD - MongoDb - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-crud-mongodb-customer-api/CustomerAPI.WebAPI)**
Allows you to search with pagination, get an item, create, change and delete.

#### Pipeline
**[Pipeline - Builder - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-pipeline-builder-customer-api/CustomerAPI.WebAPI)**
Pipeline pattern with operations registered through constructors. Excellent strategy for use cases.

**[Pipeline - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-pipeline-customer-api/CustomerAPI.WebAPI)**
Pipeline pattern with simple layered operations.

**[Pipeline - Ports and Adapters - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-pipeline-ports-adapters-customer-api/CustomerAPI.WebAPI)**
Pipeline pattern with operations recorded via constructors. We associate this strategy with the very loosely coupled Ports and Adapters architecture model.

**[Pipeline - EF (MySql, PostgreSql, SqlServer) - Complex](https://github.com/kallebelins/mvp24hours-netcore-samples/tree/main/src/complex-pipeline-ef-customer-api/CustomerAPI.WebAPI)**
We generally use it for service integration. The pipeline concept is excellent for tracking all the steps (operations/filters) performed in an integration (adapter/mediator/register/filter). In this solution we obtain the data in an integration and register it in a database with EF.

## Donations
Please consider donating if you think this library is useful to you or that my work is valuable. Glad if you can help me [buy a cup of coffee](https://www.paypal.com/donate/?hosted_button_id=EKA2L256GJVQC). :heart:

## Community
Users, stakeholders, students, enthusiasts, developers, programmers [connect on Telegram](https://t.me/+6_sL0y2TE-ZkMmZh) to follow our growth closely!

## Sponsors
Be a sponsor by choosing this project to accelerate your products.