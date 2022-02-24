# CustomerAPI - Pipeline - Complex
N-tier project used to develop APIs where the business needs to apply complex rules, higher level of security, less data traffic, validation of sensitive data and separation of responsibilities or consumption by other technologies and projects.
Pipeline represents a tunnel with several levels or operations through which a packet/message travels.
The pipeline pattern is used to perform service integration, since we have control over the entire integration process through filters/operations.

## Features:
- Pipe and Filters pattern;
- Documentation (Swagger); 
- Logging (NLog); 
- Facade pattern;
- Dependency injection (IoC);
- Using ActionResult for API resources (Restful);
- Middlewares for handling unmanaged failures;
- DDD concepts;

## Layers:

### Core
Heart of the application. In this project we define the business: entities, valueobjects/dtos, validations, service contracts, enumerators, messages, specifications, builders or any other business definition.

### Application
Layer where we implement/develop the rules defined in the "Core". We use this project as a gateway to the business frontier, which means that we will be able to consume business rules in different technologies (desktop, web api, web services, web mvc, web forms, hosted services, etc.).

### WebAPI
Layer that lies on the project boundary. We use this project to make the resources (data and actions) of our API available. Our client will connect via HTTP requests to get resources in JSON format ("application/json").

## Pipe and Filters
Access: https://mvp24hours.dev/#/en-us/pipeline?id=pipeline-pipe-and-filters-pattern