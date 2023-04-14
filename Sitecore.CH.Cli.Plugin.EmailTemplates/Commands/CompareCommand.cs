using System.IO;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Plugin.EmailTemplates.CommandHandlers;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.Commands
{
    public class CompareCommand : BaseCommand<CompareCommandHandler>
    {
        public CompareCommand() : base("compare", "Compares email templates between disk and endpoint")
        {
            // options
            AllowOverrideEndpoint = true;
            AddOption<FileInfo>("Specifies the source directory name", true, "--source", "-s");
        }
    }
}
