# CustomerAPI - CRUD - EF - Entity Log - Simple
N-tier project used to develop APIs where the business needs to apply simple rules.

## Features:
- Relational database (SQL Server, PostgreSql, MySql) with EF; 
- Documentation (Swagger); 
- Logging (NLog); 
- Patterns for data validation (FluentValidation and Data Annotations);
- Notification pattern;
- Unit of Work (Transaction);
- Repository (Paging, List, Create, Update, Delete) - Query apply: Navigation, Filter, Paging;
- FluentAPI configuration EF;
- Facade pattern;
- Dependency injection (IoC);
- Using ActionResult for API resources (Restful);
- Middlewares for handling unmanaged failures;
- DDD concepts;
- Automatically create, change and delete logs;
- Automatic filter for logically excluded records (Removed column) in any search;

## Layers:

### Core
Heart of the application. In this project we define the business: entities, valueobjects/dtos, validations, service contracts, enumerators, messages, specifications, builders or any other business definition.

### Infrastructure
Layer used to deal with issues related to infrastructure: database, web requests, reading/writing files, or rather, any connection to machine or network resources.

### WebAPI
Layer that lies on the project boundary. We use this project to make the resources (data and actions) of our API available. Our client will connect via HTTP requests to get resources in JSON format ("application/json").

## Database integrated with EF

### MYSQL
Access: https://mvp24hours.dev/#/en-us/database/relational?id=mysql

### POSTGRESQL
Access: https://mvp24hours.dev/#/en-us/database/relational?id=postgresql

### SQLSERVER
Access: https://mvp24hours.dev/#/en-us/database/relational?id=sql-server