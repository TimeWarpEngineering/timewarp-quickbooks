#region Purpose
// Executes the full CI workflow
#endregion
#region Design
// Verify-only scaffold: self-install -> clean -> build -> test.
// Self-install uses the runfile because bin/dev is gitignored and missing on a
// fresh CI checkout. Later steps use ./bin/dev (bin/dev.exe on Windows).
// Repos that publish NuGet replace this with the Nuru promotion pipeline
// (PR/merge packs the IsPackable set; release downloads Packages-* and pushes).
#endregion

namespace DevCli.Commands;

[NuruRoute("workflow", Description = "Execute full CI workflow")]
public sealed class WorkflowCommand : ICommand<Unit>
{
  public sealed class Handler : ICommandHandler<WorkflowCommand, Unit>
  {
    private readonly ITerminal Terminal;
    private CancellationToken Ct;
    private string RepoRoot = null!;
    private string DevBin = null!;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async Task<Unit> Handle(WorkflowCommand command, CancellationToken ct)
    {
      Ct = ct;

      if (!FindRepoRoot()) return Unit.Value;
      if (!await SelfInstallAsync()) return Unit.Value;
      if (!await RunStepAsync("clean", "Clean failed!")) return Unit.Value;
      if (!await RunStepAsync("build", "Build failed!")) return Unit.Value;
      if (!await RunStepAsync("test", "Tests failed!")) return Unit.Value;

      Terminal.WriteLine("\nWorkflow completed successfully!".Green());
      return Unit.Value;
    }

    private bool FindRepoRoot()
    {
      string? root = Git.FindRoot();
      if (root is null)
      {
        Terminal.WriteErrorLine("Error: could not find repository root.");
        Environment.ExitCode = 1;
        return false;
      }
      RepoRoot = root;
      string binaryName = OperatingSystem.IsWindows() ? "dev.exe" : "dev";
      DevBin = Path.Combine(RepoRoot, "bin", binaryName);
      Terminal.WriteLine("Starting CI workflow...");
      Terminal.WriteLine("Pipeline: self-install -> clean -> build -> test");
      return true;
    }

    private async Task<bool> SelfInstallAsync()
    {
      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("run", "--file", "tools/dev-cli/dev.cs", "--", "self-install")
        .WithWorkingDirectory(RepoRoot)
        .WithNoValidation()
        .RunAsync(Ct);

      if (exitCode != 0)
      {
        Terminal.WriteErrorLine("Self-install failed!".Red());
        Environment.ExitCode = exitCode;
        return false;
      }

      return true;
    }

    private async Task<bool> RunStepAsync(string subcommand, string failureMessage)
    {
      int exitCode = await Shell.Builder(DevBin)
        .WithArguments(subcommand)
        .WithNoValidation()
        .RunAsync(Ct);

      if (exitCode != 0)
      {
        Terminal.WriteErrorLine(failureMessage.Red());
        Environment.ExitCode = exitCode;
        return false;
      }
      return true;
    }
  }
}
