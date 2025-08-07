namespace TimeWarp.QuickBooks.Api;

using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Web;
using TimeWarp.QuickBooks.Authentication;
using TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Implementation of the QuickBooks API client.
/// </summary>
public class QuickBooksApiClient : IQuickBooksApiClient
{
  private readonly QuickBooksHttpClient QuickBooksHttpClient;
  private readonly IQuickBooksOAuthService OAuthService;
  private readonly ILogger<QuickBooksApiClient> Logger;
  private readonly QuickBooksApiOptions Options;
  private readonly JsonSerializerOptions JsonOptions;

  /// <summary>
  /// Initializes a new instance of the <see cref="QuickBooksApiClient"/> class.
  /// </summary>
  /// <param name="quickBooksHttpClient">The typed HTTP client.</param>
  /// <param name="oAuthService">The OAuth service for token management.</param>
  /// <param name="options">The API options.</param>
  /// <param name="logger">The logger.</param>
  public QuickBooksApiClient
  (
    QuickBooksHttpClient quickBooksHttpClient,
    IQuickBooksOAuthService oAuthService,
    IOptions<QuickBooksApiOptions> options,
    ILogger<QuickBooksApiClient> logger
  )
  {
    QuickBooksHttpClient = quickBooksHttpClient;
    OAuthService = oAuthService;
    Options = options.Value;
    Logger = logger;

    JsonOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
  }

  /// <inheritdoc/>
  public async Task<T?> GetAsync<T>(string realmId, string endpoint, CancellationToken cancellationToken = default)
  {
    string url = BuildUrl(realmId, endpoint);
    
    if (Options.EnableDetailedLogging)
      Logger.LogInformation("GET request to: {Url}", url);

    var request = QuickBooksHttpClient.CreateRequest(HttpMethod.Get, url);
    
    return await ExecuteRequestAsync<T>(realmId, request, cancellationToken);
  }

  /// <inheritdoc/>
  public async Task<T?> PostAsync<T>(string realmId, string endpoint, object payload, CancellationToken cancellationToken = default)
  {
    string url = BuildUrl(realmId, endpoint);
    
    if (Options.EnableDetailedLogging)
      Logger.LogInformation("POST request to: {Url}", url);

    string json = JsonSerializer.Serialize(payload, JsonOptions);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    
    var request = QuickBooksHttpClient.CreateRequest(HttpMethod.Post, url, content);
    
    return await ExecuteRequestAsync<T>(realmId, request, cancellationToken);
  }

  /// <inheritdoc/>
  public async Task<T?> PutAsync<T>(string realmId, string endpoint, object payload, CancellationToken cancellationToken = default)
  {
    string url = BuildUrl(realmId, endpoint);
    
    if (Options.EnableDetailedLogging)
      Logger.LogInformation("PUT request to: {Url}", url);

    string json = JsonSerializer.Serialize(payload, JsonOptions);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    
    var request = QuickBooksHttpClient.CreateRequest(HttpMethod.Put, url, content);
    
    return await ExecuteRequestAsync<T>(realmId, request, cancellationToken);
  }

  /// <inheritdoc/>
  public async Task DeleteAsync(string realmId, string endpoint, CancellationToken cancellationToken = default)
  {
    string url = BuildUrl(realmId, endpoint);
    
    if (Options.EnableDetailedLogging)
      Logger.LogInformation("DELETE request to: {Url}", url);

    var request = QuickBooksHttpClient.CreateRequest(HttpMethod.Delete, url);
    
    await ExecuteRequestAsync<object>(realmId, request, cancellationToken);
  }

  /// <inheritdoc/>
  public async Task<T?> QueryAsync<T>(string realmId, string query, CancellationToken cancellationToken = default)
  {
    string encodedQuery = HttpUtility.UrlEncode(query);
    string url = BuildUrl(realmId, $"query?query={encodedQuery}");
    
    if (Options.EnableDetailedLogging)
      Logger.LogInformation("Query request: {Query}", query);

    var request = QuickBooksHttpClient.CreateRequest(HttpMethod.Get, url);
    
    return await ExecuteRequestAsync<T>(realmId, request, cancellationToken);
  }

