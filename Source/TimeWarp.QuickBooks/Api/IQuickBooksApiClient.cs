namespace TimeWarp.QuickBooks.Api;

/// <summary>
/// Defines the contract for interacting with the QuickBooks API.
/// </summary>
public interface IQuickBooksApiClient
{
  /// <summary>
  /// Sends a GET request to the specified QuickBooks API endpoint.
  /// </summary>
  /// <typeparam name="T">The type to deserialize the response to.</typeparam>
  /// <param name="realmId">The Realm ID (Company ID) for the request.</param>
  /// <param name="endpoint">The API endpoint path (relative to /v3/company/{realmId}).</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deserialized response or null if the response is empty.</returns>
  Task<T?> GetAsync<T>(string realmId, string endpoint, CancellationToken cancellationToken = default);

  /// <summary>
  /// Sends a POST request to the specified QuickBooks API endpoint.
  /// </summary>
  /// <typeparam name="T">The type to deserialize the response to.</typeparam>
  /// <param name="realmId">The Realm ID (Company ID) for the request.</param>
  /// <param name="endpoint">The API endpoint path (relative to /v3/company/{realmId}).</param>
  /// <param name="payload">The object to serialize and send in the request body.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deserialized response or null if the response is empty.</returns>
  Task<T?> PostAsync<T>(string realmId, string endpoint, object payload, CancellationToken cancellationToken = default);

  /// <summary>
  /// Sends a PUT request to the specified QuickBooks API endpoint.
  /// </summary>
  /// <typeparam name="T">The type to deserialize the response to.</typeparam>
  /// <param name="realmId">The Realm ID (Company ID) for the request.</param>
  /// <param name="endpoint">The API endpoint path (relative to /v3/company/{realmId}).</param>
  /// <param name="payload">The object to serialize and send in the request body.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deserialized response or null if the response is empty.</returns>
  Task<T?> PutAsync<T>(string realmId, string endpoint, object payload, CancellationToken cancellationToken = default);

  /// <summary>
  /// Sends a DELETE request to the specified QuickBooks API endpoint.
  /// </summary>
  /// <param name="realmId">The Realm ID (Company ID) for the request.</param>
  /// <param name="endpoint">The API endpoint path (relative to /v3/company/{realmId}).</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  Task DeleteAsync(string realmId, string endpoint, CancellationToken cancellationToken = default);

  /// <summary>
  /// Executes a query against the QuickBooks API.
  /// </summary>
  /// <typeparam name="T">The type to deserialize the response to.</typeparam>
  /// <param name="realmId">The Realm ID (Company ID) for the request.</param>
  /// <param name="query">The query string (e.g., "SELECT * FROM Customer").</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deserialized response or null if the response is empty.</returns>
  Task<T?> QueryAsync<T>(string realmId, string query, CancellationToken cancellationToken = default);
}