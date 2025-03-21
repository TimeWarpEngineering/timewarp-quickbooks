# Product Context

This file provides a high-level overview of the project and the expected product that will be created. Initially it is based upon projectBrief.md (if provided) and all other available project-related information in the working directory. This file is intended to be updated as the project evolves, and should be used to inform all other modes of the project's goals and context.
2025-03-21 14:58:00 - Initial creation based on repository exploration.

## Project Goal

* Create a .NET library for integrating with QuickBooks Online
* Provide a clean, reusable API for QuickBooks Online integration
* Handle authentication, token management, and API interactions securely

## Key Features

* OAuth 2.0 authentication flow for QuickBooks Online
* Secure token storage and management (access tokens and refresh tokens)
* API integration with QuickBooks Online using IppDotNetSdkForQuickBooksApiV3
* Error handling and logging
* Clean service interfaces for other components to use

## Overall Architecture

* TimeWarp.QuickBooks - Main class library for QuickBooks integration
* Authentication services for OAuth 2.0 flow
* Token management services for secure handling of access and refresh tokens
* API integration services for interacting with QuickBooks Online
* Initially using file-based token storage, with plans to migrate to Entity Framework and PostgreSQL