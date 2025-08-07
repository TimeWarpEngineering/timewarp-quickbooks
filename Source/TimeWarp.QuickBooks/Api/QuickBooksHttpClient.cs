namespace TimeWarp.QuickBooks.Api;

using System.Reflection;

/// <summary>
/// A typed HttpClient for QuickBooks API requests.
/// </summary>
public class QuickBooksHttpClient
{
  /// <summary>
  /// Gets the underlying HttpClient instance.
  /// </summary>
  public HttpClient HttpClient { get; }

  private readonly QuickBooksApiOptions Options;
  private readonly ILogger<QuickBooksHttpClient> Logger;

  /// <summary>
  /// Initializes a new instance of the <see cref="QuickBooksHttpClient"/> class.
  /// </summary>
  /// <param name="httpClient">The HttpClient instance.</param>
  /// <param name="options">The QuickBooks API options.</param>
  /// <param name="logger">The logger.</param>
  public QuickBooksHttpClient
  (
    HttpClient httpClient,
    IOptions<QuickBooksApiOptions> options,
    ILogger<QuickBooksHttpClient> logger
  )
  {
    HttpClient = httpClient;
    Options = options.Value;
    Logger = logger;

    ConfigureHttpClient();
  }

  private void ConfigureHttpClient()
  {
    // Set base URL based on environment
    HttpClient.BaseAddress = new Uri(Options.GetBaseUrl());
    
    // Set timeout
    HttpClient.Timeout = TimeSpan.FromSeconds(Options.TimeoutSeconds);

    // Set default headers
    HttpClient.DefaultRequestHeaders.Clear();
    HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    
    // Set User-Agent with assembly version
    string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
    HttpClient.DefaultRequestHeaders.Add("User-Agent", $"TimeWarp.QuickBooks/{version}");

    Logger.LogInformation
    (
      "Configured QuickBooks HttpClient for {Environment} environment with base URL: {BaseUrl}",
      Options.Environment,
      HttpClient.BaseAddress
    );
  }

  /// <summary>
  /// Adds the minor version query parameter to a URL if not already present.
  /// </summary>
  /// <param name="url">The URL to add the parameter to.</param>
  /// <returns>The URL with the minor version parameter.</returns>
  public string AddMinorVersionParameter(string url)
  {
    if (string.IsNullOrEmpty(Options.MinorVersion))
      return url;

    string separator = url.Contains('?') ? "&" : "?";
    
    // Check if minorversion is already in the URL
    if (url.Contains("minorversion", StringComparison.OrdinalIgnoreCase))
      return url;

    return $"{url}{separator}minorversion={Options.MinorVersion}";
  }

  /// <summary>
  /// Creates an HttpRequestMessage with the appropriate configuration.
  /// </summary>
  /// <param name="method">The HTTP method.</param>
  /// <param name="url">The request URL.</param>
  /// <param name="content">The request content (optional).</param>
  /// <returns>A configured HttpRequestMessage.</returns>
  public HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent? content = null)
  {
    string urlWithMinorVersion = AddMinorVersionParameter(url);
    
    var request = new HttpRequestMessage(method, urlWithMinorVersion);
    
    if (content != null)
    {
      request.Content = content;
    }

    return request;
  }
}