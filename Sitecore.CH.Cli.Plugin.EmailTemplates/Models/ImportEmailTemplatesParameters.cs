using System.IO;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.Models
{
    public class ImportEmailTemplatesParameters
    {
        public FileInfo Source { get; set; }
        public bool Delete { get; set; }
        public bool WhatIf { get; set; }
    }
}