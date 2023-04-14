using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.CH.Cli.Plugin.EmailTemplates.Models;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Framework.Essentials.LoadOptions;
using Stylelabs.M.Sdk.WebClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.Services
{
    public class EmailTemplatesService : IEmailTemplatesService
    {
        private readonly Lazy<IWebMClient> _client;

        public EmailTemplatesService(Lazy<IWebMClient> client)
        {
            _client = client;
        }

        public List<EmailTemplatesDTO> CompareEmailTemplates(List<EmailTemplatesDTO> targetEmailTemplates, List<EmailTemplatesDTO> sourceEmailTemplates)
        {
            var identifierComparer = new IdentifierComparer();
            var emailTemplatesComparer = new EmailTemplatesComparer();
            return (from target in targetEmailTemplates
                    let element = sourceEmailTemplates.SingleOrDefault(s => 
                                        identifierComparer.Equals(s, target) && 
                                        !emailTemplatesComparer.Equals(s, target))
                    where element != null
                    select element).ToList();
        }

        public async Task<long> CreateEmailTemplate(EmailTemplatesDTO emailTemplateEntity)
        {
            var entityDefinition = await _client.Value.EntityDefinitions.GetCachedAsync(EmailTemplatesDTO.DefinitionName);

            var entity = await _client.Value.EntityFactory.CreateAsync(entityDefinition, new CultureLoadOption(emailTemplateEntity.Cultures));
            var target = new EmailTemplatesDTO(entity);

            target.Identifier = emailTemplateEntity.Identifier;

            return await UpdateEmailTemplateAsync(emailTemplateEntity, target);
        }

        public async Task<long> UpdateEmailTemplateAsync(EmailTemplatesDTO source, EmailTemplatesDTO target)
        {
            target.TemplateName = source.TemplateName;
            target.TemplateLabel = source.TemplateLabel;
            target.TemplateDescription = source.TemplateDescription;
            target.TemplateVariables = source.TemplateVariables;
            target.Subject = source.Subject;
            target.Body = source.Body;

            return await _client.Value.Entities.SaveAsync(target.Update());
        }

        public async Task DeleteEmailTemplateAsync(long entityId)
        {
            await _client.Value.Entities.DeleteAsync(entityId);
        }

        /// <summary>
        /// Gets email templates from Content Hub
        /// </summary>
        /// <returns>List of email templates returned by Content Hub</returns>
        public async Task<List<EmailTemplatesDTO>> GetEmailTemplatesAsync()
        {
            // For now we dicide that we will not do anything with relations for email templates,
            // in this plugin we are only interested in the properties of the templates
            var loadConfig = new EntityLoadConfiguration
            {
                RelationLoadOption = RelationLoadOption.None,
                PropertyLoadOption = PropertyLoadOption.All,
                CultureLoadOption = CultureLoadOption.All
            };

            var iterator = _client.Value.Entities.GetEntityIterator(EmailTemplatesDTO.DefinitionName, loadConfig);

            var emailTemplates = new List<EmailTemplatesDTO>();

            while (iterator.CanMoveNext())
            {
                await iterator.MoveNextAsync();
                var entities = iterator.Current.Items.Select(e => new EmailTemplatesDTO(e)).ToList();
                emailTemplates.AddRange(entities);
            }

            return emailTemplates;
        }

        /// <summary>
        /// Reads email templates from json files.
        /// </summary>
        /// <param name="files"></param>
        /// <returns>List of email templates</returns>
        public async Task<List<EmailTemplatesDTO>> ReadEmailTemplatesAsync(FileInfo[] files)
        {
            var emailTemplates = new List<EmailTemplatesDTO>();

            foreach (var file in files)
            {
                using var fileStream = file.OpenText();

                var json = await fileStream.ReadToEndAsync();
                emailTemplates.Add(JsonConvert.DeserializeObject<EmailTemplatesDTO>(json));
            }

            return emailTemplates;
        }
    }
}
