using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sitecore.CH.Cli.Core.Abstractions.Commands;
using Sitecore.CH.Cli.Core.Abstractions.Rendering;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Models;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Services;
using Stylelabs.M.Sdk.WebClient;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.CommandHandlers
{
    public class ImportCommandHandler : BaseCommandHandler
    {
        private readonly IOutputRenderer _renderer;
        private readonly IEmailTemplatesService _emailTemplatesService;

        public ImportEmailTemplatesParameters Parameters { get; set; }

        public ImportCommandHandler(Lazy<IWebMClient> client,
            IOutputRenderer renderer, 
            IEmailTemplatesService emailTemplatesService,
            IOptions<ImportEmailTemplatesParameters> parameters) 
            : base(client, renderer)
        {
            _renderer = renderer;
            _emailTemplatesService = emailTemplatesService;
            Parameters = parameters?.Value;
        }
        public override async Task<int> InvokeAsync(InvocationContext context)
        {
            _renderer.WriteLine($"Importing email templates from {Parameters.Source.FullName}...");

            var di = new DirectoryInfo(Parameters.Source.FullName);
            var filesToImport = di.GetFiles("*.json");

            var sourceEmailTemplates = await _emailTemplatesService.ReadEmailTemplatesAsync(filesToImport);

            var targetEmailTemplates = await _emailTemplatesService.GetEmailTemplatesAsync();

            var toDelete = targetEmailTemplates.Except(sourceEmailTemplates, new IdentifierComparer()).ToList();
            var toCreate = sourceEmailTemplates.Except(targetEmailTemplates, new IdentifierComparer()).ToList();
            var toUpdate = _emailTemplatesService.CompareEmailTemplates(targetEmailTemplates, sourceEmailTemplates);
   
            foreach (var emt in toDelete)
            {
                await DeleteEmailTemplateAsync(emt);
            }

            foreach (var emt in toCreate)
            {
                await CreateEmailTemplateAsync(emt);
            }

            foreach (var emt in toUpdate)
            {
                await UpdateEmailTemplateAsync(emt, targetEmailTemplates, sourceEmailTemplates);
            }
            
            return 0;
        }

        private async Task UpdateEmailTemplateAsync(EmailTemplatesDTO emt, List<EmailTemplatesDTO> targetEmailTemplates, List<EmailTemplatesDTO> sourceEmailTemplates)
        {
            _renderer.WriteLine($"Update the email template: '{emt.Identifier}'.");
            if (Parameters.WhatIf) return;

            var source = sourceEmailTemplates.Single(s => s.Identifier == emt.Identifier);
            var target = targetEmailTemplates.Single(t => t.Identifier == emt.Identifier);

            await _emailTemplatesService.UpdateEmailTemplateAsync(source, target);

            _renderer.WriteLine($"Email template: '{emt.Identifier}' updated.");
        }

        private async Task CreateEmailTemplateAsync(EmailTemplatesDTO cp)
        {
            if (Parameters.WhatIf)
            {
                _renderer.WriteLine($"[WhatIf] Create an email template: '{cp.Identifier}'");
                return;
            }
            try
            {
                _renderer.WriteLine($"Create an email template: '{cp.Identifier}'. whatif={Parameters.WhatIf}");
                var id = await _emailTemplatesService.CreateEmailTemplate(cp);
                _renderer.WriteLine($"Email template: '{cp.Identifier}' created. Id={id}");
            }
            catch(Exception ex)
            {
                _renderer.WriteLine($"Email template creation: '{cp.Identifier}' failed. {ex.Message}");
            } 
        }

        private async Task DeleteEmailTemplateAsync(EmailTemplatesDTO cp)
        {
            if (Parameters.Delete)
            {
                _renderer.WriteLine($"Delete the email template: '{cp.Identifier}', Id: {cp.Id}.");
                if (Parameters.WhatIf) return;
                
                await _emailTemplatesService.DeleteEmailTemplateAsync(cp.Id.Value);
                _renderer.WriteLine($"Email template: '{cp.Identifier}' deleted.");
            }
        }
    }
}