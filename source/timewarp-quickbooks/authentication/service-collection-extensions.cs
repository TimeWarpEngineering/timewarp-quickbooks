namespace TimeWarp.QuickBooks.Authentication;

using TimeWarp.QuickBooks.Api;
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

    // Register the OAuth service as Singleton for in-memory token storage
    // Note: In production, use persistent storage instead of in-memory storage
    services.AddSingleton<IQuickBooksOAuthService, QuickBooksOAuthService>();

    return services;
  }

  /// <summary>
  /// Adds QuickBooks OAuth services to the service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configuration">The root configuration containing QuickBooks OAuth settings.</param>
  /// <returns>The service collection.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the QuickBooks configuration section is not found.</exception>
  public static IServiceCollection AddQuickBooksOAuth
  (
    this IServiceCollection services,
    IConfiguration configuration
  )
  {
    // Get the QuickBooks section from configuration
    IConfigurationSection section = configuration.GetSection(QuickBooksOAuthOptions.SectionName);
    
    // Validate that the section exists
    if (!section.Exists())
    {
      throw new InvalidOperationException
      (
        $"Configuration section '{QuickBooksOAuthOptions.SectionName}' not found. " +
        $"Please ensure your appsettings.json contains a '{QuickBooksOAuthOptions.SectionName}' section with the required OAuth settings."
      );
    }

    // Register options from the specific configuration section
    services.Configure<QuickBooksOAuthOptions>(section);

    // Register the OAuth service as Singleton for in-memory token storage
    // Note: In production, use persistent storage instead of in-memory storage
    services.AddSingleton<IQuickBooksOAuthService, QuickBooksOAuthService>();

    return services;
  }

  /// <summary>
  /// Adds QuickBooks API client services to the service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configuration">The root configuration containing QuickBooks API settings.</param>
  /// <returns>The service collection.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the QuickBooksApi configuration section is not found.</exception>
  public static IServiceCollection AddQuickBooksApiClient
  (
    this IServiceCollection services,
    IConfiguration configuration
  )
  {
    // Get the QuickBooksApi section from configuration
    IConfigurationSection section = configuration.GetSection(QuickBooksApiOptions.SectionName);
    
    // Validate that the section exists
    if (!section.Exists())
    {
      throw new InvalidOperationException
      (
        $"Configuration section '{QuickBooksApiOptions.SectionName}' not found. " +
        "Please ensure your appsettings.json contains a 'QuickBooksApi' section with the required API settings."
      );
    }

    // Register options from the specific configuration section
    services.Configure<QuickBooksApiOptions>(section);

    // Register the typed HTTP client
    services.AddHttpClient<QuickBooksHttpClient>();

    // Register the API client
    services.AddScoped<IQuickBooksApiClient, QuickBooksApiClient>();

    return services;
  }

  /// <summary>
  /// Adds QuickBooks API client services to the service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configureOptions">The action to configure the QuickBooks API options.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddQuickBooksApiClient
  (
    this IServiceCollection services,
    Action<QuickBooksApiOptions> configureOptions
  )
  {
    // Register options
    services.Configure(configureOptions);

    // Register the typed HTTP client
    services.AddHttpClient<QuickBooksHttpClient>();

    // Register the API client
    services.AddScoped<IQuickBooksApiClient, QuickBooksApiClient>();

    return services;
  }
}