using System;
using System.IO;
using System.Threading.Tasks;
using System.CommandLine.Invocation;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Stylelabs.M.Sdk.WebClient;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Models;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Services;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.CommandHandlers
{
    public class SetActionSettingsPropCommandHandler : BaseCommandHandler
    {
        private readonly IOutputRenderer _renderer;
        private readonly ISetActionReqAttributesService _setActionReqAttributesService;

        public SetActionSettingsPropParameters Parameters { get; set; }

        public SetActionSettingsPropCommandHandler(Lazy<IWebMClient> client, 
            IOutputRenderer renderer,
            ISetActionReqAttributesService setActionSettingsPropService,
            IOptions<SetActionSettingsPropParameters> parameters) 
            : base(client, renderer)
        {
            _renderer = renderer;
            _setActionReqAttributesService = setActionSettingsPropService;
            Parameters = parameters?.Value;
        }

        public string Value { get; set; }

        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            _renderer.WriteLine($"Applying variables from a setting inside a package stored in '{Parameters.PackageDir.FullName}' ...");

            // First we need to find the setting
            string settingPath = Path.Combine(new string[] { Parameters.PackageDir.FullName, "settings", Parameters.SettingCategory, $"{Parameters.SettingName}.json" });

            // But of course, we will only be able to find it if it exists
            if (!File.Exists(settingPath))
            {
                throw new InvalidOperationException($"The Setting '{Parameters.SettingName}' in the Category '{Parameters.SettingCategory}' inside the Directory {Parameters.PackageDir.FullName} was not found");
            }

            var settingFile = new FileInfo(settingPath);
            var setting = await _setActionReqAttributesService.GetSettingAsync(settingFile);

            if (!Directory.Exists(Parameters.PackageDir.FullName))
            {
                throw new InvalidOperationException($"Directory {Parameters.PackageDir.FullName} was not found");
            }

            var jsonFilePaths = Directory.GetFiles(Parameters.PackageDir.FullName, "*.json", SearchOption.AllDirectories);

            if (setting.MSettingValue.ActionRequestAttributes != null && setting.MSettingValue.ActionRequestAttributes.Any())
            {
                foreach (var actionRequestAttributes in setting.MSettingValue.ActionRequestAttributes)
                {
                    var jsonFileOfAction = jsonFilePaths.Where(fileName => fileName.EndsWith($"{actionRequestAttributes.ActionIdentifier}.json"));
                    if (!jsonFileOfAction.Any())
                    {
                        _renderer.WriteLine($"The Action with identifier {actionRequestAttributes.ActionIdentifier} is used in the setting to set values, but the Action itself does not exist! Please update the '{Parameters.SettingName}' Setting!");
                        continue;
                    }

                    var fileAsToken = _setActionReqAttributesService.GetActionFileAsToken(jsonFileOfAction.First());
                    _setActionReqAttributesService.UpdateTokenBySetting(fileAsToken, actionRequestAttributes);
                    _setActionReqAttributesService.SaveActionFileFromToken(fileAsToken, jsonFileOfAction.First());
                }
            }
            else
            {
                _renderer.WriteLine("No ActionHeaders found to be set into actual action entities");
            }

            return 0;
        }
    }
}
