using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Stylelabs.M.Sdk.Contracts.Base;
using System.Linq;
using Stylelabs.M.Sdk.Exceptions;

namespace Sitecore.CH.Cli.Plugin.EmailTemplates.Models
{
    //https://{host}/api/entitydefinitions/M.Mailing.Template
    public class EmailTemplatesDTO
    {
        public static string DefinitionName = "M.Mailing.Template";
        private readonly IEntity _entity;

        public EmailTemplatesDTO()
        {
        }

        public IReadOnlyList<CultureInfo> Cultures { get; set; }
        public long? Id { get; set; }
        public string Identifier { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<CultureInfo, string> TemplateLabel { get; set; }
        public Dictionary<CultureInfo, string> TemplateDescription { get; set; }
        public JArray TemplateVariables { get; set; }
        public Dictionary<CultureInfo, string> Subject { get; set; }
        public Dictionary<CultureInfo, string> Body { get; set; }
        
        
        
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }

        public EmailTemplatesDTO(IEntity entity)
        {
            _entity = entity;

            Cultures = _entity.Cultures;
            Id = _entity.Id;
            Identifier = _entity.Identifier;

            // First the attributes with: "is_multilanguage": false
            TemplateName = _entity.GetPropertyValue<string>("M.Mailing.TemplateName");
            TemplateVariables = _entity.GetPropertyValue<JArray>("M.Mailing.TemplateVariables");

            // And then the attributes with: "is_multilanguage": true
            TemplateLabel = new Dictionary<CultureInfo, string>() { };
            TemplateDescription = new Dictionary<CultureInfo, string>() { };
            Subject = new Dictionary<CultureInfo, string>() { };
            Body = new Dictionary<CultureInfo, string>() { };

            foreach (var culture in Cultures)
            {
                TemplateLabel[culture] = _entity.GetPropertyValue<string>("M.Mailing.TemplateLabel", culture);
                TemplateDescription[culture] = _entity.GetPropertyValue<string>("M.Mailing.TemplateDescription", culture);
                Subject[culture] = _entity.GetPropertyValue<string>("M.Mailing.Subject", culture);
                Body[culture] = _entity.GetPropertyValue<string>("M.Mailing.Body", culture);
            }

            ModifiedBy = _entity.ModifiedBy;
            ModifiedOn = _entity.ModifiedOn;
            
        }

        public IEntity Update()
        {
            // First the attributes with: "is_multilanguage": false
            _entity.SetPropertyValue("M.Mailing.TemplateName", TemplateName);
            _entity.SetPropertyValue("M.Mailing.TemplateVariables", TemplateVariables);

            // And then the attributes with: "is_multilanguage": true
            foreach (var culture in Cultures)
            {
                _entity.SetPropertyValue("M.Mailing.TemplateLabel", culture, TemplateLabel[culture]);
                _entity.SetPropertyValue("M.Mailing.TemplateDescription", culture, TemplateDescription[culture]);
                _entity.SetPropertyValue("M.Mailing.Subject", culture, Subject[culture]);
                _entity.SetPropertyValue("M.Mailing.Body", culture, Body[culture]);
            }
            
            return _entity;
        }
    }


    public class IdentifierComparer : IEqualityComparer<EmailTemplatesDTO>
    {

        public bool Equals([AllowNull] EmailTemplatesDTO x, [AllowNull] EmailTemplatesDTO y)
        {
            return x.Identifier.Equals(y.Identifier);
        }

        public int GetHashCode([DisallowNull] EmailTemplatesDTO obj)
        {
            return obj.Identifier.GetHashCode();
        }
    }


    public class EmailTemplatesComparer : IEqualityComparer<EmailTemplatesDTO>
    {
        public bool Equals([AllowNull] EmailTemplatesDTO x, [AllowNull] EmailTemplatesDTO y)
        {
            return x.Identifier.Equals(y.Identifier)
                && x.TemplateName.Equals(y.TemplateName)
                && DictionariesAreEqual(x.TemplateLabel, y.TemplateLabel)
                && DictionariesAreEqual(x.TemplateDescription, y.TemplateDescription)
                && JToken.DeepEquals(x.TemplateVariables, y.TemplateVariables)
                && DictionariesAreEqual(x.Subject, y.Subject)
                && DictionariesAreEqual(x.Body, y.Body);
        }

        public int GetHashCode([DisallowNull] EmailTemplatesDTO obj)
        {
            var hashCode = $"{obj.Identifier}{obj.Body}";
            return hashCode.GetHashCode();
        }

        private bool DictionariesAreEqual(Dictionary<CultureInfo, string> x, Dictionary<CultureInfo, string> y)
        {
            // We specifically assume that the number of keys must be the same,
            // otherwise it would mean that environment does not have same amount of cultures than our export
            var culturesAreEqual = x.Count == y.Count &&
                    (x.Keys.All(key => y.Keys.Contains(key)) &&
                     y.Keys.All(key => x.Keys.Contains(key)));

            if (!culturesAreEqual)
                throw new ValidationException("The set of installed cultures is not matching!");
            
            var nonMatchingCulturedValues = y.Where(entry => x[entry.Key] != entry.Value).ToDictionary(entry => entry.Key, entry => entry.Value);

            return !nonMatchingCulturedValues.Keys.Any();
        }
    }
}
