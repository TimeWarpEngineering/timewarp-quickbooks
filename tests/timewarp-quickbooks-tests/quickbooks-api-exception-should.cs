namespace TimeWarp.QuickBooks.Tests;

using System.Net;
using System.Net.Http;
using System.Text.Json;
using Shouldly;
using TimeWarp.Fixie;
using TimeWarp.QuickBooks.Api;

[TestTag(TestTags.Fast)]
public class QuickBooksApiException_Should_
{
  public static void Parse_QuickBooks_Error_Response()
  {
    // Arrange
    var errorResponse = new
    {
      Fault = new
      {
        Error = new[]
        {
          new
          {
            Message = "Invalid grant",
            Detail = "Token is invalid",
            code = "3200"
          }
        },
        type = "AuthenticationFault"
      }
    };
    
    string responseBody = JsonSerializer.Serialize(errorResponse);
    
    // Act
    var exception = new QuickBooksApiException(
      "Test error",
      HttpStatusCode.Unauthorized,
      "https://api.quickbooks.com/test",
      responseBody
    );
    
    // Assert
    exception.ErrorCode.ShouldBe("3200");
    exception.ErrorDetail.ShouldBe("Token is invalid");
    exception.ErrorType.ShouldBe("AuthenticationFault");
    exception.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    exception.RequestUrl.ShouldBe("https://api.quickbooks.com/test");
  }
  
  public static void Handle_Invalid_Json_In_Error_Response()
  {
    // Arrange
    string invalidJson = "This is not valid JSON";
    
    // Act
    var exception = new QuickBooksApiException(
      "Test error",
      HttpStatusCode.BadRequest,
      "https://api.quickbooks.com/test",
      invalidJson
    );
    
    // Assert
    exception.ErrorCode.ShouldBeNull();
    exception.ErrorDetail.ShouldBeNull();
    exception.ErrorType.ShouldBeNull();
    exception.ResponseBody.ShouldBe(invalidJson);
  }
  
  public static void Create_From_HttpResponseMessage()
  {
    // Arrange
    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.quickbooks.com/test");
    var response = new HttpResponseMessage(HttpStatusCode.NotFound)
    {
      RequestMessage = request
    };
    
    var errorResponse = new
    {
      Fault = new
      {
        Error = new[]
        {
          new
          {
            Message = "Resource not found",
            Detail = "The requested resource does not exist",
            code = "610"
          }
        },
        type = "ValidationFault"
      }
    };
    
    string responseBody = JsonSerializer.Serialize(errorResponse);
    
    // Act
    var exception = QuickBooksApiException.FromHttpResponse(response, responseBody);
    
    // Assert
    exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    exception.RequestUrl.ShouldBe("https://api.quickbooks.com/test");
    exception.ErrorCode.ShouldBe("610");
    exception.ErrorDetail.ShouldBe("The requested resource does not exist");
    exception.ErrorType.ShouldBe("ValidationFault");
  }
  
  public static void Include_All_Details_In_ToString()
  {
    // Arrange
    var errorResponse = new
    {
      Fault = new
      {
        Error = new[]
        {
          new
          {
            Message = "Validation error",
            Detail = "Field is required",
            code = "2020"
          }
        },
        type = "ValidationFault"
      }
    };
    
    string responseBody = JsonSerializer.Serialize(errorResponse);
    
    var exception = new QuickBooksApiException(
      "Validation failed",
      HttpStatusCode.BadRequest,
      "https://api.quickbooks.com/v3/company/123/customer",
      responseBody
    );
    
    // Act
    string result = exception.ToString();
    
    // Assert
    result.ShouldContain("Validation failed");
    result.ShouldContain("Error Code: 2020");
    result.ShouldContain("Error Type: ValidationFault");
    result.ShouldContain("Error Detail: Field is required");
    result.ShouldContain("Request URL: https://api.quickbooks.com/v3/company/123/customer");
  }
  
  public static void Handle_Multiple_Errors_Taking_First()
  {
    // Arrange
    var errorResponse = new
    {
      Fault = new
      {
        Error = new[]
        {
          new
          {
            Message = "First error",
            Detail = "First detail",
            code = "1001"
          },
          new
          {
            Message = "Second error",
            Detail = "Second detail",
            code = "1002"
          }
        },
        type = "ValidationFault"
      }
    };
    
    string responseBody = JsonSerializer.Serialize(errorResponse);
    
    // Act
    var exception = new QuickBooksApiException(
      "Multiple errors",
      HttpStatusCode.BadRequest,
      null,
      responseBody
    );
    
    // Assert
    exception.ErrorCode.ShouldBe("1001");
    exception.ErrorDetail.ShouldBe("First detail");
  }
  
  public static void Include_Inner_Exception()
  {
    // Arrange
    var innerException = new InvalidOperationException("Inner error");
    
    // Act
    var exception = new QuickBooksApiException(
      "Outer error",
      HttpStatusCode.InternalServerError,
      "https://api.quickbooks.com/test",
      null,
      innerException
    );
    
    // Assert
    exception.InnerException.ShouldBe(innerException);
    exception.Message.ShouldBe("Outer error");
  }
}