  private string BuildUrl(string realmId, string endpoint)
  {
    // Remove leading slash if present
    endpoint = endpoint.TrimStart('/');
    
    // Build the full URL
    return $"/v3/company/{realmId}/{endpoint}";
  }

  private async Task<T?> ExecuteRequestAsync<T>
  (
    string realmId,
    HttpRequestMessage request,
    CancellationToken cancellationToken,
    bool isRetry = false
  )
  {
    // Get and refresh tokens if needed
    QuickBooksTokens? tokens = await GetAndRefreshTokensAsync(realmId);
    
    if (tokens == null)
    {
      throw new InvalidOperationException($"No tokens found for realm ID: {realmId}");
    }

    // Set authorization header
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

    try
    {
      HttpResponseMessage response = await QuickBooksHttpClient.HttpClient.SendAsync(request, cancellationToken);
      
      string? responseBody = null;
      if (response.Content != null)
      {
        responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
      }

      if (Options.EnableDetailedLogging && !string.IsNullOrEmpty(responseBody))
      {
        Logger.LogInformation("Response body: {ResponseBody}", responseBody);
      }

      // Handle 401 Unauthorized - attempt token refresh and retry once
      if (response.StatusCode == HttpStatusCode.Unauthorized && !isRetry)
      {
        Logger.LogInformation("Received 401 Unauthorized. Attempting token refresh and retry.");
        
        // Force token refresh
        tokens = await OAuthService.RefreshTokensIfNeededAsync(tokens);
        
        // Clone the request for retry
        var retryRequest = CloneHttpRequestMessage(request);
        retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        
        return await ExecuteRequestAsync<T>(realmId, retryRequest, cancellationToken, true);
      }

      // Check for success
      if (!response.IsSuccessStatusCode)
      {
        throw QuickBooksApiException.FromHttpResponse(response, responseBody);
      }

      // Return null for empty responses
      if (string.IsNullOrEmpty(responseBody))
      {
        return default;
      }

      // Handle DELETE operations that return void
      if (typeof(T) == typeof(object) && request.Method == HttpMethod.Delete)
      {
        return default;
      }

      // Deserialize the response
      return JsonSerializer.Deserialize<T>(responseBody, JsonOptions);
    }
    catch (HttpRequestException ex)
    {
      Logger.LogError(ex, "HTTP request failed for realm {RealmId}", realmId);
      throw new QuickBooksApiException
      (
        $"HTTP request failed: {ex.Message}",
        HttpStatusCode.ServiceUnavailable,
        request.RequestUri?.ToString(),
        null,
        ex
      );
    }
    catch (TaskCanceledException ex)
    {
      Logger.LogError(ex, "Request timeout for realm {RealmId}", realmId);
      throw new QuickBooksApiException
      (
        "Request timed out",
        HttpStatusCode.RequestTimeout,
        request.RequestUri?.ToString(),
        null,
        ex
      );
    }
    catch (QuickBooksApiException)
    {
      // Re-throw QuickBooksApiException as-is
      throw;
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Unexpected error during API request for realm {RealmId}", realmId);
      throw new QuickBooksApiException
      (
        $"Unexpected error: {ex.Message}",
        HttpStatusCode.InternalServerError,
        request.RequestUri?.ToString(),
        null,
        ex
      );
    }
  }

  private async Task<QuickBooksTokens?> GetAndRefreshTokensAsync(string realmId)
  {
    QuickBooksTokens? tokens = await OAuthService.GetTokensAsync(realmId);
    
    if (tokens == null)
    {
      Logger.LogWarning("No tokens found for realm {RealmId}", realmId);
      return null;
    }

    // Refresh if needed
    if (tokens.IsExpired())
    {
      Logger.LogInformation("Tokens expired for realm {RealmId}, refreshing", realmId);
      tokens = await OAuthService.RefreshTokensIfNeededAsync(tokens);
    }

    return tokens;
  }

  private static HttpRequestMessage CloneHttpRequestMessage(HttpRequestMessage request)
  {
    var clone = new HttpRequestMessage(request.Method, request.RequestUri)
    {
      Content = request.Content,
      Version = request.Version
    };

    foreach (var header in request.Headers)
    {
      clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
    }

    if (request.Content != null)
    {
      foreach (var header in request.Content.Headers)
      {
        clone.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
      }
    }

    return clone;
  }
}