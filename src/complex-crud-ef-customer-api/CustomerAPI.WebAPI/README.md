# CustomerAPI - CRUD - EF - Complex
N-tier project used to develop APIs where the business needs to apply complex rules, higher level of security, less data traffic, validation of sensitive data and separation of responsibilities or consumption by other technologies and projects.

## Features:
- Relational database (SQL Server, PostgreSql, MySql) with EF; 
- Documentation (Swagger); 
- Mapping (AutoMapper); 
- Logging (NLog); 
- Patterns for data validation (FluentValidation and Data Annotations);
- Specification pattern;
- Unit of Work (Transaction);
- Repository (Paging, List, Create, Update, Delete) - Query apply: Navigation, Filter, Paging;
- FluentAPI configuration EF;
- Facade pattern;
- Dependency injection (IoC);
- Using ActionResult for API resources (Restful);
- Middlewares for handling unmanaged failures;
- DDD concepts;
- Health Checks;

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

### SqlServer

```csharp
/// Package Manager Console >
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 5.0.10

/// Startup.cs
services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DataContext"))
);
```

Access: https://mvp24hours.dev/#/en-us/database/relational?id=sql-server

### PostgreSQL

```csharp
/// Package Manager Console >
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL -Version 5.0.10

/// Startup.cs
services.AddDbContext<DataContext>(
    options => options.UseNpgsql(configuration.GetConnectionString("DataContext"),
    options => options.SetPostgresVersion(new Version(9, 6)))
);
```

Access: https://mvp24hours.dev/#/en-us/database/relational?id=postgresql

### MySql

```csharp
/// Package Manager Console >
Install-Package MySql.EntityFrameworkCore -Version 5.0.8

/// Startup.cs
services.AddDbContext<DataContext>(options =>
    options.UseMySQL(configuration.GetConnectionString("CustomerDbContext"))
);
```

Access: https://mvp24hours.dev/#/en-us/database/relational?id=mysql

## Health Check

```csharp
/// Package Manager Console >
Install-Package AspNetCore.HealthChecks.UI.Client -Version 3.1.2
```

Access: https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks#health-checks

### SqlServer

```csharp
/// Package Manager Console >
Install-Package AspNetCore.HealthChecks.SqlServer -Version 3.2.0

/// ServiceBuilderExtensions
services.AddHealthChecks()
	.AddSqlServer(
		configuration.GetConnectionString("CustomerDbContext"),
		healthQuery: "SELECT 1;",
		name: "SqlServer", 
		failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);

```

### PostgreSQL

```csharp
/// Package Manager Console >
Install-Package AspNetCore.HealthChecks.Npgsql -Version 3.1.1

/// ServiceBuilderExtensions
services.AddHealthChecks()
	.AddNpgSql(
		configuration.GetConnectionString("CustomerDbContext"),
		name: "PostgreSql", 
		failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);

```

### MySql

```csharp
/// Package Manager Console >
Install-Package AspNetCore.HealthChecks.MySql -Version 3.2.0

/// ServiceBuilderExtensions
services.AddHealthChecks()
	.AddMySql(
		configuration.GetConnectionString("CustomerDbContext"), 
		name: "MySql", 
		failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
```
