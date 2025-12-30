# .NET 10.0 Upgrade Plan - AspireApp1

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Risk Management](#risk-management)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description

Upgrade the **AspireApp1** solution from **.NET 9.0** to **.NET 10.0 (Long Term Support)**. This .NET Aspire-based solution consists of 6 projects including frontend (Blazor), backend API, service defaults, orchestration host, API service, and test projects.

### Scope

**Projects Affected:** 6 projects
- AspireApp1.ServiceDefaults (Shared service configuration)
- AspireApp1.ApiService (API service layer)
- AspireApp1.BackEnd (DbApi - Backend API with EF Core)
- AspireApp1.FrontEnd (Web - Blazor frontend with Windows Authentication)
- AspireApp1.AppHost (Aspire orchestration host)
- AspireApp1.Tests (Test project)

**Current State:** All projects targeting net9.0

**Target State:** All projects targeting net10.0

### Discovered Metrics

| Metric | Value |
|--------|-------|
| Total Projects | 6 |
| Total NuGet Packages | 16 |
| Packages Requiring Update | 10 (62.5%) |
| Total Lines of Code | 2,441 |
| Code Files with Issues | 14 |
| Estimated LOC Impact | 39+ (1.6% of codebase) |
| Dependency Depth | 2 levels |
| Security Vulnerabilities | 0 ? |
| High-Risk Projects | 0 |

### Complexity Classification

**Classification: Simple Solution** ??

**Justification:**
- Small project count (6 projects)
- Low dependency depth (2 levels: ServiceDefaults Å® Apps Å® AppHost Å® Tests)
- No circular dependencies
- All projects currently on modern .NET 9.0
- All projects rated Low difficulty (??)
- No security vulnerabilities detected
- Homogeneous Aspire-based architecture
- Clear, simple dependency structure

### Selected Strategy

**All-At-Once Strategy** - All projects upgraded simultaneously in a single coordinated operation.

**Rationale:**
- Small solution size (6 projects) ideal for atomic upgrade
- All projects currently on .NET 9.0 (no mixed legacy frameworks)
- Clear dependency structure supports coordinated update
- All required package versions available for .NET 10.0
- No deprecated packages blocking upgrade (Aspire packages have clear migration path)
- Low overall risk profile enables single-phase approach
- Faster completion time with simpler coordination

### Critical Issues

**API Compatibility:**
- ?? 38 Behavioral changes (primarily `System.Uri` and `HttpContent` - low impact, require runtime testing)
- ?? 1 Source incompatibility (`TimeSpan.FromSeconds` in test project)
- ? 0 Binary incompatible changes

**Package Updates:**
- ?? Aspire packages: 9.0.0 Å® 13.1.0 (major version jump, well-documented upgrade path)
- ?? Entity Framework Core: 8.0.12 Å® 10.0.1
- ?? ASP.NET Core Authentication: 8.0.0 Å® 10.0.1
- ?? OpenTelemetry instrumentation: 1.9.0 Å® 1.14.0
- ? 6 packages already compatible (MSTest, Npgsql.EF, OpenTelemetry core packages, Swashbuckle)

### Iteration Strategy

**Fast Batch Approach** - 2-3 detail iterations:
1. **Foundation Iteration**: Fill dependency analysis, migration strategy, and project stubs
2. **Batch Detail Iteration**: Complete all 6 project-by-project plans in single pass (simple, homogeneous projects)
3. **Final Iteration**: Complete package reference, breaking changes catalog, testing strategy, risk management, source control, and success criteria

Expected total: **5 iterations** (3 foundation + 1 batch detail + 1 completion)

---

## Migration Strategy

### Approach Selection

**Selected Approach: All-At-Once Strategy**

All 6 projects will be upgraded simultaneously in a single coordinated operation. All TargetFramework properties and package references are updated together, followed by a unified build and fix cycle.

### Justification

**Why All-At-Once is Appropriate:**

1. **Small Solution Size** - 6 projects is well within the threshold for atomic upgrades (recommended: <10 projects)

2. **Homogeneous Technology Stack** - All projects are modern .NET 9.0 Aspire projects with consistent patterns

3. **Simple Dependency Structure** - Clear layered architecture with no circular dependencies or complex relationships

4. **Low Risk Profile:**
   - No security vulnerabilities
   - All projects rated Low difficulty (??)
   - Only 1 source incompatibility (easily addressable)
   - 38 behavioral changes (low impact, primarily `System.Uri`)

5. **Package Compatibility** - All required package versions available and compatible with .NET 10.0

6. **Team Efficiency** - Single upgrade operation is faster and requires less coordination than incremental approach

7. **Aspire Architecture** - .NET Aspire solutions are designed to be upgraded atomically (host and services together)

### All-At-Once Strategy Rationale

**Advantages for This Solution:**
- **Fastest Completion** - Single upgrade operation instead of multiple phases
- **No Multi-Targeting** - Avoid complexity of supporting multiple framework versions simultaneously
- **Consistent State** - Entire solution on .NET 10.0 immediately after upgrade
- **Simpler Testing** - Test entire solution once rather than testing multiple intermediate states
- **Aspire Compatibility** - AppHost and services upgraded together (Aspire requirement)

**Mitigated Risks:**
- **Build Failures** - Low risk due to minimal breaking changes (1 source incompatibility)
- **Runtime Issues** - Behavioral changes limited to `Uri` and `HttpContent` (well-understood)
- **Package Conflicts** - All packages have clear upgrade paths validated by assessment

### Dependency-Based Ordering

While the upgrade is atomic, validation and testing should follow dependency order:

**Validation Sequence:**
1. **Foundation First** - ServiceDefaults validates successfully
2. **Services in Parallel** - ApiService, BackEnd, FrontEnd validate together
3. **Orchestration** - AppHost validates with all services
4. **Tests Last** - Tests run against upgraded stack

This ordering ensures that if issues arise, they're detected at the appropriate layer.

### Execution Approach

**Single Atomic Operation:**
1. Update all 6 project files (TargetFramework: net9.0 Å® net10.0)
2. Update all package references across all projects simultaneously
3. Restore dependencies (`dotnet restore`)
4. Build entire solution (`dotnet build`)
5. Address all compilation errors in single pass (reference Breaking Changes Catalog)
6. Rebuild to verify all fixes applied
7. Execute all tests
8. Validate solution builds with 0 errors and all tests pass

**No Intermediate States** - Solution moves from .NET 9.0 to .NET 10.0 in single operation without partial upgrades.

### Parallel vs Sequential Execution

**Within the Atomic Operation:**
- Project file updates: Sequential (scripted updates to all 6 files)
- Package reference updates: Sequential (coordinated across all projects)
- Build: Single solution-level build (MSBuild handles project dependencies automatically)
- Error fixes: As discovered (some may affect multiple projects)
- Testing: All test projects executed together

**Human Perspective:**
The entire upgrade appears as a single cohesive change rather than project-by-project progression.

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The AspireApp1 solution follows a classic .NET Aspire architecture pattern with clear dependency layers:

```
Layer 0 (Foundation):
  Ñ§Ñü AspireApp1.ServiceDefaults (no dependencies)

Layer 1 (Application Services):
  Ñ•Ñü AspireApp1.ApiService Å® ServiceDefaults
  Ñ•Ñü AspireApp1.BackEnd Å® ServiceDefaults
  Ñ§Ñü AspireApp1.FrontEnd Å® ServiceDefaults

Layer 2 (Orchestration):
  Ñ§Ñü AspireApp1.AppHost Å® ApiService, BackEnd, FrontEnd

Layer 3 (Testing):
  Ñ§Ñü AspireApp1.Tests Å® AppHost
```

**Dependency Characteristics:**
- **No circular dependencies** ?
- **Maximum depth:** 3 levels (Tests Å® AppHost Å® Services Å® ServiceDefaults)
- **Widest layer:** 3 projects (Layer 1 - all application services)
- **Critical path:** ServiceDefaults is the foundation for all application projects

### Project Groupings by Migration Phase

For All-At-Once strategy, all projects are upgraded simultaneously as a single atomic operation. However, understanding the logical layers helps validate the upgrade progresses correctly:

**Validation Order (post-upgrade):**
1. **ServiceDefaults** (foundation) - Validate first as it impacts all services
2. **Application Services** (ApiService, BackEnd, FrontEnd) - Can be validated in parallel
3. **AppHost** (orchestration) - Validate after services
4. **Tests** - Validate last (depends on entire stack)

### Critical Dependencies

**Foundation Project:**
- **AspireApp1.ServiceDefaults** is consumed by 3 projects (ApiService, BackEnd, FrontEnd)
- Contains shared service configuration, telemetry, resilience, and service discovery
- No external project dependencies (only NuGet packages)
- **Must upgrade successfully** as all application services depend on it

**Orchestration Project:**
- **AspireApp1.AppHost** orchestrates all 3 application services
- Uses Aspire.Hosting.AppHost package (9.0.0 Å® 13.1.0 major version jump)
- Critical for development/testing experience

**Application Services:**
- All 3 services (ApiService, BackEnd, FrontEnd) depend only on ServiceDefaults
- Can be upgraded in parallel (no inter-dependencies)
- BackEnd has additional Entity Framework Core dependencies

### No Blocking Dependencies

? No circular dependencies detected
? No cross-service dependencies (services are independent)
? Clean layered architecture supports atomic upgrade

---

## Project-by-Project Migration Plans

### Project: AspireApp1.ServiceDefaults

**Current State:**
- Target Framework: net9.0
- Project Type: ClassLibrary
- Dependencies: 0 project dependencies
- Dependants: 3 projects (ApiService, BackEnd, FrontEnd)
- Lines of Code: 119
- Package Count: 6 (5 need updates)
- Risk Level: Low ??

**Target State:**
- Target Framework: net10.0
- All packages updated to .NET 10-compatible versions

**Migration Steps:**

#### 1. Prerequisites
- Verify ServiceDefaults.csproj is accessible
- No project dependencies to wait for

#### 2. Framework Update
Update `AspireApp1.ServiceDefaults\AspireApp1.ServiceDefaults.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 3. Package Updates

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Microsoft.Extensions.Http.Resilience | 9.0.0 | 10.1.0 | .NET 10 compatibility |
| Microsoft.Extensions.ServiceDiscovery | 9.0.0 | 10.1.0 | .NET 10 compatibility; out of support |
| OpenTelemetry.Instrumentation.AspNetCore | 1.9.0 | 1.14.0 | .NET 10 compatibility |
| OpenTelemetry.Instrumentation.Http | 1.9.0 | 1.14.0 | .NET 10 compatibility |
| OpenTelemetry.Exporter.OpenTelemetryProtocol | 1.9.0 | (compatible) | No update needed |
| OpenTelemetry.Extensions.Hosting | 1.9.0 | (compatible) | No update needed |
| OpenTelemetry.Instrumentation.Runtime | 1.9.0 | (compatible) | No update needed |

#### 4. Expected Breaking Changes
**None** - No API compatibility issues detected in this project.

#### 5. Code Modifications
**None required** - This is a configuration library with no source-level breaking changes.

#### 6. Testing Strategy
- **Unit Tests**: N/A (no dedicated unit tests for this project)
- **Integration Testing**: Will be validated when consuming projects (ApiService, BackEnd, FrontEnd) build and run
- **Smoke Test**: Verify telemetry, resilience, and service discovery work in consuming applications

#### 7. Validation Checklist
- [ ] Project file updated to `<TargetFramework>net10.0</TargetFramework>`
- [ ] All 4 packages updated to target versions
- [ ] `dotnet restore` succeeds
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Consuming projects (ApiService, BackEnd, FrontEnd) build successfully
- [ ] Telemetry visible in Aspire dashboard (runtime validation)

---

### Project: AspireApp1.ApiService

**Current State:**
- Target Framework: net9.0
- Project Type: AspNetCore (Minimal API)
- Dependencies: 1 (ServiceDefaults)
- Dependants: 1 (AppHost)
- Lines of Code: 25
- Package Count: 2 (1 needs update)
- API Issues: 1 behavioral change
- Risk Level: Low ??

**Target State:**
- Target Framework: net10.0
- Microsoft.AspNetCore.OpenApi updated to 10.0.1

**Migration Steps:**

#### 1. Prerequisites
- **ServiceDefaults** must be upgraded first (dependency)

#### 2. Framework Update
Update `AspireApp1.ApiService\AspireApp1.ApiService.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 3. Package Updates

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Microsoft.AspNetCore.OpenApi | 9.0.0 | 10.0.1 | .NET 10 compatibility |

#### 4. Expected Breaking Changes

**Behavioral Change (Low Impact):**
- **API:** `ExceptionHandlerExtensions.UseExceptionHandler(IApplicationBuilder)`
- **Location:** `Program.cs`, line 14: `app.UseExceptionHandler();`
- **Change:** Exception handling behavior may differ in .NET 10
- **Action:** Test exception handling scenarios at runtime

#### 5. Code Modifications
**None required** - Behavioral change does not require code updates, only runtime validation.

#### 6. Testing Strategy
- **Unit Tests**: N/A (minimal API service)
- **Integration Testing**: Test API endpoints return expected responses
- **Exception Handling**: Verify exception handler behaves correctly (intentionally trigger error)
- **OpenAPI**: Verify Swagger/OpenAPI documentation generates correctly

#### 7. Validation Checklist
- [ ] Project file updated to `<TargetFramework>net10.0</TargetFramework>`
- [ ] Microsoft.AspNetCore.OpenApi updated to 10.0.1
- [ ] `dotnet restore` succeeds
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] API endpoints respond correctly
- [ ] OpenAPI/Swagger documentation accessible
- [ ] Exception handler works as expected

