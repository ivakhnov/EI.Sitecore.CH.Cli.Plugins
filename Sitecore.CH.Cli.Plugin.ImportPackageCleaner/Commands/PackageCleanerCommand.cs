using System.IO;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Plugin.ImportPackageCleaner.CommandHandlers;

namespace Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Commands
{
    public class PackageCleanerCommand : BaseCommand<PackageCleanerCommandHandler>
    {
        public PackageCleanerCommand() : base("clean", "Cleans the import package on the disk, make it ready to be added to a version control system repository")
        {
            // options
            AddOption<FileInfo>("Specifies the package directory", true, "--packagedir", "-d");
            AddOption<bool>("If Portal Components should be cleaned from Portal Pages in related section", false, "--ShouldCleanPortalComponents", "-portal");
            AddOption<bool>("If should clean the variable details in M.Action entities (e.g. apiUrl, headers, connectionString)", false, "--ShouldCleanActionVariables", "-actionvars");
        }
    }
}
