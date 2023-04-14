using System.IO;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Plugin.EmailTemplates.CommandHandlers;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.Commands
{
    public class ExportCommand : BaseCommand<ExportCommandHandler>
    {
        public ExportCommand() : base("export", "Exports email templates to disk")
        {
            // options
            AllowOverrideEndpoint = true;
            AddOption<FileInfo>("Specifies the output directory name", isRequired: true, "--out", "-o");
        }
    }
}