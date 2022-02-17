# CustomerAPI - Pipeline - Simple
N-tier project used to develop APIs where the business needs to apply simple rules.
Pipeline represents a tunnel with several levels or operations through which a packet/message travels.
The pipeline pattern is used to perform service integration, since we have control over the entire integration process through filters/operations.

## Features:
- Pipe and Filters pattern;
- Documentation (Swagger); 
- Logging (NLog); 
- Notification pattern;
- Facade pattern;
- Dependency injection (IoC);
- Using ActionResult for API resources (Restful);
- Middlewares for handling unmanaged failures;
- DDD concepts;
- Health Checks;

## Layers:

### Core
Heart of the application. In this project we define the business: entities, valueobjects/dtos, validations, service contracts, enumerators, messages, specifications, builders or any other business definition.

### WebAPI
Layer that lies on the project boundary. We use this project to make the resources (data and actions) of our API available. Our client will connect via HTTP requests to get resources in JSON format ("application/json").

## Pipe and Filters
Access: https://mvp24hours.dev/#/en-us/pipeline?id=pipeline-pipe-and-filters-pattern