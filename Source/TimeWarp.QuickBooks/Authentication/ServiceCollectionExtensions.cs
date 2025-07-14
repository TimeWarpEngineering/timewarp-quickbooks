namespace TimeWarp.QuickBooks.Authentication;

using TimeWarp.QuickBooks.Authentication.Models;

/// <summary>
/// Extension methods for registering QuickBooks authentication services.
/// </summary>
public static class ServiceCollectionExtensions
{
  /// <summary>
  /// Adds QuickBooks OAuth services to the service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configureOptions">The action to configure the QuickBooks OAuth options.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddQuickBooksOAuth
  (
    this IServiceCollection services,
    Action<QuickBooksOAuthOptions> configureOptions
  )
  {
    // Register options
    services.Configure(configureOptions);

    // Register the OAuth service
    services.AddScoped<IQuickBooksOAuthService, QuickBooksOAuthService>();

    return services;
  }

  /// <summary>
  /// Adds QuickBooks OAuth services to the service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configuration">The configuration section containing QuickBooks OAuth settings.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddQuickBooksOAuth
  (
    this IServiceCollection services,
    IConfiguration configuration
  )
  {
    // Register options from configuration
    services.Configure<QuickBooksOAuthOptions>(configuration);

    // Register the OAuth service
    services.AddScoped<IQuickBooksOAuthService, QuickBooksOAuthService>();

    return services;
  }
}