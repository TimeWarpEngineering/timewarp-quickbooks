using TimeWarp.QuickBooks.Authentication;
using TimeWarp.QuickBooks.Authentication.Models;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add QuickBooks OAuth service
builder.Services.AddQuickBooksOAuth(options =>
{
  options.ClientId = builder.Configuration["QuickBooks:ClientId"] ?? string.Empty;
  options.ClientSecret = builder.Configuration["QuickBooks:ClientSecret"] ?? string.Empty;
  options.RedirectUri = builder.Configuration["QuickBooks:RedirectUri"] ?? string.Empty;
  options.Environment = Enum.TryParse<QuickBooksEnvironment>(builder.Configuration["QuickBooks:Environment"], out var env)
    ? env
    : QuickBooksEnvironment.Sandbox;
  options.Scopes = builder.Configuration.GetSection("QuickBooks:Scopes").Get<List<string>>() ?? new List<string>();
});

var app = builder.Build();

// Home page with a link to start the OAuth flow
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>QuickBooks OAuth Sample</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; }
        .button { display: inline-block; background: #2E86C1; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }
    </style>
</head>
<body>
    <h1>QuickBooks OAuth Sample</h1>
    <p>Click the button below to authorize with QuickBooks:</p>
    <a href=""/auth"" class=""button"">Authorize with QuickBooks</a>
</body>
</html>
", "text/html"));

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

// OAuth callback - display the access token
app.MapGet("/callback", async (HttpContext context, IQuickBooksOAuthService oauthService) =>
{
  string? code = context.Request.Query["code"];
  string? state = context.Request.Query["state"];
  string? realmId = context.Request.Query["realmId"];
  
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
  
  // Display the access token and other information
  return Results.Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Authorization Successful</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        pre {{ background: #f4f4f4; padding: 10px; border-radius: 5px; overflow-x: auto; }}
    </style>
</head>
<body>
    <h1>Authorization Successful!</h1>
    <h2>Access Token:</h2>
    <pre>{result.Tokens?.AccessToken ?? "N/A"}</pre>
    
    <h2>Refresh Token:</h2>
    <pre>{result.Tokens?.RefreshToken ?? "N/A"}</pre>
    
    <h2>Realm ID:</h2>
    <pre>{result.RealmId ?? "N/A"}</pre>
    
    <h2>Access Token Expires In:</h2>
    <pre>{result.Tokens?.ExpiresIn} seconds ({TimeSpan.FromSeconds(result.Tokens?.ExpiresIn ?? 0).TotalHours:0.##} hours)</pre>
    
    <h2>Refresh Token Expires In:</h2>
    <pre>{result.Tokens?.RefreshTokenExpiresIn} seconds ({TimeSpan.FromSeconds(result.Tokens?.RefreshTokenExpiresIn ?? 0).TotalDays:0.##} days)</pre>
    
    <p><a href=""/"">Back to Home</a></p>
</body>
</html>
", "text/html");
});

app.Run();
