namespace TimeWarp.QuickBooks.Api.Models;

/// <summary>
/// Creation and modification timestamps for a QuickBooks entity.
/// </summary>
public class ModificationMetaData
{
  /// <summary>
  /// Gets or sets the time the entity was created.
  /// </summary>
  public DateTimeOffset? CreateTime { get; set; }

  /// <summary>
  /// Gets or sets the time the entity was last updated.
  /// </summary>
  public DateTimeOffset? LastUpdatedTime { get; set; }
}
