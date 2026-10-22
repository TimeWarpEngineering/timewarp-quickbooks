namespace TimeWarp.QuickBooks.Api.Models;

/// <summary>
/// A QuickBooks Online account from the chart of accounts.
/// </summary>
public class Account
{
  /// <summary>
  /// Gets or sets the unique identifier of the account.
  /// </summary>
  public string Id { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the version token used for optimistic concurrency on updates.
  /// </summary>
  public string SyncToken { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the account name.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the fully qualified name, including parent account names.
  /// </summary>
  public string FullyQualifiedName { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the account type, such as Bank or Expense.
  /// </summary>
  public string AccountType { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the account sub-type.
  /// </summary>
  public string? AccountSubType { get; set; }

  /// <summary>
  /// Gets or sets the classification, such as Asset, Liability, or Revenue.
  /// </summary>
  public string Classification { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the user-assigned account number.
  /// </summary>
  public string? AccountNumber { get; set; }

  /// <summary>
  /// Gets or sets the account description.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the account is active.
  /// </summary>
  public bool Active { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the account is a sub-account.
  /// </summary>
  public bool SubAccount { get; set; }

  /// <summary>
  /// Gets or sets the reference to the parent account.
  /// </summary>
  public ReferenceType? ParentRef { get; set; }

  /// <summary>
  /// Gets or sets the current balance of the account.
  /// </summary>
  public decimal CurrentBalance { get; set; }

  /// <summary>
  /// Gets or sets the current balance including sub-accounts.
  /// </summary>
  public decimal CurrentBalanceWithSubAccounts { get; set; }

  /// <summary>
  /// Gets or sets the reference to the account currency.
  /// </summary>
  public ReferenceType? CurrencyRef { get; set; }

  /// <summary>
  /// Gets or sets the creation and modification metadata.
  /// </summary>
  public ModificationMetaData? MetaData { get; set; }
}
