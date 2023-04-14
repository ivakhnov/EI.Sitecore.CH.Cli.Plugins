using Sitecore.CH.Cli.Plugin.EmailTemplates.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.Services
{
    public interface IEmailTemplatesService
    {
        Task<List<EmailTemplatesDTO>> ReadEmailTemplatesAsync(FileInfo[] files);

        Task<List<EmailTemplatesDTO>> GetEmailTemplatesAsync();

        Task<long> CreateEmailTemplate(EmailTemplatesDTO emailTemplateEntity);

        Task<long> UpdateEmailTemplateAsync(EmailTemplatesDTO source, EmailTemplatesDTO target);

        Task DeleteEmailTemplateAsync(long entityId);

        List<EmailTemplatesDTO> CompareEmailTemplates(List<EmailTemplatesDTO> targetEmailTemplates, List<EmailTemplatesDTO> sourceEmailTemplates);
    }
}
