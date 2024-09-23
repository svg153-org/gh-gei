using System;
using System.CommandLine;
using OctoshiftCLI.Commands.WaitForMigration;
using OctoshiftCLI.Contracts;
using OctoshiftCLI.Services;

namespace OctoshiftCLI.GithubEnterpriseImporter.Commands.GetMigrationStatus;

public sealed class GetMigrationStatusCommand : CommandBase<GetMigrationStatusCommandArgs, GetMigrationStatusCommandHandler>
{
    public GetMigrationStatusCommand() : base(
        name: "get-migration-status",
        description: "Reports the status of the migration without waiting.")
    {
        AddOptions();
    }

    public Option<string> MigrationId { get; } = new("--migration-id")
    {
        IsRequired = true,
        Description = "Reports the status of the specified migration."
    };

    public Option<string> GithubPat { get; } = new("--github-pat")
    {
        Description = "Personal access token of the GitHub target. Overrides GH_PAT environment variable."
    };

    public Option<string> TargetApiUrl { get; } = new("--target-api-url")
    {
        Description = "The URL of the target API, if not migrating to github.com. Defaults to https://api.github.com"
    };

    public Option<bool> Verbose { get; } = new("--verbose");

    protected void AddOptions()
    {
        AddOption(MigrationId);
        AddOption(GithubPat);
        AddOption(Verbose);
        AddOption(TargetApiUrl);
    }

    public override GetMigrationStatusCommandHandler BuildHandler(GetMigrationStatusCommandArgs args, IServiceProvider sp)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (sp is null)
        {
            throw new ArgumentNullException(nameof(sp));
        }

        var log = sp.GetRequiredService<OctoLogger>();
        var githubApi = sp.GetRequiredService<ITargetGithubApiFactory>().Create(args.TargetApiUrl, args.GithubPat);
        var warningsCountLogger = sp.GetRequiredService<WarningsCountLogger>();

        return new GetMigrationStatusCommandHandler(log, githubApi, warningsCountLogger);
    }
}
