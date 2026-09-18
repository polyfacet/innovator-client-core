# Innovator Client Core

A lightweight .NET library for working with Aras Innovator using the `Innovator.Client` SDK.

This project wraps common Innovator session creation, data-model access, user membership helpers, and workflow inspection into a small reusable API. It also includes a console demo application showing how to connect to Aras and perform everyday operations.  
The test are primarily written to demonstrate the API.  

## Why this project exists

The library is meant to simplify common Aras Innovator tasks in .NET without forcing application code to deal directly with verbose AML and low-level connection setup.

Some examples includes:

- creating and reusing Innovator sessions
- retrieving item metadata and relationship data
- working with users and group membership
- inspecting workflow maps and activity templates
- giving a demo app that exercises these patterns in practice

## Features

- Session creation using Aras connection preferences and credentials
- Optional session caching with `SessionManager`
- MD5 password detection for hashed credentials
- extension methods for common item lookup patterns
- user helpers for create/get/enable/member checks
- workflow map traversal through activity templates and paths
- metadata helpers for item types and related model objects
- example console application in `demo/DemoConsoleApp`

## Project structure

- `src/InnovatorCore` - main reusable library
  - `Connections/` - session and session manager
  - `Extensions/` - convenience methods on `Innovator` and `Item`
  - `Extensions/Meta/` - data model helpers such as item types and methods
  - `Users/` - user and membership helpers
  - `Workflows/` - workflow map and activity metadata
- `demo/DemoConsoleApp` - sample console application
- `tests/InnovatorCore.Tests` - unit/integration tests for the main functionality

## Requirements

- .NET 10 SDK
- An Aras Innovator instance reachable from your environment

## Installation

Add the project to your solution or reference it from another .NET project.

```bash
dotnet add reference src/InnovatorCore/InnovatorCore.csproj
```

Or, if you are consuming it from another project, add a project reference in your `.csproj` file:

```xml
<ItemGroup>
  <ProjectReference Include="..\InnovatorCore\InnovatorCore.csproj" />
</ItemGroup>
```

## Getting started

### Create a session

```csharp
using Connections;

var inn = Session.CreateSession(
    "https://your-aras-instance.example.com",
    "YourDatabase",
    "admin",
    "yourPassword");
```

### Reuse named sessions

```csharp
using Connections;

var inn = SessionManager.CreateSession(
    "default",
    "https://your-aras-instance.example.com",
    "YourDatabase",
    "admin",
    "yourPassword");

var sameInn = SessionManager.GetSession("default");
```

### Get an item by ID

```csharp
using Extensions;

var item = inn.GetItem("Part", "some-part-id");
```

### Get an item by name

```csharp
using Extensions;

var itemType = inn.GetItemByName("ItemType", "Part");
```

### Access the data model

```csharp
using Extensions;

var model = inn.DataModel();
var partType = model.ItemType("Part");
string itemNumberDataType = partType.Property("item_number").DataType;
```

### Work with users

```csharp
using Users;

var userHelper = new User(inn);
var existing = userHelper.UserExists("jsmith");

var created = userHelper.CreateUser("jsmith", "Password123", "Jane", "Smith");
```

### Inspect workflow maps

```csharp
using Workflows;

var workflowMap = new WorkflowMap(inn, workflowMapId);
foreach (var activity in workflowMap.ActivityTemplates)
{
    Console.WriteLine(activity.Name);
}
```

## Core API overview

### `Session`

Creates a `Innovator.Client.IOM.Innovator` instance using a URL, database, username, and password. It also automatically detects hashed MD5 passwords.

### `SessionManager`

Keeps a dictionary of named sessions so the same Aras connection can be reused without repeated login overhead.

### `InnovatorExtensions`

Adds convenience methods to the Innovator instance for common operations such as:

- `GetUser()`
- `GetItem(...)`
- `GetItemByName(...)`
- `GetItemByConfigId(...)`
- `GetIdentity()`
- `ApplyAML(...)`
- `DataModel()`

### `User`

Provides helper methods such as:

- `CreateUser(...)`
- `UserExists(...)`
- `GetUserByLoginName(...)`
- `UserIsEnabled(...)`
- `IsMemberOf(...)`
- `IsDirectMemberOf(...)`
- `AddUserAsMember(...)`
- `RemoveUserAsMember(...)`

### `DataModel`

Builds metadata-oriented helpers for item types and related model information. It can be used to resolve item definitions and related metadata from Aras.

### `WorkflowMap`

Allows inspection of workflow structures by loading `Workflow Map` data and exploring its `ActivityTemplate` objects and map paths.

## Demo application

The repository includes a sample console application in `demo/DemoConsoleApp` that demonstrates:

- reading configuration from `appsettings.json`
- connecting to Aras
- selecting and switching connections
- creating or deleting users
- querying memberships
- exploring workflow and metadata information

Run it with:

```bash
dotnet run --project demo/DemoConsoleApp/DemoConsoleApp.csproj
```

## Testing

The test project is under `tests/InnovatorCore.Tests` and exercises session creation, user behavior, and workflow interactions.

```bash
dotnet test tests/InnovatorCore.Tests/InnovatorCore.Tests.csproj
```

## Notes

This project is intentionally practical and focused on working with Aras Innovator as a .NET client library. It is useful when you need a thin wrapper around common Innovator patterns while keeping the logic readable and easy to extend.

## License

This project is provided under the repository license. See the `LICENSE` file for details.