---

### Project: AspireApp1.BackEnd (DbApi)

**Current State:**
- Target Framework: net9.0
- Project Type: AspNetCore (Web API with EF Core)
- Dependencies: 1 (ServiceDefaults)
- Dependants: 1 (AppHost)
- Lines of Code: 1,499 (largest project)
- Package Count: 6 (3 need updates)
- API Issues: 1 behavioral change
- Risk Level: Low ??

**Target State:**
- Target Framework: net10.0
- Entity Framework Core packages updated: 8.0.12 Å® 10.0.1 (2 major versions)
- Authentication package updated: 8.0.0 Å® 10.0.1

**Migration Steps:**

#### 1. Prerequisites
- **ServiceDefaults** must be upgraded first (dependency)
- PostgreSQL database accessible for testing

#### 2. Framework Update
Update `AspireApp1.DbApi\AspireApp1.BackEnd.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 3. Package Updates

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Microsoft.EntityFrameworkCore.Design | 8.0.12 | 10.0.1 | .NET 10 compatibility; EF Core upgrade |
| Microsoft.EntityFrameworkCore.Tools | 8.0.12 | 10.0.1 | .NET 10 compatibility; EF Core upgrade |
| Microsoft.AspNetCore.Authentication.Negotiate | 8.0.0 | 10.0.1 | .NET 10 compatibility; Windows Auth |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.8 | (compatible) | No update needed |
| Swashbuckle.AspNetCore | 6.5.0 | (compatible) | No update needed |

#### 4. Expected Breaking Changes

**Behavioral Change (Low Impact):**
- **API:** `ExceptionHandlerExtensions.UseExceptionHandler(IApplicationBuilder, String)`
- **Location:** `Program.cs`, line 50: `app.UseExceptionHandler("/error");`
- **Change:** Exception handling behavior may differ in .NET 10
- **Action:** Test exception handling scenarios at runtime

**Entity Framework Core (Attention Required):**
- **EF Core 8 Å® 10** is a 2-major-version jump
- **Potential Issues:**
  - Migration compatibility (may need regeneration)
  - Query behavior changes
  - Database provider compatibility (Npgsql.EF 8.0.8 should be compatible)
- **Action:** Test all database operations thoroughly; regenerate migrations if issues arise

#### 5. Code Modifications
**Likely none required**, but prepare for:
- EF Core migration regeneration if runtime errors occur
- Potential query adjustments if EF Core behavioral changes affect queries

#### 6. Testing Strategy
- **Database Connectivity**: Verify connection to PostgreSQL
- **CRUD Operations**: Test Create, Read, Update, Delete for all entities
- **Migrations**: Verify existing migrations apply correctly (or regenerate if needed)
- **Authentication**: Test Windows Authentication (Negotiate package updated)
- **API Endpoints**: Test all API controllers
- **Exception Handling**: Verify error handling at `/error` endpoint

#### 7. Validation Checklist
- [ ] Project file updated to `<TargetFramework>net10.0</TargetFramework>`
- [ ] All 3 packages updated to target versions
- [ ] `dotnet restore` succeeds
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Database migrations apply successfully
- [ ] CRUD operations work correctly
- [ ] Windows Authentication functional
- [ ] All API endpoints respond correctly
- [ ] Swagger/OpenAPI documentation accessible

---

### Project: AspireApp1.FrontEnd (Web)

**Current State:**
- Target Framework: net9.0
- Project Type: AspNetCore (Blazor Server with Windows Authentication)
- Dependencies: 1 (ServiceDefaults)
- Dependants: 1 (AppHost)
- Lines of Code: 755
- Package Count: 2 (1 needs update)
- API Issues: 36 behavioral changes (highest in solution)
- Risk Level: Low ??

**Target State:**
- Target Framework: net10.0
- Authentication package updated: 8.0.0 Å® 10.0.1
- All API client code validated (36 `System.Uri` and `HttpContent` behavioral changes)

**Migration Steps:**

#### 1. Prerequisites
- **ServiceDefaults** must be upgraded first (dependency)

#### 2. Framework Update
Update `AspireApp1.Web\AspireApp1.FrontEnd.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 3. Package Updates

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Microsoft.AspNetCore.Authentication.Negotiate | 8.0.0 | 10.0.1 | .NET 10 compatibility; Windows Auth |

