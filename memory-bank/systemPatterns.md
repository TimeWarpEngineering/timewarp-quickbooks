# System Patterns

This file documents recurring patterns and standards used in the project.
It is optional, but recommended to be updated as the project evolves.
2025-03-21 15:01:00 - Initial creation based on repository exploration.

## Coding Patterns

* Using .NET 9.0 features including nullable reference types and implicit usings
* Following TimeWarp conventions for project organization and naming
* Using dependency injection for service resolution
* Using interfaces for service abstractions to promote testability

## Architectural Patterns

* Service-oriented architecture for QuickBooks integration
* Separation of concerns:
  - Authentication service for OAuth flow
  - Token management service for secure token handling
  - API integration services for QuickBooks interactions
* Repository pattern for token storage (initially file-based, later database)
* Clean API design for other components to interact with QuickBooks

## Testing Patterns

* Using TimeWarp.Fixie testing framework
* Test classes named with "_Should_" suffix
* Using Shouldly for assertions
* Using TestTag attribute for test categorization
* Using Skip attribute for tests that should be skipped
* Using Input attribute for parameterized tests