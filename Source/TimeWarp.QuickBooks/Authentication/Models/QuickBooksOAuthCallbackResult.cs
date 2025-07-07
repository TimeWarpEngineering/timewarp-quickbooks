namespace TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Represents the result of a QuickBooks OAuth callback.
/// </summary>
public class QuickBooksOAuthCallbackResult
{
  /// <summary>
  /// Gets or sets a value indicating whether the callback was successful.
  /// </summary>
  public bool IsSuccess { get; set; }

  /// <summary>
  /// Gets or sets the error message if the callback was not successful.
  /// </summary>
  public string? ErrorMessage { get; set; }

  /// <summary>
  /// Gets or sets the authorization code received from QuickBooks.
  /// This code is exchanged for access and refresh tokens.
  /// </summary>
  public string? AuthorizationCode { get; set; }

  /// <summary>
  /// Gets or sets the state parameter that was sent in the authorization request.
  /// This is used to verify that the callback is in response to a request from this application.
  /// </summary>
  public string? State { get; set; }

  /// <summary>
  /// Gets or sets the Realm ID (Company ID) received from QuickBooks.
  /// </summary>
  public string? RealmId { get; set; }

  /// <summary>
  /// Gets or sets the tokens obtained after exchanging the authorization code.
  /// This will be null if the tokens have not been obtained yet.
  /// </summary>
  public QuickBooksTokens? Tokens { get; set; }

  /// <summary>
  /// Creates a successful callback result.
  /// </summary>
  /// <param name="authorizationCode">The authorization code received from QuickBooks.</param>
  /// <param name="state">The state parameter that was sent in the authorization request.</param>
  /// <param name="realmId">The Realm ID (Company ID) received from QuickBooks.</param>
  /// <returns>A successful callback result.</returns>
  public static QuickBooksOAuthCallbackResult Success
  (
    string authorizationCode,
    string state,
    string realmId
  )
  {
    return new QuickBooksOAuthCallbackResult
    {
      IsSuccess = true,
      AuthorizationCode = authorizationCode,
      State = state,
      RealmId = realmId
    };
  }

  /// <summary>
  /// Creates a failed callback result.
  /// </summary>
  /// <param name="errorMessage">The error message.</param>
  /// <returns>A failed callback result.</returns>
  public static QuickBooksOAuthCallbackResult Failure
  (
    string errorMessage
  )
  {
    return new QuickBooksOAuthCallbackResult
    {
      IsSuccess = false,
      ErrorMessage = errorMessage
    };
  }
}