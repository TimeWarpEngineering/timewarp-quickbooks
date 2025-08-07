# TimeWarp.QuickBooks.Sample.Web

This sample application demonstrates how to use the TimeWarp.QuickBooks library for both OAuth authentication and API client operations with QuickBooks Online.

## Overview

This ASP.NET Core web application showcases the complete integration flow:

1. **OAuth Authentication**: Secure authorization with QuickBooks Online
2. **API Client Operations**: Real-time API calls to retrieve data from QuickBooks
3. **Token Management**: Automatic token refresh and status monitoring
4. **Error Handling**: Proper handling of API errors and exceptions

## Features Demonstrated

### OAuth Authentication Flow
- Generate authorization URL with CSRF protection
- Handle OAuth callback with state verification
- Secure token storage and management
- Automatic redirect after successful authorization

### API Client Operations
- **Company Information**: Retrieve basic company details
- **Customer Queries**: List customers using SQL-like queries
- **Custom Queries**: Interactive query builder for testing
- **Token Status**: Monitor token expiration and refresh status

### Advanced Features
- Automatic token refresh when expired
- Comprehensive error handling with QuickBooksApiException
- Support for both Sandbox and Production environments
- Detailed logging and debugging information

## User Experience

### 1. Home Page
- Overview of available features
- Status indicator showing if tokens are available
- Navigation to OAuth authorization and API demonstrations

### 2. OAuth Flow
- **Authorization**: Redirects to QuickBooks for user consent
- **Callback**: Processes authorization code and exchanges for tokens
- **Success**: Displays confirmation and redirects to main interface

### 3. API Demonstrations
- **Get Company Info**: `/api/company-info` - Retrieves basic company information
- **List Customers**: `/api/customers` - Queries first 10 customers
- **Test Custom Query**: `/api/test-query` - Interactive SQL query interface
- **Token Status**: `/api/token-status` - Monitor token expiration and health

## Configuration

### Required Settings

#### Option 1: User Secrets (Recommended for Development)

The project is configured to use .NET User Secrets. Set your credentials securely:

```bash
cd Samples/TimeWarp.QuickBooks.Sample.Web

# Set your QuickBooks credentials
dotnet user-secrets set "QuickBooks:ClientId" "your-actual-client-id"
dotnet user-secrets set "QuickBooks:ClientSecret" "your-actual-client-secret"

# Optional: Override other settings
dotnet user-secrets set "QuickBooks:RedirectUri" "https://localhost:5024/callback"
dotnet user-secrets set "QuickBooks:Environment" "Sandbox"
```

#### Option 2: Configuration File

Alternatively, update `appsettings.json` (avoid committing credentials):

```json
{
  "QuickBooks": {
    "ClientId": "your-client-id-from-intuit-developer",
    "ClientSecret": "your-client-secret-from-intuit-developer", 
    "RedirectUri": "https://localhost:5024/callback",
    "Environment": "Sandbox",
    "Scopes": ["Accounting"]
  },
  "QuickBooksApi": {
    "Environment": "Sandbox",
    "TimeoutSeconds": 30,
    "EnableDetailedLogging": true,
    "MinorVersion": "65"
  }
}
```

### QuickBooks Developer Setup

1. **Create App**: Go to [Intuit Developer Console](https://developer.intuit.com/)
2. **Get Credentials**: Note your Client ID and Client Secret
3. **Set Redirect URI**: Add `https://localhost:5024/callback` to allowed redirect URIs
4. **Environment**: Start with Sandbox for testing

## Running the Sample

### Prerequisites
- .NET 9.0 or later
- Valid QuickBooks Developer App credentials
- Access to a QuickBooks Online Sandbox company

### Steps
1. **Clone and Navigate**:
   ```bash
   cd Samples/TimeWarp.QuickBooks.Sample.Web
   ```

2. **Update Configuration**:
   - Edit `appsettings.json` with your QuickBooks app credentials
   - Ensure redirect URI matches your developer console settings

3. **Run Application**:
   ```bash
   dotnet run
   ```

4. **Test OAuth Flow**:
   - Navigate to `https://localhost:5024`
   - Click "Authorize with QuickBooks"
   - Complete QuickBooks authorization
   - Return to see available API demonstrations

5. **Test API Operations**:
   - Try "Get Company Info" to see basic company data
   - Use "List Customers" to see customer query results
   - Experiment with "Test Query" for custom queries
   - Monitor "Token Status" to see token health

## Example API Calls

The sample demonstrates these QuickBooks API patterns:

### Company Information
```csharp
var companyInfo = await apiClient.GetAsync<JsonElement?>(realmId, "companyinfo/1");
```

### Customer Query
```csharp
var customers = await apiClient.QueryAsync<JsonElement?>(realmId, "SELECT * FROM Customer MAXRESULTS 10");
```

### Custom Queries
Users can test queries like:
- `SELECT * FROM Account WHERE AccountType = 'Income'`
- `SELECT * FROM Invoice WHERE TotalAmt > '100.00' MAXRESULTS 5`
- `SELECT Count(*) FROM Customer`

## Implementation Details

### Service Registration
```csharp
// Add OAuth authentication
builder.Services.AddQuickBooksOAuth(options => { ... });

// Add API client
builder.Services.AddQuickBooksApiClient(builder.Configuration);
```

### Automatic Token Refresh
The API client automatically:
- Checks token expiration before each request
- Refreshes tokens when expired (within 5-minute buffer)
- Retries failed requests once after token refresh
- Updates stored tokens with fresh credentials

### Error Handling
```csharp
try
{
  var result = await apiClient.GetAsync<JsonElement?>(realmId, endpoint);
  // Process result
}
catch (QuickBooksApiException ex)
{
  // Handle specific QuickBooks API errors
  Console.WriteLine($"API Error {ex.ErrorCode}: {ex.Message}");
}
```

## Security Considerations

- **State Parameter**: CSRF protection using random state values
- **Secure Cookies**: HttpOnly, Secure, SameSite protection for sensitive data
- **Token Storage**: In-memory storage for demo (use secure database in production)
- **HTTPS Only**: Application enforces HTTPS for OAuth flow

## Troubleshooting

### Common Issues

1. **"Invalid Redirect URI"**
   - Ensure redirect URI in config matches developer console exactly
   - Check for trailing slashes and case sensitivity

2. **"Invalid Client Credentials"**
   - Verify Client ID and Client Secret are correct
   - Ensure you're using Sandbox credentials for Sandbox environment

3. **"No Tokens Found"**
   - Complete OAuth flow first before trying API calls
   - Tokens are stored in memory and reset when app restarts

4. **API Request Failures**
   - Check token expiration in "Token Status"
   - Verify realm ID is correct for your test company
   - Review error details in QuickBooksApiException

### Debug Mode
Set `"EnableDetailedLogging": true` in appsettings.json to see:
- HTTP request/response details
- Token refresh operations
- API endpoint construction
- Error response parsing

## Next Steps

After successfully running this sample:

1. **Explore API Endpoints**: Try different QuickBooks entity queries
2. **Implement Business Logic**: Build actual business features using the patterns shown
3. **Add Persistent Storage**: Replace in-memory token storage with database
4. **Error Recovery**: Implement robust error handling for production scenarios
5. **Scale Considerations**: Plan for multiple users and concurrent API calls

## Related Documentation

- [QuickBooks API Reference](https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities)
- [OAuth 2.0 Guide](https://developer.intuit.com/app/developer/qbo/docs/develop/authentication-and-authorization/oauth-2.0)
- [TimeWarp.QuickBooks Library Documentation](../../README.md)