#### 4. Expected Breaking Changes

**Behavioral Changes (Attention Required):**

**System.Uri (18 instances)** - URL construction and escaping behavior may change:
- **Locations:**
  - `Program.cs`: Lines 85, 89 (repeated) - `new Uri(...)` constructors for HttpClient BaseAddress
  - `AdminApiClient.cs`: Line 70 - `Uri.EscapeDataString(username)`
  - `ProjectActivityApiClient.cs`: Line 93 - `Uri.EscapeDataString(activityType)`
- **Impact:** URL parsing, encoding, and validation behavior may differ
- **Action:** Test all API client operations; verify URLs constructed correctly

**HttpContent (9 instances)** - JSON deserialization behavior may change:
- **Locations:**
  - `ProjectsApiClient.cs`: Line 42
  - `CustomerApiClient.cs`: Lines 174, 195, 216, 237, 258
  - `AdminApiClient.cs`: Lines 82, 141
  - `ProjectActivityApiClient.cs`: Line 109
- **Pattern:** `await res.Content.ReadFromJsonAsync<T>(ct)`
- **Impact:** JSON deserialization behavior may differ
- **Action:** Test all API client responses; verify DTOs deserialize correctly

**ExceptionHandlerExtensions (1 instance)**:
- **Location:** `Program.cs`, line 97: `app.UseExceptionHandler("/Error", createScopeForErrors: true);`
- **Impact:** Exception handling behavior may differ
- **Action:** Test error pages render correctly

#### 5. Code Modifications
**None required** - All behavioral changes require runtime validation only, no source changes needed.

#### 6. Testing Strategy
- **Authentication**: Test Windows Authentication (Negotiate) with AD users
- **API Clients**: Test all 4 API client classes:
  - ProjectsApiClient (GetProjectAsync)
  - CustomerApiClient (all CRUD operations)
  - AdminApiClient (user and role operations)
  - ProjectActivityApiClient (activity queries)
- **URL Construction**: Verify all BaseAddress assignments work (`https+http://dbapi` scheme)
- **JSON Deserialization**: Verify all DTOs deserialize correctly from API responses
- **Blazor Components**: Test interactive components render and function correctly
- **Authorization**: Test AdminOnly policy enforcement
- **Error Handling**: Verify error page at `/Error` works

#### 7. Validation Checklist
- [ ] Project file updated to `<TargetFramework>net10.0</TargetFramework>`
- [ ] Microsoft.AspNetCore.Authentication.Negotiate updated to 10.0.1
- [ ] `dotnet restore` succeeds
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Windows Authentication works
- [ ] All API client methods succeed
- [ ] All Blazor pages render correctly
- [ ] Authorization policies enforce correctly
- [ ] Error pages display correctly

---

### Project: AspireApp1.AppHost

**Current State:**
- Target Framework: net9.0
- Project Type: DotNetCoreApp (Aspire Orchestration Host)
- Dependencies: 3 (ApiService, BackEnd, FrontEnd)
- Dependants: 1 (Tests)
- Lines of Code: 15
- Package Count: 1 (needs update)
- API Issues: 0
- Risk Level: Medium ?? (Aspire major version jump)

**Target State:**
- Target Framework: net10.0
- Aspire.Hosting.AppHost: 9.0.0 Å® 13.1.0 (major version jump)

**Migration Steps:**

#### 1. Prerequisites
- **ApiService, BackEnd, FrontEnd** must all be upgraded first (dependencies)

