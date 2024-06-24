using System;
using System.IO;
using System.Threading.Tasks;
using System.CommandLine.Invocation;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Stylelabs.M.Sdk.WebClient;
using Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Models;
using Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Services;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Sitecore.CH.Cli.Plugin.ImportPackageCleaner.CommandHandlers
{
    public class PackageCleanerCommandHandler : BaseCommandHandler
    {
        private readonly IOutputRenderer _renderer;
        private readonly IPackageCleanerService _packageCleanerService;

        public PackageCleanerParameters Parameters { get; set; }

        public PackageCleanerCommandHandler(Lazy<IWebMClient> client, 
            IOutputRenderer renderer, 
            IPackageCleanerService packageCleanerService,
            IOptions<PackageCleanerParameters> parameters) 
            : base(client, renderer)
        {
            _renderer = renderer;
            _packageCleanerService = packageCleanerService;
            Parameters = parameters?.Value;
        }

        public string Value { get; set; }

        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            _renderer.WriteLine($"Cleaning the import package stored in '{Parameters.PackageDir.FullName}' ...");

            if (!Directory.Exists(Parameters.PackageDir.FullName))
            {
                throw new InvalidOperationException($"Directory {Parameters.PackageDir.FullName} was not found");
            }

            var jsonFilePaths = Directory.GetFiles(Parameters.PackageDir.FullName, "*.json", SearchOption.AllDirectories);


            if (jsonFilePaths != null && jsonFilePaths.Any())
            {
                _renderer.WriteLine($"Found {jsonFilePaths.Count()} files to process");

                foreach (var jsonFilePath in jsonFilePaths)
                {
                    var token = _packageCleanerService.GetToken(jsonFilePath);
                    _packageCleanerService.UpdateToken(token, Parameters.ShouldCleanPortalComponents, Parameters.ShouldCleanActionVariables);
                    _packageCleanerService.SaveFile(token, jsonFilePath);
                }
                _renderer.WriteLine($"Done!");

                if (_packageCleanerService.TypesOfComponentsFoundOnRelated.Any())
                {
                    _renderer.WriteLine($"Related item types found");
                    foreach (var item in _packageCleanerService.TypesOfComponentsFoundOnRelated)
                    {
                        _renderer.WriteLine(item);
                    }
                }
            }
            else
            {
                _renderer.WriteLine("No Files Found");
            }

            return 0;
        }
    }
}
