namespace TimeWarp.QuickBooks.Authentication;

using TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Interface for a service that handles OAuth authentication with QuickBooks.
/// </summary>
public interface IQuickBooksOAuthService
{
  /// <summary>
  /// Gets the QuickBooks OAuth configuration options.
  /// </summary>
  QuickBooksOAuthOptions Options { get; }

  /// <summary>
  /// Generates an authorization URL for initiating the OAuth flow.
  /// </summary>
  /// <param name="state">A unique state value to prevent CSRF attacks.</param>
  /// <returns>The authorization URL to redirect the user to.</returns>
  string GenerateAuthorizationUrl(string state);

  /// <summary>
  /// Handles the callback from QuickBooks after user authorization.
  /// </summary>
  /// <param name="code">The authorization code received from QuickBooks.</param>
  /// <param name="state">The state parameter that was sent in the authorization request.</param>
  /// <param name="realmId">The Realm ID (Company ID) received from QuickBooks.</param>
  /// <param name="expectedState">The expected state value to verify against the received state.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the callback result.</returns>
  Task<QuickBooksOAuthCallbackResult> HandleCallbackAsync
  (
    string code,
    string state,
    string realmId,
    string expectedState
  );

  /// <summary>
  /// Gets the current tokens for the specified realm.
  /// </summary>
  /// <param name="realmId">The Realm ID (Company ID) to get tokens for.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the tokens or null if not found.</returns>
  Task<QuickBooksTokens?> GetTokensAsync(string realmId);

  /// <summary>
  /// Refreshes the access token if it has expired.
  /// </summary>
  /// <param name="tokens">The current tokens.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the refreshed tokens.</returns>
  Task<QuickBooksTokens> RefreshTokensIfNeededAsync(QuickBooksTokens tokens);

  /// <summary>
  /// Revokes the access and refresh tokens.
  /// </summary>
  /// <param name="tokens">The tokens to revoke.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task RevokeTokensAsync(QuickBooksTokens tokens);

  /// <summary>
  /// Saves the tokens for the specified realm.
  /// </summary>
  /// <param name="realmId">The Realm ID (Company ID) to save tokens for.</param>
  /// <param name="tokens">The tokens to save.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  Task SaveTokensAsync(string realmId, QuickBooksTokens tokens);

  /// <summary>
  /// Validates the current state of the OAuth service.
  /// </summary>
  /// <returns>True if the service is properly configured; otherwise, false.</returns>
  bool ValidateConfiguration();
}