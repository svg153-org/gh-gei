using OctoshiftCLI.Extensions;
using OctoshiftCLI.Services;

namespace OctoshiftCLI.Commands.GetMigrationStatus;

public class GetMigrationStatusCommandArgs : CommandArgs
{
    public string MigrationId { get; set; }
    [Secret]
    public string GithubPat { get; set; }
    public string TargetApiUrl { get; set; }

    public override void Validate(OctoLogger log)
    {
        if (MigrationId.IsNullOrWhiteSpace())
        {
            throw new OctoshiftCliException("MigrationId must be provided");
        }

        if (!MigrationId.StartsWith(WaitForMigrationCommandArgs.REPO_MIGRATION_ID_PREFIX) && !MigrationId.StartsWith(WaitForMigrationCommandArgs.ORG_MIGRATION_ID_PREFIX))
        {
            throw new OctoshiftCliException($"Invalid migration id: {MigrationId}");
        }
    }
}
