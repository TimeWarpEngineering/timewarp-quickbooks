namespace TimeWarp.QuickBooks.Api;

using TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Configuration options for the QuickBooks API client.
/// </summary>
public class QuickBooksApiOptions
{
  /// <summary>
  /// The configuration section name for QuickBooks API settings.
  /// </summary>
  public const string SectionName = "QuickBooksApi";

  /// <summary>
  /// Gets or sets the base URL for the QuickBooks production API.
  /// </summary>
  public string ProductionBaseUrl { get; set; } = "https://quickbooks.api.intuit.com";

  /// <summary>
  /// Gets or sets the base URL for the QuickBooks sandbox API.
  /// </summary>
  public string SandboxBaseUrl { get; set; } = "https://sandbox-quickbooks.api.intuit.com";

  /// <summary>
  /// Gets or sets the environment to use (Sandbox or Production).
  /// </summary>
  public QuickBooksEnvironment Environment { get; set; } = QuickBooksEnvironment.Sandbox;

  /// <summary>
  /// Gets or sets the timeout in seconds for API requests.
  /// </summary>
  public int TimeoutSeconds { get; set; } = 30;

  /// <summary>
  /// Gets or sets whether to enable detailed logging of API requests and responses.
  /// </summary>
  public bool EnableDetailedLogging { get; set; } = false;

  /// <summary>
  /// Gets or sets the minor version of the QuickBooks API to use.
  /// </summary>
  /// <remarks>
  /// QuickBooks uses minor versions to introduce new features while maintaining backward compatibility.
  /// See: https://developer.intuit.com/app/developer/qbo/docs/learn/explore-the-quickbooks-online-api/minor-versions
  /// </remarks>
  public string MinorVersion { get; set; } = "65";

  /// <summary>
  /// Gets the base URL for the current environment.
  /// </summary>
  /// <returns>The base URL for the configured environment.</returns>
  public string GetBaseUrl()
  {
    return Environment == QuickBooksEnvironment.Production ? ProductionBaseUrl : SandboxBaseUrl;
  }
}