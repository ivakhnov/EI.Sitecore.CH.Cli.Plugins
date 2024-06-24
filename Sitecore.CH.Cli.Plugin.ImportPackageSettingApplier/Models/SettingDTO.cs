using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Stylelabs.M.Sdk.Contracts.Base;
using System.Linq;
using Stylelabs.M.Sdk.Exceptions;
using Newtonsoft.Json;

namespace Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Models
{
    // Could be any setting, but in the implementation below we start with a custom
    // setting which is an instance of https://{host}/api/entitydefinitions/M.Setting
    // The below DTO's are just helping to read the values from a predefined structure of data
    public class SettingDTO
    {
        public static string DefinitionName = "M.Setting";
        private readonly IEntity _entity;

        public long? Id { get; set; }
        public string Identifier { get; set; }

        [JsonProperty("M.Setting.Name")]
        public string MSettingName { get; set; }

        [JsonProperty("M.Setting.Value")]
        public MSettingValue MSettingValue { get; set; }

        [JsonProperty("M.Setting.EnvironmentSpecific")]
        public bool MSettingEnvironmentSpecific { get; set; }
        
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }

        public SettingDTO()
        { }

        public SettingDTO(string serializedSettingString)
        {
            var settingToken = JToken.Parse(serializedSettingString);

            // This gives us id and identifier
            var settingEntity = settingToken.SelectToken("$.data").ToObject<SettingDTO>();
            
            // but the actual setting values are in "properties" section
            var settingProperties = settingToken.SelectToken("$.data.properties").ToObject<SettingDTO>();

            // and as a setting is actually also a setting, we combine both
            Id = settingEntity.Id;
            Identifier = settingEntity.Identifier;

            MSettingName = settingProperties.MSettingName;
            MSettingValue = settingProperties.MSettingValue;
            MSettingEnvironmentSpecific = settingProperties.MSettingEnvironmentSpecific;

            ModifiedBy = settingEntity.ModifiedBy;
            ModifiedOn = settingEntity.ModifiedOn;
        }

        public SettingDTO(IEntity entity)
        {
            _entity = entity;

            Id = _entity.Id;
            Identifier = _entity.Identifier;

            MSettingName = _entity.GetPropertyValue<string>("M.Setting.Name");
            MSettingValue = new MSettingValue(_entity.GetPropertyValue<JToken>("M.Setting.Value"));

            MSettingEnvironmentSpecific = _entity.GetPropertyValue<bool>("M.Setting.EnvironmentSpecific");

            ModifiedBy = _entity.ModifiedBy;
            ModifiedOn = _entity.ModifiedOn;
            
        }
    }

    public class MSettingValue
    {
        // In fact, this class should also contains a dictionary list of urls, but don't need and/or don't use thos in this 
        // CLI plugin, they are used inside ContentHub itself, by actions that piont to the url values in the setting by means of templatized approach.
        // Please, read ContentHub docs to understand how actions can have their urls templatized and point to a setting. 
        
        public List<ActionRequestAttributes> ActionRequestAttributes { get; set; }

        public MSettingValue(JToken settingValue)
        {
            if (settingValue == null)
                return;

            ActionRequestAttributes = settingValue["ActionRequestAttributes"].ToObject<List<ActionRequestAttributes>>();
        }
    }
    public class ActionRequestAttributes
    {
        public string ActionIdentifier { get; set; }
        public string ActionHeaders { get; set; }
        public string ActionValues { get; set; }
    }
}
