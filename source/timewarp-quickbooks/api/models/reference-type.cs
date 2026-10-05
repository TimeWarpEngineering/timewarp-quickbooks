namespace TimeWarp.QuickBooks.Api.Models;

/// <summary>
/// A reference to another QuickBooks entity, serialized as <c>{ "value": "...", "name": "..." }</c>.
/// </summary>
public class ReferenceType
{
  /// <summary>
  /// Gets or sets the identifier of the referenced entity.
  /// </summary>
  public string Value { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the display name of the referenced entity.
  /// </summary>
  public string? Name { get; set; }
}
