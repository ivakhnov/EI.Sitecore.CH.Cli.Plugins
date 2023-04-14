
using Sitecore.CH.Cli.Core.Abstractions.Infrastructure;
using System.Collections.Generic;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.CH.Cli.Plugin.EmailTemplates.CommandHandlers;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Commands;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Services;
using Sitecore.CH.Cli.Core.Extensions;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Models;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates
{
    public class EmailTemplatesPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandHandler<ExportCommandHandler, ExportEmailTemplatesParameters>();
            services.AddCommandHandler<ImportCommandHandler, ImportEmailTemplatesParameters>();
            services.AddCommandHandler<CompareCommandHandler, CompareEmailTemplatesParameters>();
            services.AddSingleton<IEmailTemplatesService, EmailTemplatesService>();
        }

        public void RegisterCommands(ICommandRegistry registry)
        {
            ICommandRegistry commandRegistry = registry;
            List<Command> commands = new List<Command>
            {
                new ExportCommand(),
                new ImportCommand(),
                new CompareCommand()
            };
            string commandGroupEmailTemplates = "Commands for import/export of email templates";
            commandRegistry.RegisterCommandGroup("email-templates", commands, commandGroupEmailTemplates);
        }
    }
}
