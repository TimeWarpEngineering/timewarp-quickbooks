using TimeWarp.QuickBooks.Authentication;
using TimeWarp.QuickBooks.Authentication.Models;
using TimeWarp.QuickBooks.Api;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add QuickBooks OAuth service - now both methods use root configuration consistently
builder.Services.AddQuickBooksOAuth(builder.Configuration);

// Add QuickBooks API client service
builder.Services.AddQuickBooksApiClient(builder.Configuration);

var app = builder.Build();

// Home page with links to various features
app.MapGet("/", async (IQuickBooksOAuthService oauthService, HttpContext context) =>
{
  // Check if we have any stored tokens
  var hasTokens = false;
  var realmIds = new List<string>();
  string? cookieRealmId = null;
  
  // Check for realm ID in cookie
  context.Request.Cookies.TryGetValue("RealmId", out cookieRealmId);
  
  // Try to get tokens using the cookie realm ID first
  if (!string.IsNullOrEmpty(cookieRealmId))
  {
    try
    {
      var tokens = await oauthService.GetTokensAsync(cookieRealmId);
      if (tokens != null)
      {
        hasTokens = true;
        realmIds.Add(cookieRealmId);
      }
    }
    catch { }
  }
  
  // Also check test-realm-id as fallback
  if (!hasTokens)
  {
    try
    {
      var testToken = await oauthService.GetTokensAsync("test-realm-id");
      if (testToken != null)
      {
        hasTokens = true;
        realmIds.Add("test-realm-id");
      }
    }
    catch { }
  }
  
  return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>QuickBooks API Sample</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        .button {{ 
            display: inline-block; 
            background: #2E86C1; 
            color: white; 
            padding: 10px 20px; 
            text-decoration: none; 
            border-radius: 5px; 
            margin: 5px;
        }}
        .section {{ 
            border: 1px solid #ddd; 
            padding: 20px; 
            margin: 20px 0; 
            border-radius: 5px;
        }}
        .success {{ color: green; }}
        .error {{ color: red; }}
    </style>
</head>
<body>
    <h1>QuickBooks API Sample Application</h1>
    
    <div class=""section"">
        <h2>1. OAuth Authentication</h2>
        <p>First, authorize with QuickBooks to get access tokens:</p>
        <a href=""/auth"" class=""button"">Authorize with QuickBooks</a>
        {(hasTokens ? $"<p class='success'>Tokens found for Realm ID: {string.Join(", ", realmIds)}</p>" : "")}
        {(!string.IsNullOrEmpty(cookieRealmId) ? $"<p>Cookie Realm ID: {cookieRealmId}</p>" : "<p>No Realm ID in cookie</p>")}
    </div>
    
    {(hasTokens ? @"
    <div class='section'>
        <h2>2. API Client Demonstrations</h2>
        <p>Now that you're authorized, try these API operations:</p>
        <a href='/api/company-info' class='button'>Get Company Info</a>
        <a href='/api/customers' class='button'>List Customers</a>
        <a href='/api/test-query' class='button'>Test Query</a>
        <a href='/api/token-status' class='button'>Check Token Status</a>
    </div>
    " : @"
    <div class='section'>
        <h2>2. API Client Demonstrations</h2>
        <p class='error'>Please authorize first to access API demonstrations.</p>
    </div>
    ")}
    
    <div class=""section"">
        <h2>About This Sample</h2>
        <p>This application demonstrates:</p>
        <ul>
            <li>OAuth 2.0 authentication with QuickBooks</li>
            <li>Automatic token refresh</li>
            <li>API client operations (GET, POST, Query)</li>
            <li>Error handling</li>
        </ul>
    </div>
</body>
</html>
", "text/html");
});

// Start OAuth flow
app.MapGet("/auth", (IQuickBooksOAuthService oauthService, HttpContext context) =>
{
  // Generate a random state value to prevent CSRF
  string state = Guid.NewGuid().ToString();
  
  // Store the state in session or cookie for verification later
  context.Response.Cookies.Append("OAuthState", state, new CookieOptions
  {
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.Lax
  });
  
  string authUrl = oauthService.GenerateAuthorizationUrl(state);
  return Results.Redirect(authUrl);
});

// OAuth callback - save tokens and redirect to success page
app.MapGet("/callback", async (HttpContext context, IQuickBooksOAuthService oauthService, ILogger<Program> logger) =>
{
  string? code = context.Request.Query["code"];
  string? state = context.Request.Query["state"];
  string? realmId = context.Request.Query["realmId"];
  
  logger.LogInformation("OAuth callback received - Code: {HasCode}, State: {HasState}, RealmId: {RealmId}", 
    !string.IsNullOrEmpty(code), !string.IsNullOrEmpty(state), realmId);
  
  // Retrieve the stored state
  context.Request.Cookies.TryGetValue("OAuthState", out string? expectedState);
  
  if (string.IsNullOrEmpty(code))
  {
    return Results.Content("<h1>Error</h1><p>Authorization code is missing</p>", "text/html");
  }
  
  if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(expectedState) || state != expectedState)
  {
    return Results.Content("<h1>Error</h1><p>Invalid state parameter</p>", "text/html");
  }
  
  QuickBooksOAuthCallbackResult result = await oauthService.HandleCallbackAsync(
    code,
    state,
    realmId ?? string.Empty,
    expectedState
  );
  
  if (!result.IsSuccess)
  {
    return Results.Content($"<h1>Error</h1><p>{result.ErrorMessage}</p>", "text/html");
  }
  
  // Store the realm ID in a cookie for demo purposes
  // In production, you'd store this properly in a database
  context.Response.Cookies.Append("RealmId", realmId ?? "", new CookieOptions
  {
    HttpOnly = true,
    Secure = true,
    SameSite = SameSiteMode.Lax
  });
  
  // Display success and redirect to home
  return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Authorization Successful</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        .success {{ color: green; }}
        pre {{ background: #f4f4f4; padding: 10px; border-radius: 5px; overflow-x: auto; }}
    </style>
    <meta http-equiv=""refresh"" content=""3;url=/"" />
</head>
<body>
    <h1 class=""success"">Authorization Successful!</h1>
    <p>Realm ID: <strong>{result.RealmId}</strong></p>
    <p>Tokens have been saved. Redirecting to home page...</p>
    <p><a href=""/"">Click here if not redirected</a></p>
</body>
</html>
", "text/html");
});

// API endpoint: Get Company Info
app.MapGet("/api/company-info", async (IQuickBooksApiClient apiClient, IQuickBooksOAuthService oauthService, HttpContext context) =>
{
  try
  {
    // Get realm ID from cookie (in production, get from proper storage)
    context.Request.Cookies.TryGetValue("RealmId", out string? realmId);
    if (string.IsNullOrEmpty(realmId))
    {
      // Try to get any stored tokens for demo
      var tokens = await oauthService.GetTokensAsync("test-realm-id");
      realmId = tokens?.RealmId ?? "";
    }
    
    if (string.IsNullOrEmpty(realmId))
    {
      return Results.Content(@"
        <h1>Error</h1>
        <p>No realm ID found. Please <a href='/auth'>authorize</a> first.</p>
      ", "text/html");
    }
    
    // Get company info
    var companyInfo = await apiClient.GetAsync<JsonElement?>(realmId, "companyinfo/1");
    
    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    var formattedJson = JsonSerializer.Serialize(companyInfo, jsonOptions);
    
    return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Company Info</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        pre {{ background: #f4f4f4; padding: 15px; border-radius: 5px; overflow-x: auto; }}
        .back {{ margin-bottom: 20px; }}
    </style>
</head>
<body>
    <div class=""back"">
        <a href=""/"">&lt; Back to Home</a>
    </div>
    <h1>Company Information</h1>
    <p>Retrieved from QuickBooks API:</p>
    <pre>{formattedJson}</pre>
</body>
</html>
", "text/html");
  }
  catch (QuickBooksApiException ex)
  {
    return Results.Content($@"
      <h1>API Error</h1>
      <p>Error Code: {ex.ErrorCode}</p>
      <p>Error Type: {ex.ErrorType}</p>
      <p>Message: {ex.Message}</p>
      <p>Detail: {ex.ErrorDetail}</p>
      <p><a href=""/"">&lt; Back to Home</a></p>
    ", "text/html");
  }
  catch (Exception ex)
  {
    return Results.Content($@"
      <h1>Error</h1>
      <p>{ex.Message}</p>
      <p><a href=""/"">&lt; Back to Home</a></p>
    ", "text/html");
  }
});

// API endpoint: List Customers
app.MapGet("/api/customers", async (IQuickBooksApiClient apiClient, IQuickBooksOAuthService oauthService, HttpContext context) =>
{
  try
  {
    // Get realm ID from cookie
    context.Request.Cookies.TryGetValue("RealmId", out string? realmId);
    if (string.IsNullOrEmpty(realmId))
    {
      var tokens = await oauthService.GetTokensAsync("test-realm-id");
      realmId = tokens?.RealmId ?? "";
    }
    
    if (string.IsNullOrEmpty(realmId))
    {
      return Results.Content(@"
        <h1>Error</h1>
        <p>No realm ID found. Please <a href='/auth'>authorize</a> first.</p>
      ", "text/html");
    }
    
    // Query for customers
    var customers = await apiClient.QueryAsync<JsonElement?>(realmId, "SELECT * FROM Customer MAXRESULTS 10");
    
    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    var formattedJson = JsonSerializer.Serialize(customers, jsonOptions);
    
    return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Customers</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        pre {{ background: #f4f4f4; padding: 15px; border-radius: 5px; overflow-x: auto; }}
        .back {{ margin-bottom: 20px; }}
    </style>
</head>
<body>
    <div class=""back"">
        <a href=""/"">&lt; Back to Home</a>
    </div>
    <h1>Customer List</h1>
    <p>Query: SELECT * FROM Customer MAXRESULTS 10</p>
    <pre>{formattedJson}</pre>
</body>
</html>
", "text/html");
  }
  catch (Exception ex)
  {
    return Results.Content($@"
      <h1>Error</h1>
      <p>{ex.Message}</p>
      <p><a href=""/"">&lt; Back to Home</a></p>
    ", "text/html");
  }
});

// API endpoint: Test Query
app.MapGet("/api/test-query", (HttpContext context) =>
{
  return Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>Test Query</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; }
        input, select { padding: 5px; margin: 5px; }
        button { background: #2E86C1; color: white; padding: 8px 15px; border: none; border-radius: 5px; cursor: pointer; }
        pre { background: #f4f4f4; padding: 15px; border-radius: 5px; overflow-x: auto; }
    </style>
</head>
<body>
    <h1>Test Custom Query</h1>
    <form action=""/api/execute-query"" method=""get"">
        <label>Enter Query:</label><br/>
        <input type=""text"" name=""query"" style=""width: 500px;"" 
               value=""SELECT * FROM Account WHERE AccountType = 'Income' MAXRESULTS 5"" /><br/>
        <button type=""submit"">Execute Query</button>
    </form>
    <div style=""margin-top: 20px;"">
        <h3>Example Queries:</h3>
        <ul>
            <li>SELECT * FROM Customer MAXRESULTS 5</li>
            <li>SELECT * FROM Invoice WHERE TotalAmt > '100.00' MAXRESULTS 5</li>
            <li>SELECT * FROM Account WHERE AccountType = 'Income'</li>
            <li>SELECT Count(*) FROM Customer</li>
        </ul>
    </div>
    <p><a href=""/"">&lt; Back to Home</a></p>
</body>
</html>
", "text/html");
});

// API endpoint: Execute Query
app.MapGet("/api/execute-query", async (string query, IQuickBooksApiClient apiClient, IQuickBooksOAuthService oauthService, HttpContext context) =>
{
  try
  {
    // Get realm ID from cookie
    context.Request.Cookies.TryGetValue("RealmId", out string? realmId);
    if (string.IsNullOrEmpty(realmId))
    {
      var tokens = await oauthService.GetTokensAsync("test-realm-id");
      realmId = tokens?.RealmId ?? "";
    }
    
    if (string.IsNullOrEmpty(realmId))
    {
      return Results.Content(@"
        <h1>Error</h1>
        <p>No realm ID found. Please <a href='/auth'>authorize</a> first.</p>
      ", "text/html");
    }
    
    // Execute the query
    var result = await apiClient.QueryAsync<JsonElement?>(realmId, query);
    
    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
    var formattedJson = JsonSerializer.Serialize(result, jsonOptions);
    
    return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Query Results</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        pre {{ background: #f4f4f4; padding: 15px; border-radius: 5px; overflow-x: auto; }}
        .query {{ background: #e8f4f8; padding: 10px; border-left: 4px solid #2E86C1; margin-bottom: 20px; }}
        .nav {{ margin-bottom: 20px; }}
    </style>
</head>
<body>
    <div class=""nav"">
        <a href=""/api/test-query"">&lt; Try Another Query</a> | <a href=""/"">Home</a>
    </div>
    <h1>Query Results</h1>
    <div class=""query"">
        <strong>Query:</strong> {query}
    </div>
    <pre>{formattedJson}</pre>
</body>
</html>
", "text/html");
  }
  catch (Exception ex)
  {
    return Results.Content($@"
      <h1>Query Error</h1>
      <p><strong>Query:</strong> {query}</p>
      <p><strong>Error:</strong> {ex.Message}</p>
      <p><a href=""/api/test-query"">&lt; Try Another Query</a> | <a href=""/"">Home</a></p>
    ", "text/html");
  }
});

// API endpoint: Token Status
app.MapGet("/api/token-status", async (IQuickBooksOAuthService oauthService, HttpContext context) =>
{
  try
  {
    // Get realm ID from cookie
    context.Request.Cookies.TryGetValue("RealmId", out string? realmId);
    if (string.IsNullOrEmpty(realmId))
    {
      // Try test realm
      realmId = "test-realm-id";
    }
    
    var tokens = await oauthService.GetTokensAsync(realmId);
    
    if (tokens == null)
    {
      return Results.Content(@"
        <h1>No Tokens Found</h1>
        <p>Please <a href='/auth'>authorize</a> first.</p>
        <p><a href=""/"">&lt; Back to Home</a></p>
      ", "text/html");
    }
    
    var isExpired = tokens.IsExpired();
    var expiresAt = tokens.IssuedUtc.AddSeconds(tokens.ExpiresIn);
    var refreshExpiresAt = tokens.IssuedUtc.AddSeconds(tokens.RefreshTokenExpiresIn);
    var timeUntilExpiry = expiresAt - DateTime.UtcNow;
    var timeUntilRefreshExpiry = refreshExpiresAt - DateTime.UtcNow;
    
    return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Token Status</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        .status {{ padding: 20px; border-radius: 5px; margin: 10px 0; }}
        .valid {{ background: #d4edda; border: 1px solid #c3e6cb; }}
        .expired {{ background: #f8d7da; border: 1px solid #f5c6cb; }}
        table {{ border-collapse: collapse; width: 100%; }}
        td, th {{ padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }}
        .back {{ margin-bottom: 20px; }}
    </style>
</head>
<body>
    <div class=""back"">
        <a href=""/"">&lt; Back to Home</a>
    </div>
    <h1>Token Status</h1>
    
    <div class=""status {(isExpired ? "expired" : "valid")}"">
        <h2>Access Token: {(isExpired ? "EXPIRED" : "VALID")}</h2>
    </div>
    
    <table>
        <tr>
            <th>Property</th>
            <th>Value</th>
        </tr>
        <tr>
            <td>Realm ID</td>
            <td>{tokens.RealmId}</td>
        </tr>
        <tr>
            <td>Token Type</td>
            <td>{tokens.TokenType}</td>
        </tr>
        <tr>
            <td>Issued At</td>
            <td>{tokens.IssuedUtc:yyyy-MM-dd HH:mm:ss} UTC</td>
        </tr>
        <tr>
            <td>Access Token Expires At</td>
            <td>{expiresAt:yyyy-MM-dd HH:mm:ss} UTC</td>
        </tr>
        <tr>
            <td>Time Until Expiry</td>
            <td>{(timeUntilExpiry.TotalMinutes > 0 ? $"{timeUntilExpiry.TotalMinutes:0} minutes" : "EXPIRED")}</td>
        </tr>
        <tr>
            <td>Refresh Token Expires At</td>
            <td>{refreshExpiresAt:yyyy-MM-dd HH:mm:ss} UTC</td>
        </tr>
        <tr>
            <td>Time Until Refresh Token Expiry</td>
            <td>{timeUntilRefreshExpiry.TotalDays:0} days</td>
        </tr>
    </table>
    
    <p style=""margin-top: 20px;"">
        <strong>Note:</strong> The API client will automatically refresh the access token when it expires.
    </p>
</body>
</html>
", "text/html");
  }
  catch (Exception ex)
  {
    return Results.Content($@"
      <h1>Error</h1>
      <p>{ex.Message}</p>
      <p><a href=""/"">&lt; Back to Home</a></p>
    ", "text/html");
  }
});

app.Run();