#### 2. Framework Update
Update `AspireApp1.AppHost\AspireApp1.AppHost.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 3. Package Updates

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Aspire.Hosting.AppHost | 9.0.0 | 13.1.0 | .NET 10 compatibility; out of support; **major version jump** |
| Aspire.Hosting.Testing | 9.0.0 | 13.1.0 | .NET 10 compatibility; out of support; **major version jump** |

**?? IMPORTANT:** Aspire 9.0 Å® 13.1 is a **major version upgrade** (4 minor versions). Expect:
- API changes in resource configuration
- New features and capabilities
- Potential deprecations
- Configuration schema changes

#### 4. Expected Breaking Changes

**Aspire Hosting API (Medium Risk):**
- **Review Required:** [Aspire 13.x What's New](https://learn.microsoft.com/en-us/dotnet/aspire/whats-new/)
- **Potential Changes:**
  - Resource builder API changes (e.g., `AddProject`, `AddPostgres`)
  - Configuration method signatures
  - Environment variable handling
  - Launch profile behavior
- **Action:** Review Program.cs orchestration code; update if compilation errors occur

#### 5. Code Modifications
**Likely required if Aspire API changed**:
- Update resource builder calls if API signatures changed
- Update configuration methods if deprecated
- Consult Aspire 13.x migration guide for specific changes

#### 6. Testing Strategy
- **Orchestration**: Verify AppHost launches all 3 services (ApiService, BackEnd, FrontEnd)
- **Service Discovery**: Verify services discover each other correctly
- **Aspire Dashboard**: Verify dashboard accessible and shows all resources
- **Resource Configuration**: Verify PostgreSQL and other resources configure correctly
- **Launch Profiles**: Test all launch profiles work

#### 7. Validation Checklist
- [ ] Project file updated to `<TargetFramework>net10.0</TargetFramework>`
- [ ] Aspire.Hosting.AppHost updated to 13.1.0
- [ ] Review Aspire 13.x breaking changes documentation
- [ ] `dotnet restore` succeeds
- [ ] Project builds without errors (update code if API changed)
- [ ] Project builds without warnings
- [ ] AppHost launches successfully
- [ ] All 3 services start correctly
- [ ] Aspire dashboard accessible
- [ ] Service discovery functional
- [ ] PostgreSQL resource available

---

### Project: AspireApp1.Tests

**Current State:**
- Target Framework: net9.0
- Project Type: DotNetCoreApp (MSTest)
- Dependencies: 1 (AppHost)
- Dependants: 0
- Lines of Code: 28
- Package Count: 3 (1 needs update, MSTest compatible)
- API Issues: 1 source incompatibility (requires code fix)
- Risk Level: Low ??

**Target State:**
- Target Framework: net10.0
- Aspire.Hosting.Testing: 9.0.0 Å® 13.1.0
- Source incompatibility fixed

**Migration Steps:**

#### 1. Prerequisites
- **AppHost** must be upgraded first (dependency)

#### 2. Framework Update
Update `AspireApp1.Tests\AspireApp1.Tests.csproj`:
```xml
<TargetFramework>net10.0</TargetFramework>
```

#### 3. Package Updates

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Aspire.Hosting.Testing | 9.0.0 | 13.1.0 | .NET 10 compatibility; out of support |

#### 4. Expected Breaking Changes

**?? Source Incompatibility (Requires Code Fix):**
- **API:** `TimeSpan.FromSeconds(Int64)`
- **Location:** `WebTests.cs`, line 21
- **Current Code:**
  ```csharp
  await resourceNotificationService.WaitForResourceAsync("webfrontend", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
  ```
- **Issue:** `TimeSpan.FromSeconds` changed from accepting `Int64` to requiring `double` in .NET 10
- **Fix:** Cast integer to `int` or use `double` literal:
  ```csharp
  await resourceNotificationService.WaitForResourceAsync("webfrontend", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30.0));
  ```
  Or:
  ```csharp
  await resourceNotificationService.WaitForResourceAsync("webfrontend", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds((double)30));
  ```

#### 5. Code Modifications

**Required Fix:**
Update `AspireApp1.Tests\WebTests.cs`, line 21:
```csharp
// Before:
TimeSpan.FromSeconds(30)

// After:
TimeSpan.FromSeconds(30.0)
```

#### 6. Testing Strategy
- **Unit Tests**: Execute all MSTest tests
- **Aspire Integration**: Verify tests can launch AppHost and wait for resources
- **Web Frontend Test**: Verify test waits for "webfrontend" resource correctly

#### 7. Validation Checklist
- [ ] Project file updated to `<TargetFramework>net10.0</TargetFramework>`
- [ ] Aspire.Hosting.Testing updated to 13.1.0
- [ ] `TimeSpan.FromSeconds` fix applied in WebTests.cs
- [ ] `dotnet restore` succeeds
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] All tests pass (`dotnet test`)
- [ ] Web frontend resource detection works

---

## Package Update Reference

### Common Package Updates (Affecting Multiple Projects)

| Package | Current | Target | Projects Affected | Update Reason |
|---------|---------|--------|-------------------|---------------|
| Microsoft.AspNetCore.Authentication.Negotiate | 8.0.0 | 10.0.1 | 2 projects (BackEnd, FrontEnd) | .NET 10 compatibility; Windows Authentication |

### Category-Specific Updates

**Aspire Packages** (Major Version Jump - Requires Attention):
| Package | Current | Target | Projects | Update Reason |
|---------|---------|--------|----------|---------------|
| Aspire.Hosting.AppHost | 9.0.0 | 13.1.0 | AppHost | .NET 10 compatibility; out of support; **major version jump** |
| Aspire.Hosting.Testing | 9.0.0 | 13.1.0 | Tests | .NET 10 compatibility; out of support; **major version jump** |

**Entity Framework Core** (2 Major Versions Jump):
| Package | Current | Target | Projects | Update Reason |
|---------|---------|--------|----------|---------------|
| Microsoft.EntityFrameworkCore.Design | 8.0.12 | 10.0.1 | BackEnd | .NET 10 compatibility; EF Core upgrade |
| Microsoft.EntityFrameworkCore.Tools | 8.0.12 | 10.0.1 | BackEnd | .NET 10 compatibility; EF Core upgrade |

**OpenTelemetry Instrumentation**:
| Package | Current | Target | Projects | Update Reason |
|---------|---------|--------|----------|---------------|
| OpenTelemetry.Instrumentation.AspNetCore | 1.9.0 | 1.14.0 | ServiceDefaults | .NET 10 compatibility |
| OpenTelemetry.Instrumentation.Http | 1.9.0 | 1.14.0 | ServiceDefaults | .NET 10 compatibility |

**ASP.NET Core Extensions**:
| Package | Current | Target | Projects | Update Reason |
|---------|---------|--------|----------|---------------|
| Microsoft.Extensions.Http.Resilience | 9.0.0 | 10.1.0 | ServiceDefaults | .NET 10 compatibility |
| Microsoft.Extensions.ServiceDiscovery | 9.0.0 | 10.1.0 | ServiceDefaults | .NET 10 compatibility; out of support |

### Project-Specific Updates

**ApiService**:
| Package | Current | Target | Update Reason |
|---------|---------|--------|---------------|
| Microsoft.AspNetCore.OpenApi | 9.0.0 | 10.0.1 | .NET 10 compatibility |

### Compatible Packages (No Update Needed)

| Package | Version | Projects | Notes |
|---------|---------|----------|-------|
| MSTest | 3.4.3 | Tests | Already compatible with .NET 10 |
| Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.8 | BackEnd | Compatible with EF Core 10 |
| Swashbuckle.AspNetCore | 6.5.0 | BackEnd | Compatible with .NET 10 |
| OpenTelemetry.Exporter.OpenTelemetryProtocol | 1.9.0 | ServiceDefaults | Compatible with .NET 10 |
| OpenTelemetry.Extensions.Hosting | 1.9.0 | ServiceDefaults | Compatible with .NET 10 |
| OpenTelemetry.Instrumentation.Runtime | 1.9.0 | ServiceDefaults | Compatible with .NET 10 |

### Package Update Summary

- **Total Packages:** 16
- **Require Update:** 10 (62.5%)
- **Already Compatible:** 6 (37.5%)
- **Security Vulnerabilities:** 0 ?
- **Out of Support:** 2 (Aspire.Hosting.AppHost 9.0.0, Microsoft.Extensions.ServiceDiscovery 9.0.0)

---

## Breaking Changes Catalog

### Source Incompatibilities (Require Code Changes)

#### ?? TimeSpan.FromSeconds - Method Signature Change
- **API:** `System.TimeSpan.FromSeconds(Int64)`
- **Change:** Method changed from accepting `Int64` to requiring `double` in .NET 10
- **Severity:** High (compilation error)
- **Affected Projects:** AspireApp1.Tests
- **Occurrences:** 1
- **Location:** `AspireApp1.Tests\WebTests.cs`, line 21

**Current Code:**
```csharp
TimeSpan.FromSeconds(30)
```

**Required Fix:**
```csharp
TimeSpan.FromSeconds(30.0)
```

**Alternative Fix:**
```csharp
TimeSpan.FromSeconds((double)30)
```

**Reference:** [.NET Breaking Changes Documentation](https://go.microsoft.com/fwlink/?linkid=2262679)

---

### Behavioral Changes (Require Runtime Testing)

#### ?? System.Uri - URL Parsing and Encoding Behavior
- **API:** `System.Uri` type and constructors
- **Change:** URL parsing, validation, and encoding behavior may differ in .NET 10
- **Severity:** Low-Medium (runtime behavior change)
- **Affected Projects:** AspireApp1.FrontEnd
- **Occurrences:** 18

**Affected Locations:**
1. **Program.cs (HttpClient BaseAddress configuration):**
   - Lines 85, 89 (multiple instances): `new Uri(dbApiBase)` and `new Uri("https+http://dbapi")`
   - **Impact:** Service discovery URLs must parse correctly
   - **Testing:** Verify HttpClient BaseAddress configured correctly; API calls succeed

2. **AdminApiClient.cs:**
   - Line 70: `Uri.EscapeDataString(username)`
   - **Impact:** Username encoding for URL query parameters
   - **Testing:** Test with usernames containing special characters

3. **ProjectActivityApiClient.cs:**
   - Line 93: `Uri.EscapeDataString(activityType)`
   - **Impact:** Activity type encoding for URL query parameters
   - **Testing:** Test with activity types containing special characters

**Mitigation Strategy:**
- Test all API client operations
- Verify URLs with special characters
- Confirm `https+http://` Aspire scheme parses correctly

