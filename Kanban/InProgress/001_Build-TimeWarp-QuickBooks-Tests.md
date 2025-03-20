# 001 - Build TimeWarp.QuickBooks.Tests Project

## Description

Create a new test project called TimeWarp.QuickBooks.Tests using the TimeWarp.Fixie testing framework. This will establish the foundation for all testing in the QuickBooks project.

## Requirements

- Set up a proper test project structure following TimeWarp conventions
- Implement TimeWarp.Fixie testing framework
- Include sample ConventionTests.cs file

## Checklist

### Design
- [ ] Add/Update Tests

### Implementation
- [ ] Create new test project TimeWarp.QuickBooks.Tests
- [ ] Add required NuGet packages (TimeWarp.Fixie, Fixie.TestAdapter, Shouldly)
- [ ] Create dotnet tool manifest and install Fixie.Console
- [ ] Configure TestingConvention class
- [ ] Add ConventionTests.cs example file
- [ ] Verify tests run successfully

### Documentation
- [ ] Update Documentation

## Notes

Reference: [TimeWarp.Fixie GitHub Repository](https://github.com/TimeWarpEngineering/timewarp-fixie)

## Implementation Steps

1. Create a new test project:
   ```console
   dotnet new classlib -n TimeWarp.QuickBooks.Tests
   ```

2. Add NuGet packages to the project:
   ```console
   dotnet add package TimeWarp.Fixie
   dotnet add package Fixie.TestAdapter
   dotnet add package Shouldly
   ```

3. Create a dotnet tool manifest:
   ```console
   dotnet new tool-manifest
   ```

4. Add Fixie.Console to the manifest:
   ```console
   dotnet tool install Fixie.Console
   ```

5. Create TestingConvention class:
   ```csharp
   namespace TimeWarp.QuickBooks.Tests;

   public class TestingConvention : TimeWarp.Fixie.TestingConvention { }
   ```

6. Add ConventionTests.cs example file:
   ```csharp
   namespace TimeWarp.QuickBooks.Tests;

   using Shouldly;
   using TimeWarp.Fixie;

   [TestTag(TestTags.Fast)]
   public class SimpleNoApplicationTest_Should_
   {
     public static void AlwaysPass() => true.ShouldBeTrue();

     [Skip("Demonstrates skip attribute")]
     public static void SkipExample() => true.ShouldBeFalse();

     [TestTag(TestTags.Fast)]
     public static void TagExample() => true.ShouldBeTrue();

     [Input(5, 3, 2)]
     [Input(8, 5, 3)]
     public static void Subtract(int x, int y, int expectedDifference)
     {
       int result = x - y;
       result.ShouldBe(expectedDifference);
     }
   }
   ```

7. Run tests to verify setup:
   ```console
   dotnet fixie