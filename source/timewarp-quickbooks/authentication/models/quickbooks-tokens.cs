namespace TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Represents the OAuth tokens received from QuickBooks API.
/// </summary>
public class QuickBooksTokens
{
  /// <summary>
  /// Gets or sets the access token used for API requests.
  /// </summary>
  public string AccessToken { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the refresh token used to obtain a new access token when it expires.
  /// </summary>
  public string RefreshToken { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of the access token (e.g., "Bearer").
  /// </summary>
  public string TokenType { get; set; } = "Bearer";

  /// <summary>
  /// Gets or sets the expiration time of the access token in seconds from the time it was issued.
  /// </summary>
  public long ExpiresIn { get; set; }

  /// <summary>
  /// Gets or sets the expiration time of the refresh token in seconds from the time it was issued.
  /// This corresponds to the x_refresh_token_expires_in field from QuickBooks OAuth response.
  /// </summary>
  public long RefreshTokenExpiresIn { get; set; }

  /// <summary>
  /// Gets or sets the UTC date and time when the access token was issued.
  /// </summary>
  public DateTime IssuedUtc { get; set; } = DateTime.UtcNow;

  /// <summary>
  /// Gets or sets the Realm ID (Company ID) associated with these tokens.
  /// </summary>
  public string RealmId { get; set; } = string.Empty;

  /// <summary>
  /// Determines whether the access token has expired.
  /// </summary>
  /// <returns>True if the token has expired; otherwise, false.</returns>
  public bool IsExpired()
  {
    return DateTime.UtcNow >= IssuedUtc.AddSeconds(ExpiresIn - 300); // 5-minute buffer
  }

  /// <summary>
  /// Determines whether the refresh token has expired.
  /// </summary>
  /// <returns>True if the refresh token has expired; otherwise, false.</returns>
  public bool IsRefreshTokenExpired()
  {
    return DateTime.UtcNow >= IssuedUtc.AddSeconds(RefreshTokenExpiresIn - 300); // 5-minute buffer
  }
}