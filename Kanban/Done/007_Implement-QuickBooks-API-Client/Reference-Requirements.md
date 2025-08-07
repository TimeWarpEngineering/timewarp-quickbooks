# TimeWarp.QuickBooks API Client Implementation

## Overview
Add QuickBooks API client functionality to the TimeWarp.QuickBooks library to enable direct API communication with QuickBooks Online.

## Background
The TimeWarp.QuickBooks library currently provides OAuth authentication functionality via IQuickBooksOAuthService. This task adds API client capabilities to make authenticated requests to QuickBooks API endpoints.

## Requirements

### 1. IQuickBooksApiClient Interface
**Location**: `Source/TimeWarp.QuickBooks/Api/IQuickBooksApiClient.cs`

```csharp
public interface IQuickBooksApiClient
{
    Task<T?> GetAsync<T>(string realmId, string endpoint, CancellationToken cancellationToken = default);
    Task<T?> PostAsync<T>(string realmId, string endpoint, object payload, CancellationToken cancellationToken = default);
    Task<T?> PutAsync<T>(string realmId, string endpoint, object payload, CancellationToken cancellationToken = default);
    Task DeleteAsync(string realmId, string endpoint, CancellationToken cancellationToken = default);
    Task<T?> QueryAsync<T>(string realmId, string query, CancellationToken cancellationToken = default);
}
```

### 2. QuickBooksApiClient Implementation
**Location**: `Source/TimeWarp.QuickBooks/Api/QuickBooksApiClient.cs`

Key features:
- Inject HttpClient and IQuickBooksOAuthService
- Automatically refresh expired tokens using `IQuickBooksOAuthService.RefreshTokensIfNeededAsync`
- Set Authorization header with Bearer token
- Handle JSON serialization/deserialization with System.Text.Json
- Include request/response logging when enabled
- Support both sandbox and production environments

### 3. QuickBooksApiOptions Configuration
**Location**: `Source/TimeWarp.QuickBooks/Api/QuickBooksApiOptions.cs`

```csharp
public class QuickBooksApiOptions
{
    public const string SectionName = "QuickBooksApi";
    
    public string ProductionBaseUrl { get; set; } = "https://quickbooks.api.intuit.com";
    public string SandboxBaseUrl { get; set; } = "https://sandbox-quickbooks.api.intuit.com";
    public QuickBooksEnvironment Environment { get; set; } = QuickBooksEnvironment.Sandbox;
    public int TimeoutSeconds { get; set; } = 30;
    public bool EnableDetailedLogging { get; set; } = false;
    public string MinorVersion { get; set; } = "65";
}
```

### 4. QuickBooksHttpClient Typed Client
**Location**: `Source/TimeWarp.QuickBooks/Api/QuickBooksHttpClient.cs`

Configuration:
- Set base URL based on environment (sandbox vs production)
- Configure default headers:
  - Accept: application/json
  - User-Agent: TimeWarp.QuickBooks/{version}
- Apply timeout from options
- Add minorversion query parameter to all requests

### 5. QuickBooksApiException
**Location**: `Source/TimeWarp.QuickBooks/Api/QuickBooksApiException.cs`

Parse QuickBooks error response format:
```json
{
  "Fault": {
    "Error": [{
      "Message": "Error message",
      "Detail": "Detailed error information",
      "code": "ERROR_CODE"
    }],
    "type": "ValidationFault"
  }
}
```

Include:
- HTTP status code
- Error code and message
- Request URL
- Inner exception if applicable

### 6. Update ServiceCollectionExtensions
**Location**: `Source/TimeWarp.QuickBooks/Authentication/ServiceCollectionExtensions.cs`

Add new extension method:
```csharp
public static IServiceCollection AddQuickBooksApiClient(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.Configure<QuickBooksApiOptions>(
        configuration.GetSection(QuickBooksApiOptions.SectionName));
    
    services.AddHttpClient<QuickBooksHttpClient>();
    services.AddScoped<IQuickBooksApiClient, QuickBooksApiClient>();
    
    return services;
}
```

## Implementation Details

### Token Management
- Before each API call, check token expiration using `QuickBooksTokens.ExpiresAt`
- If expired or about to expire (within 5 minutes), call `IQuickBooksOAuthService.RefreshTokensIfNeededAsync`
- Update stored tokens after refresh
- Include refreshed access token in Authorization header

### API Endpoint Construction
QuickBooks API v3 endpoints follow this pattern:
- Base: `{baseUrl}/v3/company/{realmId}`
- Entity endpoints: `/v3/company/{realmId}/{entity}`
- Query endpoint: `/v3/company/{realmId}/query?query={query}`

### Request Headers
All requests should include:
- `Authorization: Bearer {accessToken}`
- `Accept: application/json`
- `Content-Type: application/json` (for POST/PUT)

### Error Handling
- 401 Unauthorized: Attempt token refresh once, then retry
- 403 Forbidden: Throw with clear message about permissions
- 429 Too Many Requests: Include rate limit information
- 500+ Server Errors: Include request ID for support

## Testing Requirements

1. Unit tests for all public methods
2. Mock HttpClient responses
3. Test token refresh scenarios
4. Verify error parsing
5. Test both sandbox and production URL construction

## Dependencies
- Existing IQuickBooksOAuthService for token management
- Intuit.Ipp.OAuth2PlatformClient (already in use)
- System.Text.Json for serialization
- Microsoft.Extensions.Http for typed HttpClient

## Success Criteria
- [ ] API client can make authenticated requests to QuickBooks
- [ ] Tokens are automatically refreshed when expired
- [ ] Errors are properly parsed and thrown as QuickBooksApiException
- [ ] Both sandbox and production environments are supported
- [ ] All unit tests pass
- [ ] Integration with existing OAuth service works seamlessly