---

#### ?? HttpContent - JSON Deserialization Behavior
- **API:** `System.Net.Http.HttpContent.ReadFromJsonAsync<T>`
- **Change:** JSON deserialization behavior may differ in .NET 10
- **Severity:** Low (runtime behavior change)
- **Affected Projects:** AspireApp1.FrontEnd
- **Occurrences:** 9

**Affected Locations:**
All API client classes reading JSON responses:
- `ProjectsApiClient.cs`: Line 42 - `ReadFromJsonAsync<ProjectDto>`
- `CustomerApiClient.cs`: Lines 174, 195, 216, 237, 258 - Various DTOs
- `AdminApiClient.cs`: Lines 82, 141 - `UserDto`, `RoleDto`
- `ProjectActivityApiClient.cs`: Line 109 - `ProjectActivityDto`

**Pattern:**
```csharp
return await res.Content.ReadFromJsonAsync<T>(ct);
```

**Impact:** DTO deserialization behavior may differ (JSON property mapping, null handling, etc.)

**Mitigation Strategy:**
- Test all API endpoints that return JSON
- Verify DTOs deserialize correctly
- Check for null reference exceptions if nullable handling changed

---

#### ?? ExceptionHandlerExtensions.UseExceptionHandler - Exception Handling Behavior
- **API:** `Microsoft.AspNetCore.Builder.ExceptionHandlerExtensions.UseExceptionHandler`
- **Change:** Exception handling pipeline behavior may differ in .NET 10
- **Severity:** Low (runtime behavior change)
- **Affected Projects:** AspireApp1.ApiService, AspireApp1.BackEnd, AspireApp1.FrontEnd
- **Occurrences:** 3

**Affected Locations:**
1. **ApiService\Program.cs**, line 14: `app.UseExceptionHandler();`
2. **BackEnd\Program.cs**, line 50: `app.UseExceptionHandler("/error");`
3. **FrontEnd\Program.cs**, line 97: `app.UseExceptionHandler("/Error", createScopeForErrors: true);`

**Impact:** Exception handling middleware may behave differently (error response format, logging, scoping)

**Mitigation Strategy:**
- Test error handling by intentionally triggering exceptions
- Verify error pages render correctly (BackEnd `/error`, FrontEnd `/Error`)
- Confirm error responses have expected format

---

### Aspire-Specific Breaking Changes

#### ?? Aspire.Hosting.AppHost 9.0 Å® 13.1 (Major Version Jump)
- **Severity:** Medium (API changes likely)
- **Affected Projects:** AspireApp1.AppHost, AspireApp1.Tests
- **Impact:** Resource builder API changes, configuration method updates, new features

**Potential Changes:**
- Resource builder method signatures (e.g., `AddProject`, `AddPostgres`, `AddNpgsql`)
- Configuration API changes
- Environment variable handling
- Launch profile behavior
- Dashboard integration

