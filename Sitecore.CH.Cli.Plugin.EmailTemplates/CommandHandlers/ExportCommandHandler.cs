using System;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Models;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Services;
using Stylelabs.M.Sdk.WebClient;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.CommandHandlers
{
    public class ExportCommandHandler : BaseCommandHandler
    {

        private readonly IOutputRenderer _renderer;
        private readonly IEmailTemplatesService _emailTemplatesService;

        public ExportEmailTemplatesParameters Parameters { get; set; }

        public ExportCommandHandler(Lazy<IWebMClient> client,
            IOutputRenderer renderer,
            IEmailTemplatesService emailTemplatesService,
            IOptions<ExportEmailTemplatesParameters> parameters)
            : base(client, renderer)
        {
            _renderer = renderer;
            _emailTemplatesService = emailTemplatesService;
            Parameters = parameters?.Value;
        }
        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            _renderer.WriteLine($"Exporting email templates to {Parameters.Out.FullName}...");
            
            var emailTemplates = await _emailTemplatesService.GetEmailTemplatesAsync();

            foreach (var emailTemplate in emailTemplates.OrderByDescending(p => p.ModifiedOn))
            {
                var jsonString = JObject.FromObject(emailTemplate);
                var exportFilePath = Path.Combine(Parameters.Out.FullName, $"{emailTemplate.Identifier}.json");
                _renderer.WriteLine($"Exporting email template {emailTemplate.Identifier}");
                await File.WriteAllTextAsync(exportFilePath, jsonString.ToString());
            }

            return 0;
        }
    }
}