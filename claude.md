# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TimeWarp.QuickBooks is a .NET 9.0 library for QuickBooks Online integration. It provides OAuth authentication and data access capabilities for the QuickBooks API using the official Intuit SDK (`IppDotNetSdkForQuickBooksApiV3`).

## Common Commands

### Build and Test
- **Run tests**: `pwsh run-tests.ps1` or `dotnet fixie tests/timewarp-quickbooks-tests`
- **Build solution**: `dotnet build timewarp-quickbooks.slnx`
- **Run sample app**: `cd samples/timewarp-quickbooks-sample-web && dotnet run`

### Project Structure
- **Main library**: `source/timewarp-quickbooks/`
- **Tests**: `tests/timewarp-quickbooks-tests/`
- **Sample application**: `samples/timewarp-quickbooks-sample-web/`

## Architecture

### Core Components

**Authentication Layer** (`source/timewarp-quickbooks/authentication/`):
- `IQuickBooksOAuthService`: Interface for OAuth operations (authorization URL generation, token handling, callback processing)
- `QuickBooksOAuthService`: Implementation using Intuit's OAuth2Client
- `Models/`: OAuth-related models including `QuickBooksTokens`, `QuickBooksOAuthOptions`, and `QuickBooksOAuthCallbackResult`

**Service Registration**:
- `ServiceCollectionExtensions.cs`: Contains `AddQuickBooksOAuth()` extension method for DI registration

### Key Patterns

**Token Management**: The service uses an in-memory dictionary for token storage. Production implementations should replace `TokenStore` with persistent storage.

**OAuth Flow**: Standard OAuth 2.0 flow with state verification for CSRF protection. Supports both sandbox and production environments.

**Configuration**: Uses .NET Options pattern with `QuickBooksOAuthOptions` for environment-specific settings.

### Testing Framework

Uses **TimeWarp.Fixie** testing framework with:
- `[TestTag]` attributes for test categorization (Fast, Slow, etc.)
- `[Input]` attributes for parameterized tests
- `[Skip]` attributes for disabled tests
- Shouldly assertions

### Dependencies

**Main Library**:
- `IppDotNetSdkForQuickBooksApiV3`: Official Intuit QuickBooks SDK

**Test Project**:
- `TimeWarp.Fixie`: Custom testing framework
- `Shouldly`: Assertion library
- `Fixie.TestAdapter`: Test runner integration

## Configuration

**Global Settings** (`Directory.Build.props`):
- Target Framework: .NET 9.0
- Nullable reference types enabled
- Warnings treated as errors
- Assembly version: 0.1.2.0

**Sample App Configuration**:
- QuickBooks OAuth settings in `appsettings.json`
- Environment support (Sandbox/Production)
- Scope configuration for QuickBooks permissions

## Development Notes

**Kanban Workflow**: Project uses Kanban methodology with tasks organized in `Kanban/` directory (ToDo, InProgress, Done).

**Documentation**: Comprehensive documentation in `Documentation/` with separate User and Developer sections.

**CI/CD**: GitHub Actions workflows for CI builds and NuGet publishing (requires `PUBLISH_TO_NUGET_ORG` secret).