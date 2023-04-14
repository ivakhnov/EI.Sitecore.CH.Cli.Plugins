using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Stylelabs.M.Sdk.WebClient;
using System;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Models;
using System.IO;
using System.Linq;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Services;
using Microsoft.Extensions.Options;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.CommandHandlers
{
    public class CompareCommandHandler : BaseCommandHandler
    {
        private readonly IOutputRenderer _renderer;
        private readonly IEmailTemplatesService _emailTemplatesService;

        public CompareEmailTemplatesParameters Parameters { get; set; }

        public CompareCommandHandler(Lazy<IWebMClient> client, 
            IOutputRenderer renderer, 
            IEmailTemplatesService emailTemplatesService,
            IOptions<CompareEmailTemplatesParameters> parameters) 
            : base(client, renderer)
        {
            _renderer = renderer;
            _emailTemplatesService = emailTemplatesService;
            Parameters = parameters?.Value;
        }

        public string Value { get; set; }
        static string CopyProfileIdentifierSelector(EmailTemplatesDTO emailTemplate) => emailTemplate.Identifier;

        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            _renderer.WriteLine($"Comparing email templates from {Parameters.Source.FullName}...");

            var di = new DirectoryInfo(Parameters.Source.FullName);
            var filesToImport = di.GetFiles("*.json");

            var sourceEmailTemplates = await _emailTemplatesService.ReadEmailTemplatesAsync(filesToImport);

            var targetEmailTemplates = await _emailTemplatesService.GetEmailTemplatesAsync();

            var toDelete = targetEmailTemplates.Except(sourceEmailTemplates, new IdentifierComparer()).ToList();
            var toCreate = sourceEmailTemplates.Except(targetEmailTemplates, new IdentifierComparer()).ToList();
            var toUpdate = _emailTemplatesService.CompareEmailTemplates(targetEmailTemplates, sourceEmailTemplates);

            toDelete.ForEach(cp => _renderer.WriteLine($"Email template should be deleted: {cp.Identifier}"));
            toCreate.ForEach(cp => _renderer.WriteLine($"Email template should be created: {cp.Identifier}"));
            toUpdate.ForEach(cp => _renderer.WriteLine($"Email template should be updated: {cp.Identifier}"));

            return 0;
        }
    }
}
