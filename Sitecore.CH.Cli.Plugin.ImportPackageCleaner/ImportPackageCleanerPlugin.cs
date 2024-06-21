using System.CommandLine;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.CH.Cli.Core.Abstractions.Infrastructure;
using Sitecore.CH.Cli.Plugin.ImportPackageCleaner.CommandHandlers;
using Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Commands;
using Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Services;
using Sitecore.CH.Cli.Core.Extensions;
using Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Models;

namespace Sitecore.CH.Cli.Plugin.ImportPackageCleaner
{
    public class ImportPackageCleanerPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandHandler<PackageCleanerCommandHandler, PackageCleanerParameters>();
            services.AddSingleton<IPackageCleanerService, PackageCleanerService>();
        }

        public void RegisterCommands(ICommandRegistry registry)
        {
            ICommandRegistry commandRegistry = registry;
            List<Command> commands = new List<Command>
            {
                new PackageCleanerCommand()
            };
            string commandGroupEmailTemplates = "Commands for cleaning the import package, make it ready to be added to a version control system repository.";
            commandRegistry.RegisterCommandGroup("import-package-cleaner", commands, commandGroupEmailTemplates);
        }
    }
}