**Mitigation Strategy:**
1. **Review Documentation:** [Aspire 13.x What's New](https://learn.microsoft.com/en-us/dotnet/aspire/whats-new/)
2. **Compile and Fix:** Address any compilation errors in AppHost Program.cs
3. **Test Orchestration:** Verify all services launch correctly
4. **Verify Dashboard:** Ensure Aspire dashboard displays all resources

**If Compilation Errors Occur:**
- Check Aspire 13.x migration guide for API renames or deprecations
- Update resource builder calls to match new API
- Update configuration methods if signatures changed

---

### Entity Framework Core Breaking Changes

#### ?? EF Core 8.0 Å® 10.0 (2 Major Versions Jump)
- **Severity:** Low-Medium (migration compatibility, query behavior)
- **Affected Projects:** AspireApp1.BackEnd
- **Impact:** Database migrations, query behavior, database provider compatibility

**Potential Issues:**
- **Migrations:** Existing migrations may need regeneration for EF Core 10
- **Query Behavior:** LINQ query translation may differ
- **Provider Compatibility:** Npgsql.EF 8.0.8 should be compatible with EF Core 10, but verify

**Mitigation Strategy:**
1. **Test Migrations:** Apply existing migrations to test database
2. **Test CRUD Operations:** Verify all database operations work
3. **If Migration Errors Occur:**
   - Regenerate migrations: `dotnet ef migrations add RegenerateMigrations --project AspireApp1.DbApi`
   - Review EF Core 9 Å® 10 breaking changes: [EF Core Breaking Changes](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/breaking-changes)

---

### No Breaking Changes

**These projects have NO expected breaking changes:**
- ? AspireApp1.ServiceDefaults (0 API issues)
- ? AspireApp1.AppHost (0 API issues; Aspire package update is separate concern)

---

### Breaking Changes Summary

| Category | Count | Severity | Action Required |
|----------|-------|----------|-----------------|
| Source Incompatible | 1 | High | Code change required (TimeSpan fix) |
| Behavioral Changes | 38 | Low | Runtime testing required |
| Aspire API Changes | TBD | Medium | Compile-time fixes if API changed |
| EF Core Changes | TBD | Low-Medium | Database testing required |
| **Total Issues** | **39+** |  | **1 code fix + extensive testing** |

---

## Testing & Validation Strategy

### Multi-Level Testing Approach

Testing follows the All-At-Once strategy: upgrade all projects, then test the entire solution as a unified system.

---

### Level 1: Compilation Validation

**Objective:** Verify entire solution compiles with .NET 10.0

**Process:**
1. Update all 6 project files to `<TargetFramework>net10.0</TargetFramework>`
2. Update all package references across all projects
3. Apply required code fix: `TimeSpan.FromSeconds(30.0)` in Tests\WebTests.cs
4. Run `dotnet restore` on solution
5. Run `dotnet build` on solution

**Success Criteria:**
- [ ] `dotnet restore` completes without errors
- [ ] `dotnet build` completes with 0 errors
- [ ] `dotnet build` completes with 0 warnings (ideal; minor warnings acceptable if documented)
- [ ] No package dependency conflicts reported

**If Failures Occur:**
- Review build output for specific errors
- Consult Breaking Changes Catalog
- Address Aspire API changes if AppHost fails to compile
- Fix any unexpected compilation errors

---

### Level 2: Project-Level Validation

**Objective:** Validate each project in dependency order

#### Phase 2.1: ServiceDefaults (Foundation)
**When:** After solution builds
**Tests:**
- [ ] Project builds independently: `dotnet build AspireApp1.ServiceDefaults`
- [ ] No warnings generated
- [ ] Package restore succeeds

**Expected Outcome:** Foundation project ready for consumption by application services

---

#### Phase 2.2: Application Services (Parallel Validation)

**ApiService:**
- [ ] Project builds: `dotnet build AspireApp1.ApiService`
- [ ] No compilation warnings
- [ ] OpenAPI endpoints configured correctly

**BackEnd (DbApi):**
- [ ] Project builds: `dotnet build AspireApp1.DbApi`
- [ ] Database connection string configured
- [ ] EF Core migrations apply: `dotnet ef database update --project AspireApp1.DbApi`
- [ ] No migration errors
- [ ] Swagger/OpenAPI endpoint accessible

**FrontEnd (Web):**
- [ ] Project builds: `dotnet build AspireApp1.Web`
- [ ] Blazor components compile without errors
- [ ] Windows Authentication configured

**Expected Outcome:** All 3 application services build and configure correctly

---

#### Phase 2.3: AppHost (Orchestration)
**When:** After all application services validated
**Tests:**
- [ ] Project builds: `dotnet build AspireApp1.AppHost`
- [ ] No Aspire API compilation errors
- [ ] All project references resolve

**Expected Outcome:** Orchestration layer ready to launch services

---

#### Phase 2.4: Tests
**When:** After AppHost validated
**Tests:**
- [ ] Project builds: `dotnet build AspireApp1.Tests`
- [ ] `TimeSpan.FromSeconds` fix applied correctly
- [ ] Test project compiles without errors

**Expected Outcome:** Test project ready to execute

---

### Level 3: Integration Testing (Runtime Validation)

**Objective:** Verify entire Aspire solution runs correctly

#### Phase 3.1: Aspire Orchestration
**Process:** Launch AppHost and verify all services start

**Tests:**
- [ ] Launch AppHost: `dotnet run --project AspireApp1.AppHost`
- [ ] Aspire dashboard accessible (typically http://localhost:15888)
- [ ] All 3 services appear in dashboard:
  - [ ] ApiService
  - [ ] BackEnd (dbapi)
  - [ ] FrontEnd (webfrontend)
- [ ] PostgreSQL resource available
- [ ] All services show "Running" state
- [ ] No startup errors in logs

**Expected Outcome:** Entire Aspire application stack running

---

#### Phase 3.2: Service Functionality

**ServiceDefaults (Implicit Testing):**
- [ ] Telemetry visible in Aspire dashboard (verify OpenTelemetry working)
- [ ] HTTP resilience policies active (check logs for retry behavior if issues occur)
- [ ] Service discovery functional (services discover each other)

**ApiService:**
- [ ] API responds to requests
- [ ] OpenAPI/Swagger endpoint accessible
- [ ] Exception handler works (test by triggering error)

**BackEnd (DbApi):**
- [ ] API endpoints accessible
- [ ] Database CRUD operations work:
  - [ ] Create entity
  - [ ] Read entity
  - [ ] Update entity
  - [ ] Delete entity
- [ ] Windows Authentication functional (test with AD user)
- [ ] Exception handler works (test `/error` endpoint)
- [ ] Swagger/OpenAPI documentation accessible

**FrontEnd (Web):**
- [ ] Blazor application loads
- [ ] Windows Authentication works (login with AD user)
- [ ] Authorization policies enforce (test AdminOnly pages)
- [ ] API clients work:
  - [ ] ProjectsApiClient - Test GetProjectAsync
  - [ ] CustomerApiClient - Test CRUD operations
  - [ ] AdminApiClient - Test user and role operations
  - [ ] ProjectActivityApiClient - Test activity queries
- [ ] All Blazor components render correctly
- [ ] Interactive components function correctly
- [ ] Exception handler works (test `/Error` page)

#### 7. Validation Checklist
- [ ] Project file updated to `<TargetFramework>net10.0</TargetFramework>`
- [ ] All 4 packages updated to target versions
- [ ] `dotnet restore` succeeds
- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Consuming projects (ApiService, BackEnd, FrontEnd) build successfully
- [ ] Telemetry visible in Aspire dashboard (runtime validation)

---

### Level 4: Automated Test Execution

**Objective:** Run all MSTest test projects

**Process:**
1. Run tests: `dotnet test AspireApp1.Tests`
2. Review test results

**Tests:**
- [ ] All tests pass
- [ ] WebTests.cs passes (verify `TimeSpan.FromSeconds` fix works)
- [ ] Aspire integration tests pass (verify test can launch AppHost)
- [ ] No test failures

**Expected Outcome:** All tests pass with 0 failures

---

### Level 5: Behavioral Change Validation

**Objective:** Validate 38 behavioral changes don't cause runtime issues

#### System.Uri Behavioral Changes (18 instances)
**Test Cases:**
- [ ] HttpClient BaseAddress with `https+http://dbapi` scheme works
- [ ] API client operations with `Uri.EscapeDataString` work:
  - [ ] Usernames with special characters (e.g., `user@domain`)
  - [ ] Activity types with special characters
- [ ] No URL parsing errors in logs
- [ ] Service discovery URLs resolve correctly

#### HttpContent Behavioral Changes (9 instances)
**Test Cases:**
- [ ] All API responses deserialize correctly:
  - [ ] ProjectDto from ProjectsApiClient
  - [ ] CustomerDto, CustomerOrderDto, CustomerSystemDto, CustomerSiteDto, CustomerDatabaseDto from CustomerApiClient
  - [ ] UserDto, RoleDto from AdminApiClient
  - [ ] ProjectActivityDto from ProjectActivityApiClient
- [ ] No JSON deserialization errors
- [ ] Nullable properties handle correctly

#### Exception Handling Behavioral Changes (3 instances)
**Test Cases:**
- [ ] Trigger exceptions in ApiService (verify `UseExceptionHandler()` works)
- [ ] Trigger exceptions in BackEnd (verify `/error` page works)
- [ ] Trigger exceptions in FrontEnd (verify `/Error` page works)
- [ ] Error responses have expected format
- [ ] Error logging works correctly

**Expected Outcome:** No behavioral change regressions detected

---

### Level 6: Entity Framework Core Validation

**Objective:** Verify EF Core 8 Å® 10 upgrade doesn't break database operations

**Test Cases:**
- [ ] Existing migrations apply without errors
- [ ] New migrations can be generated if needed
- [ ] CRUD operations for all entities work:
  - [ ] Projects
  - [ ] Customers
  - [ ] Customer Orders
  - [ ] Customer Systems
  - [ ] Customer Sites
  - [ ] Customer Databases
  - [ ] Project Activities
  - [ ] Users (Admin)
  - [ ] Roles (Admin)
- [ ] Query performance acceptable (no significant regression)
- [ ] Database indexes still optimized
- [ ] PostgreSQL provider (Npgsql) works correctly

**Expected Outcome:** Database operations function correctly with EF Core 10

---

### Test Execution Order

**Sequential Execution (dependency-based):**
1. ? **Compilation** - Must pass before proceeding
2. ? **Project-Level Validation** - Validate in dependency order (ServiceDefaults Å® Services Å® AppHost Å® Tests)
3. ? **Integration Testing** - Launch Aspire stack
4. ? **Automated Tests** - Run MSTest suite
5. ? **Behavioral Validation** - Test all 38 behavioral changes
6. ? **EF Core Validation** - Verify database operations

**Stop Conditions:**
- If compilation fails Å® Fix errors before proceeding to runtime tests
- If Aspire fails to launch Å® Fix orchestration before testing services
- If automated tests fail Å® Investigate failures before considering upgrade complete

---

### Testing Checklist Summary

| Test Level | Tests | Must Pass Before |
|------------|-------|------------------|
| **Compilation** | 4 checks | Proceeding to runtime |
| **Project-Level** | 6 projects Å~ ~3 checks | Integration testing |
| **Integration** | Aspire orchestration + 4 services Å~ ~5 checks | Automated tests |
| **Automated Tests** | MSTest suite | Behavioral validation |
| **Behavioral Changes** | 30+ runtime checks | Considering complete |
| **EF Core** | 10+ database checks | Considering complete |
| **TOTAL** | **100+ validation points** |  |

---

### Acceptance Criteria

Upgrade considered successful when:
- ? Solution builds with 0 errors and 0 warnings
- ? All 6 projects compile and package references resolve
- ? Aspire AppHost launches successfully
- ? All services appear in Aspire dashboard with "Running" state
- ? All automated tests pass (0 failures)
- ? All API clients function correctly
- ? Windows Authentication works
- ? Database operations work (EF Core 10)
- ? No runtime errors related to behavioral changes
- ? Telemetry and monitoring functional

---

## Source Control Strategy

### Branching Strategy

**Primary Branch:** `rename-folders` (source branch - remains unchanged)
**Upgrade Branch:** `upgrade-to-NET10` (target branch - all changes committed here)

**Branch Protection:**
- Source branch (`rename-folders`) remains clean and unchanged
- All upgrade work performed on `upgrade-to-NET10` branch
- Easy rollback: `git checkout rename-folders` if upgrade fails

---

### Commit Strategy (All-At-Once Approach)

**Recommended: Single Atomic Commit**

Given the All-At-Once strategy, the entire upgrade should be committed as a single atomic operation:

**Single Commit Structure:**
```
Upgrade solution to .NET 10.0

- Update all 6 projects from net9.0 to net10.0
- Update 10 NuGet packages to .NET 10-compatible versions
- Fix TimeSpan.FromSeconds source incompatibility in Tests
- Aspire packages: 9.0.0 Å® 13.1.0 (major version jump)
- Entity Framework Core: 8.0.12 Å® 10.0.1
- All compilation errors resolved
- All tests passing

Projects upgraded:
- AspireApp1.ServiceDefaults
- AspireApp1.ApiService
- AspireApp1.BackEnd
- AspireApp1.FrontEnd
- AspireApp1.AppHost
- AspireApp1.Tests

Breaking changes addressed:
- TimeSpan.FromSeconds(Int64) Å® TimeSpan.FromSeconds(double) in WebTests.cs
- 38 behavioral changes validated (System.Uri, HttpContent, ExceptionHandler)
```

**Why Single Commit:**
- ? All-At-Once strategy = atomic operation
- ? Solution cannot function in partial upgrade state
- ? Simplifies rollback (single revert vs. multiple)
- ? Clear upgrade boundary in Git history
- ? Easy to cherry-pick or revert entire upgrade

---

### Alternative: Phased Commits (If Needed)

If issues arise during upgrade, consider breaking into logical commits:

**Commit 1: Project Files and Packages**
```
Update project files and packages to .NET 10.0

- Update TargetFramework to net10.0 in all 6 projects
- Update all 10 NuGet packages to target versions
```

**Commit 2: Breaking Change Fixes**
```
Fix .NET 10 breaking changes

- Fix TimeSpan.FromSeconds source incompatibility
- Address Aspire API changes (if any)
- Fix compilation errors
```

**Commit 3: Verification and Adjustments**
```
Verify .NET 10 upgrade and address runtime issues

- All tests passing
- Behavioral changes validated
- EF Core operations verified
```

**Note:** Phased commits are NOT recommended for All-At-Once strategy but provided as fallback if issues encountered.

---

### File Change Scope

**Files Expected to Change:**
- `AspireApp1.ServiceDefaults/AspireApp1.ServiceDefaults.csproj`
- `AspireApp1.ApiService/AspireApp1.ApiService.csproj`
- `AspireApp1.DbApi/AspireApp1.BackEnd.csproj`
- `AspireApp1.Web/AspireApp1.FrontEnd.csproj`
- `AspireApp1.AppHost/AspireApp1.AppHost.csproj`
- `AspireApp1.Tests/AspireApp1.Tests.csproj`
- `AspireApp1.Tests/WebTests.cs` (TimeSpan fix)
- Possibly: `AspireApp1.AppHost/Program.cs` (if Aspire API changed)

**Files NOT Expected to Change:**
- Solution file (`.sln`) - no changes needed
- Source code (except WebTests.cs)
- Configuration files (`appsettings.json`)
- Database migration files (unless regenerated)

---

### Review and Merge Process

#### Pull Request Requirements

**PR Title:**
```
Upgrade AspireApp1 solution to .NET 10.0 (All-At-Once)
```

**PR Description Template:**
```markdown
## Overview
Upgrade entire AspireApp1 solution from .NET 9.0 to .NET 10.0 using All-At-Once strategy.

## Changes
- **Projects Upgraded:** 6 (ServiceDefaults, ApiService, BackEnd, FrontEnd, AppHost, Tests)
- **Packages Updated:** 10 (Aspire, EF Core, OpenTelemetry, ASP.NET Core)
- **Breaking Changes Fixed:** 1 (TimeSpan.FromSeconds)

## Testing Completed
- [x] Solution builds with 0 errors and 0 warnings
- [x] All 6 projects compile successfully
- [x] Aspire AppHost launches correctly
- [x] All services running in Aspire dashboard
- [x] All MSTest tests pass (0 failures)
- [x] API clients tested (Uri behavioral changes validated)
- [x] Database operations tested (EF Core 10 validated)
- [x] Windows Authentication functional
- [x] Exception handling tested

## Package Updates
- Aspire.Hosting.AppHost: 9.0.0 Å® 13.1.0
- Entity Framework Core: 8.0.12 Å® 10.0.1
- All other packages updated to target versions

## Breaking Changes
- Fixed: TimeSpan.FromSeconds(Int64) Å® TimeSpan.FromSeconds(double) in WebTests.cs
- Validated: 38 behavioral changes (System.Uri, HttpContent, ExceptionHandler)

## Risks
- Low overall risk (all projects Low difficulty, no security vulnerabilities)
- Medium risk: Aspire major version jump (tested and validated)

## Rollback Plan
- Revert this PR or `git checkout rename-folders`
```

#### Review Checklist

- [ ] Review all `.csproj` changes (TargetFramework and PackageReferences)
- [ ] Review TimeSpan.FromSeconds fix in WebTests.cs
- [ ] Verify no unintended changes (e.g., reformatting, unrelated fixes)
- [ ] Confirm test results (screenshot or CI/CD output)
- [ ] Validate package versions match plan
- [ ] Check for merge conflicts with `rename-folders`

#### Merge Criteria

- [ ] All PR checks pass (if CI/CD configured)
- [ ] At least one approval from team member
- [ ] No unresolved comments
- [ ] All tests passing (100% success rate)
- [ ] No merge conflicts

---

### Merge Approach

**Recommended: Squash and Merge**

**Why Squash:**
- All-At-Once upgrade is logically a single change
- Simplifies Git history (one commit = one upgrade)
- Easier to revert if issues found later
- Clean history for future reference

**Squash Commit Message:**
```
Upgrade AspireApp1 solution to .NET 10.0 (#PR_NUMBER)

Upgraded all 6 projects from .NET 9.0 to .NET 10.0 using All-At-Once strategy.

- Updated TargetFramework to net10.0 in all projects
- Updated 10 NuGet packages (Aspire, EF Core, OpenTelemetry, ASP.NET Core)
- Fixed TimeSpan.FromSeconds source incompatibility
- Validated 38 behavioral changes
- All tests passing (100% success rate)

Tested: Entire Aspire solution builds, runs, and functions correctly with .NET 10.0
```

**Alternative: Merge Commit**
- Preserves individual commits if phased commits were used
- Use if detailed commit history is valuable

**NOT Recommended: Rebase and Merge**
- Rewrites commit history
- Complicates rollback if issues arise

---

### Post-Merge Actions

**After Successful Merge to `rename-folders`:**
1. [ ] Delete `upgrade-to-NET10` branch (cleanup)
2. [ ] Update CI/CD pipelines if needed (Docker images, deployment targets)
3. [ ] Update team documentation (note .NET 10.0 requirement)
4. [ ] Notify team of upgrade completion
5. [ ] Monitor production (if deployed) for any runtime issues

---

### Rollback Strategy

**If Issues Found After Merge:**

````````markdown
## Success Criteria

### Technical Criteria

#### Build Success
- ? All 6 projects target `net10.0`
- ? All 10 package updates applied with exact target versions
- ? `dotnet restore` completes without errors or warnings
- ? `dotnet build` completes with 0 errors
- ? `dotnet build` completes with 0 warnings (ideal; minor warnings acceptable if documented)
- ? No package dependency conflicts reported

#### Runtime Success
- ? Aspire AppHost launches successfully
- ? All 3 services appear in Aspire dashboard (ApiService, BackEnd, FrontEnd)
- ? All services show "Running" state in dashboard
- ? PostgreSQL resource available and accessible
- ? Service discovery functional (services communicate correctly)
- ? No startup errors or exceptions in logs

#### Testing Success
- ? All MSTest tests pass (`dotnet test` reports 0 failures)
- ? TimeSpan.FromSeconds fix validated (WebTests.cs test passes)
- ? Aspire integration tests pass (AppHost launch test works)
- ? Test coverage maintained (no tests skipped or disabled)

#### API Compatibility Success
- ? All API endpoints respond correctly
- ? All API client operations succeed:
  - ProjectsApiClient (GetProjectAsync)
  - CustomerApiClient (CRUD operations)
  - AdminApiClient (user and role operations)
  - ProjectActivityApiClient (activity queries)
- ? System.Uri behavioral changes validated (18 instances):
  - HttpClient BaseAddress with `https+http://` scheme works
  - Uri.EscapeDataString works with special characters
- ? HttpContent behavioral changes validated (9 instances):
  - All DTOs deserialize correctly from JSON responses
- ? Exception handling works (3 instances):
  - ApiService UseExceptionHandler() functions
  - BackEnd `/error` endpoint works
  - FrontEnd `/Error` page works

#### Data Layer Success
- ? Entity Framework Core 10.0.1 functional
- ? Database migrations apply successfully (existing or regenerated)
- ? All CRUD operations work for all entities
- ? Query performance acceptable (no significant regression)
- ? PostgreSQL provider (Npgsql) compatible

#### Security & Authentication Success
- ? No security vulnerabilities remain (all packages updated)
- ? Windows Authentication functional (Negotiate package 10.0.1)
- ? Authorization policies enforce correctly (AdminOnly policy works)
- ? User authentication tested with AD credentials

#### Aspire-Specific Success
- ? Aspire.Hosting.AppHost 13.1.0 functional (major version jump validated)
- ? Aspire.Hosting.Testing 13.1.0 functional (test integration works)
- ? Aspire dashboard accessible and displays all resources
- ? Telemetry visible (OpenTelemetry instrumentation working)
- ? Service discovery operational (services find each other)
- ? Resource orchestration functional (PostgreSQL, services configured)

---

### Quality Criteria

#### Code Quality
- ? Code quality maintained (no degradation from .NET 9 baseline)
- ? No code smells introduced by upgrade
- ? Breaking change fixes implemented cleanly (TimeSpan.FromSeconds)
- ? No commented-out code or debug statements left behind

#### Test Coverage
- ? Test coverage maintained at same level as .NET 9
- ? No tests disabled or skipped without justification
- ? All behavioral changes covered by testing strategy (38 instances validated)

#### Documentation
- ? Upgrade plan documented (this document)
- ? Breaking changes cataloged completely
- ? Package updates documented with justifications
- ? Source control strategy followed (commit message describes changes)

---

### Process Criteria

#### Strategy Adherence
- ? All-At-Once strategy followed:
  - All 6 projects upgraded simultaneously
  - No intermediate multi-targeting states
  - Single coordinated build and test cycle
  - Solution moved from .NET 9.0 to .NET 10.0 atomically
- ? Dependency order respected during validation:
  - ServiceDefaults validated first (foundation)
  - Application services validated in parallel
  - AppHost validated after services
  - Tests validated last

#### Source Control
- ? All changes committed to `upgrade-to-NET10` branch
- ? Source branch (`rename-folders`) remains unchanged during upgrade
- ? Single atomic commit preferred (or phased commits if issues encountered)
- ? Commit message describes upgrade comprehensively
- ? PR created with complete description and checklist

#### Review and Approval
- ? Code review completed by at least one team member
- ? All `.csproj` changes validated against plan
- ? Package versions confirmed to match Package Update Reference
- ? Breaking change fixes reviewed (TimeSpan.FromSeconds)
- ? Test results verified (screenshot or CI/CD output provided)

---

### All-At-Once Strategy-Specific Criteria

#### Atomic Operation Validation
- ? All project files updated together (no partial upgrades)
- ? All package references updated together (no mixed versions)
- ? Single build pass identifies all compilation errors
- ? All errors fixed in single pass (reference Breaking Changes Catalog)
- ? Single test cycle validates entire solution

#### No Intermediate States
- ? Solution never in partially-upgraded state
- ? No projects left on .NET 9.0
- ? No multi-targeting (`<TargetFrameworks>net9.0;net10.0</TargetFrameworks>`) used
- ? Clean transition from .NET 9.0 to .NET 10.0

---

### Acceptance Gate

**The upgrade is COMPLETE and ready for merge when:**

1. **All Technical Criteria met** (100% - no exceptions)
2. **All Quality Criteria met** (100% - no exceptions)
3. **All Process Criteria met** (100% - no exceptions)
4. **All-At-Once Strategy Criteria met** (100% - no exceptions)

**If ANY criterion fails:**
- ? Upgrade is NOT complete
- ?? Address failing criteria
- ?? Re-validate all criteria
- ? Only proceed when 100% criteria met

---

### Sign-Off Checklist

**Technical Sign-Off:**
- [ ] DevOps Engineer: Build and deployment validated
- [ ] QA Engineer: Testing strategy executed and passed
- [ ] Developer: Code changes reviewed and approved

**Business Sign-Off:**
- [ ] Product Owner: Upgrade aligns with product roadmap
- [ ] Tech Lead: Upgrade strategy appropriate for solution

**Final Approval:**
- [ ] All sign-offs obtained
- [ ] PR approved and ready to merge
- [ ] Team notified of upgrade completion

---

### Post-Upgrade Success Indicators

**After merge to `rename-folders` branch:**

**Short-Term (First 24 Hours):**
- ? CI/CD pipeline succeeds with .NET 10.0
- ? Deployments to test/staging environments successful
- ? No runtime errors in application logs
- ? Monitoring and telemetry operational

**Medium-Term (First Week):**
- ? No performance degradation observed
- ? No user-reported issues related to upgrade
- ? Database operations stable (EF Core 10)
- ? All behavioral changes validated in production

**Long-Term (First Month):**
- ? Solution stability maintained
- ? Team comfortable with .NET 10.0
- ? No regressions or unexpected issues
- ? Benefits of .NET 10.0 (performance, features) realized

---

### Definition of Done

**Upgrade is DONE when:**

? All success criteria met (100%)
? PR merged to `rename-folders` branch
? `upgrade-to-NET10` branch deleted (cleanup)
? Team notified and documentation updated
? Solution running successfully on .NET 10.0 in all environments
? No outstanding issues or technical debt from upgrade

**?? Congratulations! AspireApp1 successfully upgraded to .NET 10.0!**
