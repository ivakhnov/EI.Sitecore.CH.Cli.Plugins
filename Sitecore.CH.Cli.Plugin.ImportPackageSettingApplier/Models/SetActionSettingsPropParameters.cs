using System.IO;

namespace Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Models
{
    public class SetActionSettingsPropParameters
    {
        public FileInfo PackageDir { get; set; }
        public string SettingCategory { get; set; }
        public string SettingName { get; set; }
    }
   

}