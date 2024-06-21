using System.IO;

namespace Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Models
{
    public class PackageCleanerParameters
    {
        public FileInfo PackageDir { get; set; }
        public bool ShouldCleanPortalComponents { get; set; }
        public bool ShouldCleanActionApiUrls { get; set; }
    }
   

}