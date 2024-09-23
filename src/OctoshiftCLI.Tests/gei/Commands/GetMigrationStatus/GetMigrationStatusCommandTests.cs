using FluentAssertions;
using OctoshiftCLI.GithubEnterpriseImporter.Commands.GetMigrationStatus;
using Xunit;

namespace OctoshiftCLI.Tests.GithubEnterpriseImporter.Commands.GetMigrationStatus;

public class GetMigrationStatusCommandTests
{
    [Fact]
    public void Should_Have_Options()
    {
        var command = new GetMigrationStatusCommand();
        command.Should().NotBeNull();
        command.Name.Should().Be("get-migration-status");
        command.Options.Count.Should().Be(4);

        TestHelpers.VerifyCommandOption(command.Options, "migration-id", true);
        TestHelpers.VerifyCommandOption(command.Options, "github-pat", false);
        TestHelpers.VerifyCommandOption(command.Options, "verbose", false);
        TestHelpers.VerifyCommandOption(command.Options, "target-api-url", false);
    }
}
