# Decision Log

This file records architectural and implementation decisions using a list format.
2025-03-21 15:00:00 - Initial creation based on repository exploration.

## 2025-03-21: Project Structure Decision

### Decision
* Create a separate test project (TimeWarp.QuickBooks.Tests) using TimeWarp.Fixie framework
* Organize the project with Source and Tests directories
* Target .NET 9.0 for both the main library and tests

### Rationale
* Following TimeWarp conventions for project organization
* Using TimeWarp.Fixie provides a consistent testing approach across TimeWarp projects
* .NET 9.0 provides the latest features and performance improvements

### Implementation Details
* Created TimeWarp.QuickBooks.Tests with TimeWarp.Fixie, Fixie.TestAdapter, and Shouldly
* Set up TestingConvention class inheriting from TimeWarp.Fixie.TestingConvention
* Created sample tests to verify the testing infrastructure

## 2025-03-21: QuickBooks Integration Approach

### Decision
* Use IppDotNetSdkForQuickBooksApiV3 NuGet package for QuickBooks API integration
* Implement OAuth 2.0 authentication flow
* Initially use file-based token storage, with plans to migrate to Entity Framework and PostgreSQL

### Rationale
* IppDotNetSdkForQuickBooksApiV3 is the official SDK for QuickBooks API
* OAuth 2.0 is required for QuickBooks Online authentication
* File-based storage allows for quick implementation during the spike phase
* Future migration to database storage will provide better security and scalability

### Implementation Details
* Will create authentication service to handle OAuth flow
* Will implement token management service for secure handling of tokens
* Will provide clean service interfaces for other components to use