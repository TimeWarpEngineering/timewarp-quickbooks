# 001 - Build TimeWarp.QuickBooks.Tests Project

## Description

Create a new test project called TimeWarp.QuickBooks.Tests using the TimeWarp.Fixie testing framework. This will establish the foundation for all testing in the QuickBooks project.

## Requirements

- Set up a proper test project structure following TimeWarp conventions
- Implement TimeWarp.Fixie testing framework
- Include sample ConventionTests.cs file

## Checklist

### Design
- [x] Add/Update Tests

### Implementation
- [x] Create new test project TimeWarp.QuickBooks.Tests
- [x] Add required NuGet packages (TimeWarp.Fixie, Fixie.TestAdapter, Shouldly)
- [x] Create dotnet tool manifest and install Fixie.Console
- [x] Configure TestingConvention class
- [x] Add ConventionTests.cs example file
- [x] Verify tests run successfully
- [x] Move project to Tests directory

### Documentation
- [x] Update Documentation

## Notes

Reference: [TimeWarp.Fixie GitHub Repository](https://github.com/TimeWarpEngineering/timewarp-fixie)

## Implementation Notes

The TimeWarp.QuickBooks.Tests project has been successfully created and configured:

1. Created a new classlib project named TimeWarp.QuickBooks.Tests
2. Added the required NuGet packages:
   - TimeWarp.Fixie
   - Fixie.TestAdapter
   - Shouldly
3. Created a dotnet tool manifest and installed Fixie.Console
4. Created the TestingConvention class that inherits from TimeWarp.Fixie.TestingConvention
5. Added the ConventionTests.cs example file with various test examples:
   - Basic test
   - Test with Skip attribute
   - Test with TestTag attribute
   - Parameterized tests using Input attribute
6. Verified tests run successfully with the following results:
   - 4 passed tests
   - 1 skipped test (as expected with the Skip attribute)
   - Test execution completed in 0.33 seconds
7. Moved the project to the Tests directory to follow project structure conventions

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
   ```

8. Move project to Tests directory to follow project structure conventions