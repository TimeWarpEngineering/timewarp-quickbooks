namespace TimeWarp.QuickBooks.Tests;
using Shouldly;
using TimeWarp.Fixie;
using TimeWarp.QuickBooks.Api;
using TimeWarp.QuickBooks.Authentication;
using TimeWarp.QuickBooks.Authentication.Models;

[TestTag(TestTags.Fast)]
public class QuickBooksApiClient_Should_
{
  private static IServiceProvider CreateServiceProvider(TestHttpMessageHandler testHandler)
  {
    var services = new ServiceCollection();
    
    // Configure logging
    services.AddLogging();
    
    // Configure OAuth service with test options
    services.AddQuickBooksOAuth(options =>
    {
      options.ClientId = "test-client-id";
      options.ClientSecret = "test-client-secret";
      options.RedirectUri = "https://localhost/callback";
      options.Environment = QuickBooksEnvironment.Sandbox;
      options.Scopes = ["com.intuit.quickbooks.accounting"];
    });
    
    // Configure API client with test options
    services.Configure<QuickBooksApiOptions>(options =>
    {
      options.Environment = QuickBooksEnvironment.Sandbox;
      options.TimeoutSeconds = 30;
      options.EnableDetailedLogging = true;
      options.MinorVersion = "65";
    });
    
    // Register HttpClient with test handler
    services.AddHttpClient<QuickBooksHttpClient>()
      .ConfigurePrimaryHttpMessageHandler(() => testHandler);
    
    // Register API client
    services.AddScoped<IQuickBooksApiClient, QuickBooksApiClient>();
    
    return services.BuildServiceProvider();
  }
  
  private static async Task SetupTestTokens(IServiceProvider serviceProvider, string realmId = "test-realm-id")
  {
    var oAuthService = serviceProvider.GetRequiredService<IQuickBooksOAuthService>();
    var tokens = new QuickBooksTokens
    {
      AccessToken = "test-access-token",
      RefreshToken = "test-refresh-token",
      TokenType = "Bearer",
      ExpiresIn = 3600,
      RefreshTokenExpiresIn = 8726400,
      IssuedUtc = DateTime.UtcNow,
      RealmId = realmId
    };
    
    await oAuthService.SaveTokensAsync(realmId, tokens);
  }
  
  public static async Task Return_Deserialized_Response_On_Get()
  {
    // Arrange
    var testHandler = new TestHttpMessageHandler();
    var serviceProvider = CreateServiceProvider(testHandler);
    await SetupTestTokens(serviceProvider);
    
    var apiClient = serviceProvider.GetRequiredService<IQuickBooksApiClient>();
    var expectedResponse = new { CompanyName = "Test Company", Id = "1" };
    
    testHandler.SetupResponse(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.OK,
      Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
    });
    
    // Act
    var result = await apiClient.GetAsync<JsonElement?>("test-realm-id", "companyinfo/1");
    
