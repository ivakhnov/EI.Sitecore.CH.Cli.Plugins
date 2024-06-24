using Newtonsoft.Json.Linq;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Models;
using System.Collections.Generic;

namespace Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Services
{
    public interface ISetActionReqAttributesService
    {
        public Task<SettingDTO> GetSettingAsync(FileInfo settingFile);

        JToken GetActionFileAsToken(string jsonFilePath);
        void UpdateTokenBySetting(JToken token, ActionRequestAttributes reqAttributes);
        void SaveActionFileFromToken(JToken token, string jsonFilePath);
    }
}
