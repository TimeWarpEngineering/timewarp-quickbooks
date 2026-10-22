namespace TimeWarp.QuickBooks.Api.Models;

/// <summary>
/// Generic envelope for QuickBooks query results: <c>{ "QueryResponse": { ... }, "time": "..." }</c>.
/// </summary>
/// <typeparam name="T">The type of the inner query response payload.</typeparam>
public class QueryResponseEnvelope<T>
{
  /// <summary>
  /// Gets or sets the query response payload.
  /// </summary>
  public T? QueryResponse { get; set; }

  /// <summary>
  /// Gets or sets the server time of the response.
  /// </summary>
  public DateTimeOffset? Time { get; set; }
}

/// <summary>
/// Paging information shared by all QuickBooks query responses.
/// </summary>
public class QueryResponsePage
{
  /// <summary>
  /// Gets or sets the 1-based start position of this page.
  /// </summary>
  public int StartPosition { get; set; }

  /// <summary>
  /// Gets or sets the number of entities in this page.
  /// </summary>
  public int MaxResults { get; set; }
}

/// <summary>
/// Query response payload for Account queries.
/// </summary>
public class AccountQueryResponse : QueryResponsePage
{
  /// <summary>
  /// Gets or sets the accounts in this page. Absent when the query matches nothing.
  /// </summary>
  public List<Account>? Account { get; set; }
}

/// <summary>
/// Envelope for Account query results.
/// </summary>
public class AccountQueryResponseEnvelope : QueryResponseEnvelope<AccountQueryResponse>
{
}
