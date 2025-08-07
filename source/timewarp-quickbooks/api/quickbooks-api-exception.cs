namespace TimeWarp.QuickBooks.Api;

using System.Net;
using System.Text.Json.Serialization;

/// <summary>
/// Represents an exception that occurs when interacting with the QuickBooks API.
/// </summary>
public class QuickBooksApiException : Exception
{
  /// <summary>
  /// Gets the HTTP status code of the failed request.
  /// </summary>
  public HttpStatusCode StatusCode { get; }

  /// <summary>
  /// Gets the error code returned by QuickBooks.
  /// </summary>
  public string? ErrorCode { get; private set; }

  /// <summary>
  /// Gets the error type returned by QuickBooks.
  /// </summary>
  public string? ErrorType { get; private set; }

  /// <summary>
  /// Gets the detailed error information returned by QuickBooks.
  /// </summary>
  public string? ErrorDetail { get; private set; }

  /// <summary>
  /// Gets the request URL that caused the error.
  /// </summary>
  public string? RequestUrl { get; }

  /// <summary>
  /// Gets the raw error response body from QuickBooks.
  /// </summary>
  public string? ResponseBody { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="QuickBooksApiException"/> class.
  /// </summary>
  /// <param name="message">The error message.</param>
  /// <param name="statusCode">The HTTP status code.</param>
  /// <param name="requestUrl">The request URL.</param>
  /// <param name="responseBody">The raw response body.</param>
  /// <param name="innerException">The inner exception.</param>
  public QuickBooksApiException
  (
    string message,
    HttpStatusCode statusCode,
    string? requestUrl = null,
    string? responseBody = null,
    Exception? innerException = null
  ) : base(message, innerException)
  {
    StatusCode = statusCode;
    RequestUrl = requestUrl;
    ResponseBody = responseBody;

    if (!string.IsNullOrEmpty(responseBody))
    {
      ParseErrorResponse(responseBody);
    }
  }

  /// <summary>
  /// Creates a QuickBooksApiException from an HTTP response.
  /// </summary>
  /// <param name="response">The HTTP response message.</param>
  /// <param name="responseBody">The response body content.</param>
  /// <returns>A new QuickBooksApiException instance.</returns>
  public static QuickBooksApiException FromHttpResponse(HttpResponseMessage response, string? responseBody = null)
  {
    string message = $"QuickBooks API request failed with status {(int)response.StatusCode} {response.StatusCode}";
    
    return new QuickBooksApiException
    (
      message,
      response.StatusCode,
      response.RequestMessage?.RequestUri?.ToString(),
      responseBody
    );
  }

  private void ParseErrorResponse(string responseBody)
  {
    try
    {
      var options = new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      };

      var errorResponse = JsonSerializer.Deserialize<QuickBooksErrorResponse>(responseBody, options);
      
      if (errorResponse?.Fault?.Error?.Count > 0)
      {
        var firstError = errorResponse.Fault.Error[0];
        ErrorCode = firstError.Code;
        ErrorDetail = firstError.Detail;
        ErrorType = errorResponse.Fault.Type;
      }
    }
    catch
    {
      // If we can't parse the error response, we'll just use the raw response body
    }
  }

  /// <summary>
  /// Returns a string representation of the exception.
  /// </summary>
  /// <returns>A string containing the exception details.</returns>
  public override string ToString()
  {
    var details = new List<string> { base.ToString() };

    if (!string.IsNullOrEmpty(ErrorCode))
      details.Add($"Error Code: {ErrorCode}");
    
    if (!string.IsNullOrEmpty(ErrorType))
      details.Add($"Error Type: {ErrorType}");
    
    if (!string.IsNullOrEmpty(ErrorDetail))
      details.Add($"Error Detail: {ErrorDetail}");
    
    if (!string.IsNullOrEmpty(RequestUrl))
      details.Add($"Request URL: {RequestUrl}");

    return string.Join(Environment.NewLine, details);
  }
}

/// <summary>
/// Represents the error response structure from QuickBooks API.
/// </summary>
internal class QuickBooksErrorResponse
{
  [JsonPropertyName("Fault")]
  public QuickBooksFault? Fault { get; set; }
}

/// <summary>
/// Represents the fault structure in a QuickBooks error response.
/// </summary>
internal class QuickBooksFault
{
  [JsonPropertyName("Error")]
  public List<QuickBooksError> Error { get; set; } = [];

  [JsonPropertyName("type")]
  public string? Type { get; set; }
}

/// <summary>
/// Represents an individual error in a QuickBooks error response.
/// </summary>
internal class QuickBooksError
{
  [JsonPropertyName("Message")]
  public string? Message { get; set; }

  [JsonPropertyName("Detail")]
  public string? Detail { get; set; }

  [JsonPropertyName("code")]
  public string? Code { get; set; }
}