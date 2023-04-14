using Sitecore.CH.Cli.Core.Abstractions.Commands;
using System.IO;
using Sitecore.CH.Cli.Plugin.EmailTemplates.CommandHandlers;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.Commands
{
    public class ImportCommand : BaseCommand<ImportCommandHandler>
    {
        public ImportCommand() : base("import", "Imports email templates from disk")
        {
            // optionS
            AllowOverrideEndpoint = true;
            AddOption<FileInfo>("Specifies the source directory name", true, "--source", "-s");
            AddOption<bool>("Delete email templates on target", false, "--delete", "-d");
            AddOption<bool>("What if option. Do not perform any change.", false, "--whatif", "-wif");
        }
    }
}