    // Assert
    result.ShouldNotBeNull();
    testHandler.LastRequest.ShouldNotBeNull();
    testHandler.LastRequest!.Headers.Authorization?.ToString().ShouldBe("Bearer test-access-token");
    testHandler.LastRequest!.RequestUri?.PathAndQuery.ShouldContain("/v3/company/test-realm-id/companyinfo/1");
    testHandler.LastRequest!.RequestUri?.Query.ShouldContain("minorversion=65");
  }
  
  public static async Task Send_Json_Payload_On_Post()
  {
    // Arrange
    var testHandler = new TestHttpMessageHandler();
    var serviceProvider = CreateServiceProvider(testHandler);
    await SetupTestTokens(serviceProvider);
    
    var apiClient = serviceProvider.GetRequiredService<IQuickBooksApiClient>();
    var payload = new { DisplayName = "Test Customer", PrimaryEmailAddr = new { Address = "test@example.com" } };
    var expectedResponse = new { Id = "123", DisplayName = "Test Customer" };
    
    testHandler.SetupResponse(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.OK,
      Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
    });
    
    // Act
    var result = await apiClient.PostAsync<JsonElement?>("test-realm-id", "customer", payload);
    
    // Assert
    result.ShouldNotBeNull();
    testHandler.LastRequest.ShouldNotBeNull();
    testHandler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
    testHandler.LastRequest!.Content.ShouldNotBeNull();
    
    string requestContent = await testHandler.LastRequest!.Content!.ReadAsStringAsync();
    requestContent.ShouldContain("Test Customer");
    requestContent.ShouldContain("test@example.com");
  }
  
  public static async Task Encode_Query_Parameter_On_Query()
  {
    // Arrange
    var testHandler = new TestHttpMessageHandler();
    var serviceProvider = CreateServiceProvider(testHandler);
    await SetupTestTokens(serviceProvider);
    
    var apiClient = serviceProvider.GetRequiredService<IQuickBooksApiClient>();
    string query = "SELECT * FROM Customer WHERE Active = true";
    var expectedResponse = new { QueryResponse = new { Customer = new[] { new { Id = "1" } } } };
    
    testHandler.SetupResponse(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.OK,
      Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
    });
    
    // Act
    var result = await apiClient.QueryAsync<JsonElement?>("test-realm-id", query);
    
    // Assert
    result.ShouldNotBeNull();
    testHandler.LastRequest.ShouldNotBeNull();
    testHandler.LastRequest!.RequestUri?.PathAndQuery.ShouldContain("/v3/company/test-realm-id/query");
    testHandler.LastRequest!.RequestUri?.Query.ShouldContain("query=SELECT");
    testHandler.LastRequest!.RequestUri?.Query.ShouldContain("FROM+Customer");
  }
  
  public static async Task Retry_Once_On_401_Unauthorized()
  {
    // Arrange
    var testHandler = new TestHttpMessageHandler();
    var serviceProvider = CreateServiceProvider(testHandler);
    await SetupTestTokens(serviceProvider);
    
    var apiClient = serviceProvider.GetRequiredService<IQuickBooksApiClient>();
    int callCount = 0;
    
    testHandler.SetupResponseSequence(() =>
    {
      callCount++;
      if (callCount == 1)
      {
        return new HttpResponseMessage { StatusCode = HttpStatusCode.Unauthorized };
      }
      return new HttpResponseMessage
      {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent(JsonSerializer.Serialize(new { Success = true }), Encoding.UTF8, "application/json")
      };
    });
    
    // Act
    var result = await apiClient.GetAsync<JsonElement?>("test-realm-id", "test");
    
    // Assert
    result.ShouldNotBeNull();
    callCount.ShouldBe(2); // Should have retried once
  }
  
  public static async Task Parse_QuickBooks_Error_Response()
  {
    // Arrange
    var testHandler = new TestHttpMessageHandler();
    var serviceProvider = CreateServiceProvider(testHandler);
    await SetupTestTokens(serviceProvider);
    
    var apiClient = serviceProvider.GetRequiredService<IQuickBooksApiClient>();
    var errorResponse = new
    {
      Fault = new
      {
        Error = new[]
        {
          new
          {
            Message = "Invalid request",
            Detail = "The request is invalid",
            code = "2020"
          }
        },
        type = "ValidationFault"
      }
    };
    
    testHandler.SetupResponse(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.BadRequest,
      Content = new StringContent(JsonSerializer.Serialize(errorResponse), Encoding.UTF8, "application/json")
    });
    
    // Act & Assert
    var exception = await Should.ThrowAsync<QuickBooksApiException>(
      apiClient.GetAsync<JsonElement?>("test-realm-id", "test")
    );
    
    exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    exception.ErrorCode.ShouldBe("2020");
    exception.ErrorDetail.ShouldBe("The request is invalid");
    exception.ErrorType.ShouldBe("ValidationFault");
  }
  
  public static async Task Send_Delete_Request()
  {
    // Arrange
    var testHandler = new TestHttpMessageHandler();
    var serviceProvider = CreateServiceProvider(testHandler);
    await SetupTestTokens(serviceProvider);
    
    var apiClient = serviceProvider.GetRequiredService<IQuickBooksApiClient>();
    
    testHandler.SetupResponse(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.OK
    });
    
    // Act
    await apiClient.DeleteAsync("test-realm-id", "customer/123");
    
    // Assert
    testHandler.LastRequest.ShouldNotBeNull();
    testHandler.LastRequest!.Method.ShouldBe(HttpMethod.Delete);
    testHandler.LastRequest!.RequestUri?.PathAndQuery.ShouldContain("/v3/company/test-realm-id/customer/123");
  }
  
  public static async Task Send_Put_Request_With_Payload()
  {
    // Arrange
    var testHandler = new TestHttpMessageHandler();
    var serviceProvider = CreateServiceProvider(testHandler);
    await SetupTestTokens(serviceProvider);
    
    var apiClient = serviceProvider.GetRequiredService<IQuickBooksApiClient>();
    var payload = new { Id = "123", DisplayName = "Updated Customer" };
    var expectedResponse = new { Id = "123", DisplayName = "Updated Customer", SyncToken = "1" };
    
    testHandler.SetupResponse(new HttpResponseMessage
    {
      StatusCode = HttpStatusCode.OK,
      Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
    });
    
    // Act
    var result = await apiClient.PutAsync<JsonElement?>("test-realm-id", "customer", payload);
    
    // Assert
    result.ShouldNotBeNull();
    testHandler.LastRequest.ShouldNotBeNull();
    testHandler.LastRequest!.Method.ShouldBe(HttpMethod.Put);
    
    string requestContent = await testHandler.LastRequest!.Content!.ReadAsStringAsync();
    requestContent.ShouldContain("Updated Customer");
  }
}

/// <summary>
/// Test HTTP message handler for mocking HTTP responses.
/// </summary>
public class TestHttpMessageHandler : HttpMessageHandler
{
  private HttpResponseMessage? _response;
  private Func<HttpResponseMessage>? _responseFactory;
  
  public HttpRequestMessage? LastRequest { get; private set; }
  
  public void SetupResponse(HttpResponseMessage response)
  {
    _response = response;
    _responseFactory = null;
  }
  
  public void SetupResponseSequence(Func<HttpResponseMessage> responseFactory)
  {
    _responseFactory = responseFactory;
    _response = null;
  }
  
  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    LastRequest = request;
    
    // Clone content if present to preserve it for assertions
    if (request.Content != null)
    {
      string contentString = await request.Content.ReadAsStringAsync(cancellationToken);
      request.Content = new StringContent(contentString, Encoding.UTF8, "application/json");
    }
    
    if (_responseFactory != null)
    {
      return _responseFactory();
    }
    
    return _response ?? new HttpResponseMessage(HttpStatusCode.OK);
  }
}