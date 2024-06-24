using System.CommandLine;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.CH.Cli.Core.Extensions;
using Sitecore.CH.Cli.Core.Abstractions.Infrastructure;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.CommandHandlers;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Commands;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Services;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Models;

namespace Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier
{
    public class ImportPackageSettingApplierPlugin : IPlugin
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandHandler<SetActionSettingsPropCommandHandler, SetActionSettingsPropParameters>();
            services.AddSingleton<ISetActionReqAttributesService, SetActionReqAttributesService>();
        }

        public void RegisterCommands(ICommandRegistry registry)
        {
            ICommandRegistry commandRegistry = registry;
            List<Command> commands = new List<Command>
            {
                new SetActionSettingsPropCommand()
            };
            string commandGroupEmailTemplates = "Commands for setting the variables values from a setting inside the import package to actual entities in it (e.g. action apiUrls, connectionStrings, etc), make it ready to be imported into an environment after being extracted from another.";
            commandRegistry.RegisterCommandGroup("import-package-setting-applier", commands, commandGroupEmailTemplates);
        }
    }
}