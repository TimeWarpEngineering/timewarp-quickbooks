namespace TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Configuration options for QuickBooks OAuth authentication.
/// </summary>
public class QuickBooksOAuthOptions
{
  /// <summary>
  /// Gets or sets the client ID (consumer key) for the QuickBooks application.
  /// </summary>
  public string ClientId { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the client secret (consumer secret) for the QuickBooks application.
  /// </summary>
  public string ClientSecret { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the redirect URI where QuickBooks will send the authorization response.
  /// </summary>
  public string RedirectUri { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the environment (production or sandbox).
  /// </summary>
  public QuickBooksEnvironment Environment { get; set; } = QuickBooksEnvironment.Sandbox;

  /// <summary>
  /// Gets or sets the scopes requested during authorization.
  /// </summary>
  public List<string> Scopes { get; set; } = new List<string>();
}

/// <summary>
/// Represents the QuickBooks environment.
/// </summary>
public enum QuickBooksEnvironment
{
  /// <summary>
  /// QuickBooks sandbox environment for development and testing.
  /// </summary>
  Sandbox,

  /// <summary>
  /// QuickBooks production environment for live applications.
  /// </summary>
  Production
}