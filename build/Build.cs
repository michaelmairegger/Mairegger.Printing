using Nuke.Common;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Serilog;

[GitHubActions("build", GitHubActionsImage.WindowsLatest,
    On = [GitHubActionsTrigger.Push],
    FetchDepth = 50,
    InvokedTargets = [nameof(Compile)])]
[GitHubActions("publish", GitHubActionsImage.WindowsLatest,
    OnPushTags = ["v*.*.*"],
    FetchDepth = 50,
    ImportSecrets = [nameof(NuGetApiKey)],
    InvokedTargets = [nameof(Publish)])]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Required]
    [NerdbankGitVersioning(UpdateBuildNumber = true)]
    private readonly NerdbankGitVersioning NerdbankVersioning;

    [Parameter] [Secret] readonly string NuGetApiKey;

    Target Clean => t => t
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean();
        });

    Target Restore => t => t
        .Executes(() =>
        {
            DotNetRestore();
        });

    Target Compile => t => t
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Information("NerdbankVersioning = {Value}", NerdbankVersioning.SimpleVersion);
            DotNetBuild(c => c.EnableNoRestore()
                .SetConfiguration(Configuration)
            );
        });

    Target Test => t => t
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(c => c
                .EnableNoBuild()
                .SetConfiguration(Configuration)
                .SetDataCollector("XPlat Code Coverage")
            );
        });

    Target Pack => t => t
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(c => c
                .EnableNoBuild()
                .SetVersion(NerdbankVersioning.SimpleVersion)
                .SetConfiguration(Configuration)
            );
        });

    Target Publish => t => t
        .Requires(() => NuGetApiKey)
        .DependsOn(Pack)
        .Executes(() =>
        {
            DotNetNuGetPush();
        });
}
