# CustomerAPI - CRUD - EF - Dapper - Complex
N-tier project used to develop APIs where the business needs to apply complex rules, higher level of security, less data traffic, validation of sensitive data and separation of responsibilities or consumption by other technologies and projects.

## Features:
- Relational database (SQL Server, PostgreSql, MySql) with EF (actions) and Dapper (queries); 
- Documentation (Swagger); 
- Mapping (AutoMapper); 
- Logging (NLog); 
- Patterns for data validation (FluentValidation and Data Annotations);
- Notification pattern;
- Specification pattern;
- Unit of Work (Transaction);
- Repository (Paging, List, Create, Update, Delete) - Query apply: Navigation, Filter, Paging;
- FluentAPI configuration EF;
- Facade pattern;
- Dependency injection (IoC);
- Using ActionResult for API resources (Restful);
- Middlewares for handling unmanaged failures;
- DDD concepts;

## Layers:

### Core
Heart of the application. In this project we define the business: entities, valueobjects/dtos, validations, service contracts, enumerators, messages, specifications, builders or any other business definition.

### Infrastructure
Layer used to deal with issues related to infrastructure: database, web requests, reading/writing files, or rather, any connection to machine or network resources.

### Application
Layer where we implement/develop the rules defined in the "Core". We use this project as a gateway to the business frontier, which means that we will be able to consume business rules in different technologies (desktop, web api, web services, web mvc, web forms, hosted services, etc.).

### WebAPI
Layer that lies on the project boundary. We use this project to make the resources (data and actions) of our API available. Our client will connect via HTTP requests to get resources in JSON format ("application/json").

## Database integrated with EF

### MYSQL
Access: https://mvp24hours.dev/#/en-us/database/relational?id=mysql

### POSTGRESQL
Access: https://mvp24hours.dev/#/en-us/database/relational?id=postgresql

### SQLSERVER
Access: https://mvp24hours.dev/#/en-us/database/relational?id=sql-server