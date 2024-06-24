using System.IO;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.CommandHandlers;

namespace Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Commands
{
    public class SetActionSettingsPropCommand : BaseCommand<SetActionSettingsPropCommandHandler>
    {
        public SetActionSettingsPropCommand() : base("actions", "Applies variables from a Setting in the package into the 'Settings' prop of all Actions to override apiUrl, header, connectionString, etc")
        {
            // options
            AddOption<FileInfo>("Specifies the package directory", true, "--packagedir", "-d");
            AddOption<string>("The Category of the Setting that contains the variables which should be set into the Action entities", true, "--SettingCategory", "-cat");
            AddOption<string>("The Name of the Setting that contains the variables which should be set into the Action entities", true, "--SettingName", "-name");
        }
    }
}
