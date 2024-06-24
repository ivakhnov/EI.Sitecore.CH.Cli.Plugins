using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Models;

namespace Sitecore.CH.Cli.Plugin.ImportPackageSettingApplier.Services
{
    public class SetActionReqAttributesService : ISetActionReqAttributesService
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        public SetActionReqAttributesService()
        {
        }


        public async Task<SettingDTO> GetSettingAsync(FileInfo settingFile)
        {
            using var fileStream = settingFile.OpenText();

            var jsonString = await fileStream.ReadToEndAsync();
            var setting = new SettingDTO(jsonString);

            return setting;
        }

        public JToken GetActionFileAsToken(string jsonFilePath)
        {
            JToken token = null;
            using (StreamReader file = File.OpenText(jsonFilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                token = JToken.ReadFrom(reader);
            }
            return token;
        }
        

        public void UpdateTokenBySetting(JToken actionToken, ActionRequestAttributes requestAttributes)
        { 
            if (!string.IsNullOrEmpty(requestAttributes.ActionHeaders))
            {
                UpdateProperties(actionToken, "$.data.properties.Settings.headers", requestAttributes.ActionHeaders);
            }

            if (!string.IsNullOrEmpty(requestAttributes.ActionValues))
            {
                UpdateProperties(actionToken, "$.data.properties.Settings.values", requestAttributes.ActionValues);
            }
        }

        public void SaveActionFileFromToken(JToken token, string jsonFilePath)
        {
            if (token != null)
            {
                using (StreamWriter writer = new StreamWriter(jsonFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, token);
                }
            }
        }
        
        private void UpdateProperties(JToken mainToken, string jsonPath, string value)
        {
            var tokens = mainToken.SelectTokens(jsonPath);
            if (tokens != null && tokens.Any())
            {
                foreach (var token in tokens)
                {

                    var jProperty = token.Parent as JProperty;

                    if (jProperty == null)
                        throw new NotImplementedException($"Expected JProperty but found something else");

                    jProperty.Value = JToken.Parse(value);
                }
            }
        }
    }
}
