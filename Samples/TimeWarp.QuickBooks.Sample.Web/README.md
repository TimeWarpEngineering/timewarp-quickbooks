# TimeWarp.QuickBooks.Sample.Web

This sample application demonstrates how to use the TimeWarp.QuickBooks library to implement OAuth authentication with QuickBooks Online.

## Overview

This minimal ASP.NET Core web application shows the essential steps to implement QuickBooks OAuth authentication:

1. Configure the QuickBooks OAuth service
2. Initiate the OAuth authorization flow
3. Handle the OAuth callback
4. Retrieve and display access tokens

## User Experience

The sample provides a minimal but functional user experience:

1. **Home Page**: A simple page with a button/link to start the OAuth flow
   - Text: "Authorize with QuickBooks"
   - When clicked, redirects to the `/auth` endpoint

2. **Authorization Page**: Redirects to QuickBooks authorization page
   - User logs in to their QuickBooks account
   - User grants permissions to the application
   - QuickBooks redirects back to the application's callback URL

3. **Callback Page**: Displays the OAuth results
   - Shows the access token (proof that the OAuth flow worked)
   - Shows the refresh token
   - Shows the realm ID (company ID)
   - Provides a link back to the home page

This minimal UX demonstrates the complete OAuth flow while keeping the implementation as simple as possible.

## Implementation

The minimal implementation requires:

1. **Configuration**:
   ```json
   {
     "QuickBooks": {
       "ClientId": "your-client-id",
       "ClientSecret": "your-client-secret",
       "RedirectUri": "https://localhost:5001/callback",
       "Environment": "Sandbox",
       "Scopes": ["com.intuit.quickbooks.accounting"]
     }
   }
   ```

2. **Service Registration**:
   ```csharp
   builder.Services.AddQuickBooksOAuth(options =>
   {
     options.ClientId = "your-client-id";
     options.ClientSecret = "your-client-secret";
     options.RedirectUri = "https://localhost:5001/callback";
     options.Environment = QuickBooksEnvironment.Sandbox;
     options.Scopes = new List<string> { "com.intuit.quickbooks.accounting" };
   });
   ```

3. **Home Page Endpoint**:
   ```csharp
   app.MapGet("/", () => Results.Content(@"
     <h1>QuickBooks OAuth Sample</h1>
     <a href=""/auth"">Authorize with QuickBooks</a>
   ", "text/html"));
   ```

4. **Auth Endpoint**:
   ```csharp
   app.MapGet("/auth", (IQuickBooksOAuthService oauthService) =>
   {
     string authUrl = oauthService.GetAuthorizationUrl("state123");
     return Results.Redirect(authUrl);
   });
   ```

5. **Callback Endpoint**:
   ```csharp
   app.MapGet("/callback", async (HttpContext context, IQuickBooksOAuthService oauthService) =>
   {
     string? code = context.Request.Query["code"];
     string? realmId = context.Request.Query["realmId"];
     
     QuickBooksOAuthCallbackResult result = await oauthService.HandleCallbackAsync(code, realmId);
     
     return Results.Content($@"
       <h1>Authorization Successful!</h1>
       <h2>Access Token:</h2>
       <pre>{result.Tokens.AccessToken}</pre>
       <h2>Refresh Token:</h2>
       <pre>{result.Tokens.RefreshToken}</pre>
       <h2>Realm ID:</h2>
       <pre>{result.RealmId}</pre>
     ", "text/html");
   });
   ```

## Running the Sample

1. Update the configuration with your QuickBooks app credentials
2. Run the application: `dotnet run`
3. Navigate to `https://localhost:5001/` in your browser
4. Click the "Authorize with QuickBooks" link
5. Complete the QuickBooks authorization process
6. View the access token and other OAuth information on the callback page

## Next Steps

After obtaining the access token, you can use it to make API calls to QuickBooks Online. The TimeWarp.QuickBooks library provides additional functionality for working with the QuickBooks API.