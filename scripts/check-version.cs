#!/usr/bin/env -S dotnet --
// CheckVersion.cs - Check if NuGet packages are already published
#:package TimeWarp.Amuru
#:package TimeWarp.Amuru.Tools
#:package TimeWarp.Terminal

using System.Xml.Linq;
using TimeWarp.Amuru;
using static TimeWarp.Terminal.Terminal;

// Change to script directory for relative paths
string scriptDir = (AppContext.GetData("EntryPointFileDirectoryPath") as string)!;
Directory.SetCurrentDirectory(scriptDir);

// Read version from Directory.Build.props
string propsPath = "../Directory.Build.props";
var doc = XDocument.Load(propsPath);
string? version = doc.Descendants("Version").FirstOrDefault()?.Value;

if (string.IsNullOrEmpty(version))
{
    WriteLine("❌ Could not find version in Directory.Build.props");
    Environment.Exit(1);
}

WriteLine($"Checking if packages with version {version} are already published on NuGet.org...");

// Packages to check
string[] packages = ["TimeWarp.QuickBooks"];
bool anyPublished = false;

foreach (string package in packages)
{
    WriteLine($"\nChecking {package}...");

    CommandResult searchResult = DotNet.PackageSearch(package)
        .WithExactMatch()
        .WithPrerelease()
        .WithSource("https://api.nuget.org/v3/index.json")
        .Build();

    CommandOutput result = await searchResult.CaptureAsync();

    // Check if the version appears in the output
    if (result.Stdout.Contains($"| {version} |", StringComparison.Ordinal))
    {
        WriteLine($"⚠️  WARNING: {package} {version} is already published to NuGet.org");
        anyPublished = true;
    }
    else
    {
        WriteLine($"✅ {package} {version} is not yet published on NuGet.org");
    }
}

if (anyPublished)
{
    WriteLine("\n❌ One or more packages are already published. Please increment the version in Directory.Build.props");
    Environment.Exit(1);
}

WriteLine("\n✅ All packages are ready to publish!");