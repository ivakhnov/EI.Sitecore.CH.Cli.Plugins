using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Services
{
    public interface IPackageCleanerService
    {
        HashSet<string> TypesOfComponentsFoundOnRelated { get; }

        JToken GetToken(string jsonFilePath);
        void UpdateToken(JToken token, bool shouldCleanPortalComponents, bool shouldCleanActionApiUrls);
        void SaveFile(JToken token, string jsonFilePath);
    }
}
