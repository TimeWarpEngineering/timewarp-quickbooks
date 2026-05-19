# File and Directory Naming Convention

Everything uses kebab-case. No exceptions.

## Directory Naming
- Use kebab-case: `timewarp-code`, `my-feature`, `developer-tools`
- No capital letters
- Words separated by hyphens

## File Naming
- Source files: `program.cs`, `my-class.cs`, `data-service.cs`
- Project files: `timewarp-code.csproj`
- Documentation: `file-naming-conventions.md`, `overview.md`

## C# Namespace Handling
C# namespaces cannot contain hyphens. The .NET tooling automatically converts kebab-case to underscore format:
- Directory: `timewarp-code`
- Namespace: `timewarp_code`

## .NET Project Creation
When creating new projects, use only the `-o` parameter:
```bash
dotnet new console --aot --framework net10.0 -o project-name
cd project-name
mv Program.cs program.cs
```

This ensures:
- Consistent kebab-case directory and file names
- Automatic namespace conversion
- Manual rename of `Program.cs` to maintain convention