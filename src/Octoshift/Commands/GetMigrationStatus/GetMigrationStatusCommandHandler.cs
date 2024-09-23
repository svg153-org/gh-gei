using System;
using System.Threading.Tasks;
using OctoshiftCLI.Services;

namespace OctoshiftCLI.Commands.GetMigrationStatus;

public class GetMigrationStatusCommandHandler : ICommandHandler<GetMigrationStatusCommandArgs>
{
    private readonly OctoLogger _log;
    private readonly GithubApi _githubApi;
    private readonly WarningsCountLogger _warningsCountLogger;

    public GetMigrationStatusCommandHandler(OctoLogger log, GithubApi githubApi, WarningsCountLogger warningsCountLogger)
    {
        _log = log;
        _githubApi = githubApi;
        _warningsCountLogger = warningsCountLogger;
    }

    public async Task Handle(GetMigrationStatusCommandArgs args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (args.MigrationId.StartsWith(WaitForMigrationCommandArgs.REPO_MIGRATION_ID_PREFIX))
        {
            await ReportRepositoryMigrationStatus(args.MigrationId, _githubApi);
        }
        else
        {
            await ReportOrgMigrationStatus(args.MigrationId, _githubApi);
        }
    }

    private async Task ReportOrgMigrationStatus(string migrationId, GithubApi githubApi)
    {
        var (state, sourceOrgUrl, targetOrgName, failureReason, remainingRepositoriesCount, totalRepositoriesCount) = await githubApi.GetOrganizationMigration(migrationId);

        _log.LogInformation($"Migration {migrationId} status:");
        _log.LogInformation($"Source Org URL: {sourceOrgUrl}");
        _log.LogInformation($"Target Org Name: {targetOrgName}");
        _log.LogInformation($"State: {state}");

        if (OrganizationMigrationStatus.IsFailed(state))
        {
            _log.LogError($"Migration {migrationId} failed. Failure reason: {failureReason}");
        }
        else if (OrganizationMigrationStatus.IsRepoMigration(state))
        {
            var completedRepositoriesCount = (int)totalRepositoriesCount - (int)remainingRepositoriesCount;
            _log.LogInformation($"Repositories migrated: {completedRepositoriesCount}/{totalRepositoriesCount}");
        }
    }

    private async Task ReportRepositoryMigrationStatus(string migrationId, GithubApi githubApi)
    {
        var (state, repositoryName, warningsCount, failureReason, migrationLogUrl) = await githubApi.GetMigration(migrationId);

        _log.LogInformation($"Migration {migrationId} status:");
        _log.LogInformation($"Repository Name: {repositoryName}");
        _log.LogInformation($"State: {state}");

        if (RepositoryMigrationStatus.IsFailed(state))
        {
            _log.LogError($"Migration {migrationId} failed. Failure reason: {failureReason}");
            _warningsCountLogger.LogWarningsCount(warningsCount);
            _log.LogInformation($"Migration log available at {migrationLogUrl}");
        }
        else if (RepositoryMigrationStatus.IsSucceeded(state))
        {
            _log.LogSuccess($"Migration {migrationId} succeeded for {repositoryName}");
            _warningsCountLogger.LogWarningsCount(warningsCount);
            _log.LogInformation($"Migration log available at {migrationLogUrl}");
        }
    }
}
