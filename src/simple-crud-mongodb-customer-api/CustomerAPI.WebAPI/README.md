# CustomerAPI - CRUD - MongoDb - Simple
N-tier project used to develop APIs where the business needs to apply simple rules.

## Features:
- NoSQL database (MongoDb); 
- Documentation (Swagger); 
- Logging (NLog); 
- Patterns for data validation (FluentValidation and Data Annotations);
- Notification pattern;
- Unit of Work (Transaction);
- Repository (Paging, List, Create, Update, Delete) - Query apply: Filter, Paging - No navigation;
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

### WebAPI
Layer that lies on the project boundary. We use this project to make the resources (data and actions) of our API available. Our client will connect via HTTP requests to get resources in JSON format ("application/json").

## NoSQL Database

### MongoDb (Document Oriented)
https://mvp24hours.dev/#/en-us/database/nosql?id=mongodb