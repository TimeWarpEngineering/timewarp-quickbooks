namespace TimeWarp.QuickBooks.Authentication;

using TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Service that handles OAuth authentication with QuickBooks.
/// </summary>
public class QuickBooksOAuthService : IQuickBooksOAuthService
{
  private readonly ILogger<QuickBooksOAuthService> Logger;
  private readonly OAuth2Client OAuth2Client;
  private readonly Dictionary<string, QuickBooksTokens> TokenStore;

  /// <summary>
  /// Gets the QuickBooks OAuth configuration options.
  /// </summary>
  public QuickBooksOAuthOptions Options { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="QuickBooksOAuthService"/> class.
  /// </summary>
  /// <param name="options">The QuickBooks OAuth options.</param>
  /// <param name="logger">The logger.</param>
  public QuickBooksOAuthService
  (
    IOptions<QuickBooksOAuthOptions> options,
    ILogger<QuickBooksOAuthService> logger
  )
  {
    Options = options.Value;
    Logger = logger;
    TokenStore = [];

    // Create the OAuth2 client using the Intuit SDK
    OAuth2Client = new OAuth2Client
    (
      Options.ClientId,
      Options.ClientSecret,
      Options.RedirectUri,
      Options.Environment == QuickBooksEnvironment.Production ?
        nameof(QuickBooksEnvironment.Production) :
        nameof(QuickBooksEnvironment.Sandbox)
    );
  }

  /// <summary>
  /// Generates an authorization URL for initiating the OAuth flow.
  /// </summary>
  /// <param name="state">A unique state value to prevent CSRF attacks.</param>
  /// <returns>The authorization URL to redirect the user to.</returns>
  public string GenerateAuthorizationUrl(string state)
  {
    Logger.LogInformation("Generating authorization URL with state: {State}", state);

    List<OidcScopes> scopes = [];
    
    // Convert string scopes to OidcScopes enum values
    foreach (string scope in Options.Scopes)
    {
      if (Enum.TryParse(scope, true, out OidcScopes oidcScope))
        scopes.Add(oidcScope);
      else
        Logger.LogWarning("Invalid scope: {Scope}", scope);
    }

    // Generate the authorization URL
    string authUrl = OAuth2Client.GetAuthorizationURL(scopes, state);
    
    Logger.LogInformation("Generated authorization URL: {AuthUrl}", authUrl);
    
    return authUrl;
  }

  /// <summary>
  /// Handles the callback from QuickBooks after user authorization.
  /// </summary>
  /// <param name="code">The authorization code received from QuickBooks.</param>
  /// <param name="state">The state parameter that was sent in the authorization request.</param>
  /// <param name="realmId">The Realm ID (Company ID) received from QuickBooks.</param>
  /// <param name="expectedState">The expected state value to verify against the received state.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the callback result.</returns>
  public async Task<QuickBooksOAuthCallbackResult> HandleCallbackAsync
  (
    string code,
    string state,
    string realmId,
    string expectedState
  )
  {
    Logger.LogInformation("Handling OAuth callback for realm: {RealmId}", realmId);

    // Verify state to prevent CSRF attacks
    if (state != expectedState)
    {
      string errorMessage = "State mismatch. Possible CSRF attack.";
      Logger.LogWarning(errorMessage);
      return QuickBooksOAuthCallbackResult.Failure(errorMessage);
    }

    try
    {
      // Exchange authorization code for tokens
      TokenResponse tokenResponse = await OAuth2Client.GetBearerTokenAsync(code);

      // Create and store tokens
      QuickBooksTokens tokens = new()
      {
        AccessToken = tokenResponse.AccessToken,
        RefreshToken = tokenResponse.RefreshToken,
        TokenType = tokenResponse.TokenType,
        ExpiresIn = tokenResponse.AccessTokenExpiresIn,
        IssuedUtc = DateTime.UtcNow,
        RealmId = realmId
      };

      await SaveTokensAsync(realmId, tokens);

      // Create successful result with tokens
      QuickBooksOAuthCallbackResult result = QuickBooksOAuthCallbackResult.Success(code, state, realmId);
      result.Tokens = tokens;

      Logger.LogInformation("Successfully processed OAuth callback for realm: {RealmId}", realmId);
      
      return result;
    }
    catch (Exception ex)
    {
      string errorMessage = $"Error handling OAuth callback: {ex.Message}";
      Logger.LogError(ex, errorMessage);
      return QuickBooksOAuthCallbackResult.Failure(errorMessage);
    }
  }

  /// <summary>
  /// Gets the current tokens for the specified realm.
  /// </summary>
  /// <param name="realmId">The Realm ID (Company ID) to get tokens for.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the tokens or null if not found.</returns>
  public Task<QuickBooksTokens?> GetTokensAsync(string realmId)
  {
    Logger.LogInformation("Getting tokens for realm: {RealmId}", realmId);

    if (TokenStore.TryGetValue(realmId, out QuickBooksTokens? tokens))
    {
      return Task.FromResult<QuickBooksTokens?>(tokens);
    }

    Logger.LogWarning("No tokens found for realm: {RealmId}", realmId);
    return Task.FromResult<QuickBooksTokens?>(null);
  }

  /// <summary>
  /// Refreshes the access token if it has expired.
  /// </summary>
  /// <param name="tokens">The current tokens.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the refreshed tokens.</returns>
  public async Task<QuickBooksTokens> RefreshTokensIfNeededAsync(QuickBooksTokens tokens)
  {
    if (!tokens.IsExpired())
    {
      Logger.LogInformation("Tokens for realm {RealmId} are still valid", tokens.RealmId);
      return tokens;
    }

    Logger.LogInformation("Refreshing tokens for realm: {RealmId}", tokens.RealmId);

    try
    {
      // Use the refresh token to get a new access token
      TokenResponse tokenResponse = await OAuth2Client.RefreshTokenAsync(tokens.RefreshToken);

      // Update tokens
      QuickBooksTokens refreshedTokens = new()
      {
        AccessToken = tokenResponse.AccessToken,
        RefreshToken = tokenResponse.RefreshToken,
        TokenType = tokenResponse.TokenType,
        ExpiresIn = tokenResponse.AccessTokenExpiresIn,
        IssuedUtc = DateTime.UtcNow,
        RealmId = tokens.RealmId
      };

      // Save the refreshed tokens
      await SaveTokensAsync(tokens.RealmId, refreshedTokens);

      Logger.LogInformation("Successfully refreshed tokens for realm: {RealmId}", tokens.RealmId);
      
      return refreshedTokens;
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error refreshing tokens for realm: {RealmId}", tokens.RealmId);
      throw;
    }
  }

  /// <summary>
  /// Revokes the access and refresh tokens.
  /// </summary>
  /// <param name="tokens">The tokens to revoke.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  public async Task RevokeTokensAsync(QuickBooksTokens tokens)
  {
    Logger.LogInformation("Revoking tokens for realm: {RealmId}", tokens.RealmId);

    try
    {
      // Revoke the access token
      await OAuth2Client.RevokeTokenAsync(tokens.AccessToken, "access_token");
      
      // Revoke the refresh token
      await OAuth2Client.RevokeTokenAsync(tokens.RefreshToken, "refresh_token");

      // Remove from token store
      if (TokenStore.ContainsKey(tokens.RealmId))
      {
        TokenStore.Remove(tokens.RealmId);
      }

      Logger.LogInformation("Successfully revoked tokens for realm: {RealmId}", tokens.RealmId);
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Error revoking tokens for realm: {RealmId}", tokens.RealmId);
      throw;
    }
  }

  /// <summary>
  /// Saves the tokens for the specified realm.
  /// </summary>
  /// <param name="realmId">The Realm ID (Company ID) to save tokens for.</param>
  /// <param name="tokens">The tokens to save.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  public Task SaveTokensAsync(string realmId, QuickBooksTokens tokens)
  {
    Logger.LogInformation("Saving tokens for realm: {RealmId}", realmId);

    // In a real implementation, tokens should be stored securely in a database or secure storage
    // For this implementation, we're using an in-memory dictionary
    TokenStore[realmId] = tokens;

    return Task.CompletedTask;
  }

  /// <summary>
  /// Validates the current state of the OAuth service.
  /// </summary>
  /// <returns>True if the service is properly configured; otherwise, false.</returns>
  public bool ValidateConfiguration()
  {
    bool isValid = true;

    if (string.IsNullOrEmpty(Options.ClientId))
    {
      Logger.LogError("ClientId is not configured");
      isValid = false;
    }

    if (string.IsNullOrEmpty(Options.ClientSecret))
    {
      Logger.LogError("ClientSecret is not configured");
      isValid = false;
    }

    if (string.IsNullOrEmpty(Options.RedirectUri))
    {
      Logger.LogError("RedirectUri is not configured");
      isValid = false;
    }

    if (Options.Scopes.Count == 0)
    {
      Logger.LogError("No scopes are configured");
      isValid = false;
    }

    return isValid;
  }
}