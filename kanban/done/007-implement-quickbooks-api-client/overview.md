# 007: Implement QuickBooks API Client

## Status: Completed
## Priority: High
## Assigned: 2025-08-07
## Completed: 2025-08-07

## Description
Add QuickBooks API client functionality to the TimeWarp.QuickBooks library to enable direct API communication with QuickBooks Online. This builds upon the existing OAuth authentication functionality to provide a complete solution for interacting with the QuickBooks API.

## Requirements

### Core Components to Implement

1. **IQuickBooksApiClient Interface**
   - Location: `Source/TimeWarp.QuickBooks/Api/IQuickBooksApiClient.cs`
   - Methods: GetAsync, PostAsync, PutAsync, DeleteAsync, QueryAsync
   - Generic return types for flexibility

2. **QuickBooksApiClient Implementation**
   - Location: `Source/TimeWarp.QuickBooks/Api/QuickBooksApiClient.cs`
   - Automatic token refresh via IQuickBooksOAuthService
   - JSON serialization with System.Text.Json
   - Support for sandbox and production environments

3. **QuickBooksApiOptions Configuration**
   - Location: `Source/TimeWarp.QuickBooks/Api/QuickBooksApiOptions.cs`
   - Environment-specific base URLs
   - Timeout and logging configuration
   - API minor version support

4. **QuickBooksHttpClient Typed Client**
   - Location: `Source/TimeWarp.QuickBooks/Api/QuickBooksHttpClient.cs`
   - Configure base URLs and default headers
   - User-Agent and Accept headers
   - Minor version query parameter

5. **QuickBooksApiException**
   - Location: `Source/TimeWarp.QuickBooks/Api/QuickBooksApiException.cs`
   - Parse QuickBooks error response format
   - Include HTTP status codes and error details

6. **Update ServiceCollectionExtensions**
   - Add AddQuickBooksApiClient extension method
   - Register HttpClient and API services

## Technical Details

### Token Management Flow
- Check token expiration before each API call
- Auto-refresh tokens if expired or expiring within 5 minutes
- Update stored tokens after refresh
- Include Bearer token in Authorization header

### API Endpoint Patterns
- Base: `{baseUrl}/v3/company/{realmId}`
- Entity: `/v3/company/{realmId}/{entity}`
- Query: `/v3/company/{realmId}/query?query={query}`

### Error Handling Strategy
- 401: Attempt token refresh and retry once
- 403: Clear permission error message
- 429: Include rate limit information
- 500+: Include request ID for support

## Testing Requirements
- Unit tests for all public methods
- Mock HttpClient responses
- Token refresh scenario tests
- Error parsing verification
- Environment URL construction tests

## Dependencies
- IQuickBooksOAuthService (existing)
- Intuit.Ipp.OAuth2PlatformClient (existing)
- System.Text.Json
- Microsoft.Extensions.Http

## Acceptance Criteria
- [x] API client makes authenticated requests successfully
- [x] Automatic token refresh works seamlessly
- [x] Errors properly parsed as QuickBooksApiException
- [x] Sandbox and production environments supported
- [x] All unit tests passing
- [x] Integration with OAuth service working

## Notes
- This task extends the existing OAuth functionality from task 003
- Follows the same patterns and conventions established in the codebase
- Uses dependency injection and configuration patterns consistent with the project