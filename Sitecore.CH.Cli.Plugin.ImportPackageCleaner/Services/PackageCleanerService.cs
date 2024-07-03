using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Sitecore.CH.Cli.Plugin.ImportPackageCleaner.Services
{
    public class PackageCleanerService : IPackageCleanerService
    {
        public HashSet<string> TypesOfComponentsFoundOnRelated { get; }

        public class Constants
        {
            public class Definition
            {
                public const string Page = "Portal.Page";
                public const string Action = "M.Action";
            }
        }

        private Dictionary<string, string> _pathForDefinitions = new Dictionary<string, string>()
        {
            [Constants.Definition.Page] = "data.entity.entitydefinition.href",
            [Constants.Definition.Action] = "data.entitydefinition.href",
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        public PackageCleanerService()
        {
            TypesOfComponentsFoundOnRelated = new HashSet<string>();
        }

        public JToken GetToken(string jsonFilePath)
        {
            JToken token = null;
            using (StreamReader file = File.OpenText(jsonFilePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                token = JToken.ReadFrom(reader);
            }
            return token;
        }

        public void UpdateToken(JToken token, bool shouldCleanPortalComponents, bool shouldCleanActionVariables)
        {
            var propertiesToLookList = new Dictionary<string, object>()
            {
                ["id"] = 0,
                ["modified_on"] = DateTime.MinValue,
                ["modified_by"] = null,
                ["created_on"] = DateTime.MinValue,
                ["created_by"] = null,
                ["version"] = 0
            };


            var prefixesList = new Dictionary<string, List<string>>
            {
                ["$.data"] = propertiesToLookList.Keys.ToList(),
                ["$.data.entity"] = propertiesToLookList.Keys.ToList(),
                ["$.data.related[*]"] = propertiesToLookList.Keys.ToList(),
                ["$.data"] = propertiesToLookList.Keys.ToList(),
                ["$.data.restricted[*]"] = propertiesToLookList.Keys.Except(new[] { "id" }).ToList()
            };

            if (token == null)
                return;

            foreach (var prefix in prefixesList)
            {
                propertiesToLookList.Where(u => prefix.Value.Contains(u.Key)).ToList().ForEach(property => UpdateProperties(token, $"{prefix.Key}.{property.Key}", property.Value));
            }

            if (shouldCleanPortalComponents)
                CleanPortalPageComponents(token);

            if (shouldCleanActionVariables)
                CleanActionVariables(token);

            CleanUserGroupIdentifiers(token);

            SortCultures(token);
        }

        public void SaveFile(JToken token, string jsonFilePath)
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

        
        private void UpdateProperties(JToken mainToken, string jsonPath, object value)
        {
            var tokens = mainToken.SelectTokens(jsonPath);
            if (tokens != null && tokens.Any())
            {
                foreach (var token in tokens)
                {

                    var jProperty = token.Parent as JProperty;

                    if (jProperty == null)
                        throw new NotImplementedException($"Expected JProperty but found something else");

                    jProperty.Value = new JValue(value);
                }

            }
        }

        private void CleanPortalPageComponents(JToken token)
        {
            if (TokenIsOfDefinition(token, Constants.Definition.Page))
            {
                var relatedTokens = token.SelectTokens("$..related.[*].entitydefinition.href");
                if (relatedTokens != null)
                {
                    foreach (var item in relatedTokens.ToList())
                    {
                        var value = item.Value<string>();
                        TypesOfComponentsFoundOnRelated.Add(value);
                        if (value != "Portal.PageComponent")
                            Remove(item);
                    }
                }

                var related = token.SelectToken("$.data.related") as JArray;
                if (related != null)
                {
                    var tuple = related.Select(elem => (elem, elem.ToString())).ToList();

                    var tokens = tuple.OrderBy(elem => elem.Item2).Select(elem => elem.elem).ToList();
                    related.Clear();
                    tokens.ForEach(related.Add);
                }
            }
        }

        private void CleanActionVariables(JToken token)
        {
            if (TokenIsOfDefinition(token, Constants.Definition.Action))
            {
                var aPIUrlToken = token.SelectToken("data.properties.Settings.apiUrl") as JValue;
                if (aPIUrlToken != null)
                    aPIUrlToken.Value = string.Empty;

                var headersToken = token.SelectToken("data.properties.Settings.headers") as JArray;
                if (headersToken != null)
                    headersToken.Clear();

                var serviceBusConnectionString = token.SelectToken("data.properties.Settings.connectionString") as JValue;
                if (serviceBusConnectionString != null)
                    serviceBusConnectionString.Value = string.Empty;
            }
        }

        private void SortCultures(JToken token)
        {
            SortKeyValues(token, "$.data.cultures");
            SortKeyValues(token, "$.data..labels");
            SortKeyValues(token, "$.data..associated_labels");
        }

        private void SortKeyValues(JToken token, string path)
        {
            var arrayOfTokenValues = token.SelectTokens(path);
            if (arrayOfTokenValues != null && arrayOfTokenValues.Any())
            {
                foreach (var values in arrayOfTokenValues)
                {
                    // it's either an array or an object..
                    var JArrayValue = values as JArray;
                    if (JArrayValue != null)
                    {
                        var tuple = JArrayValue.Select(elem => (elem, elem.ToString())).ToList();

                        var tokens = tuple.OrderBy(elem => elem.Item2).Select(elem => elem.elem).ToList();
                        JArrayValue.Clear();
                        tokens.ForEach(JArrayValue.Add);
                    }
                    else
                    // Although the JSON spec defines a JSON object as an unordered set of properties,
                    // Json.Net's JObject class does appear to maintain the order of properties within it.
                    // You can sort the properties by value like this:
                    {
                        JObject voteObj = values as JObject;

                        var sortedObj = new JObject(
                            voteObj.Properties().OrderBy(elem => (string)elem.Name)
                        );

                        values.Replace(sortedObj);
                    }
                }
            }
        }

        private void CleanUserGroupIdentifiers(JToken token)
        {
            var pathsToIdentifyUserGroups = new[] {
                            "$.data.rules",
                            "$.data.usergroup"
            };

            var isUserGroup = true;

            foreach (var path in pathsToIdentifyUserGroups)
            {
                if (token.SelectToken(path) == null)
                {
                    isUserGroup = false;
                    break;
                }
            }

            if (isUserGroup)
            {
                UpdateProperties(token, "$..identifier", Guid.Empty);
                RemoveBuiltInRules(token);
                SortRules(token);
            }

        }

        private void RemoveBuiltInRules(JToken token)
        {
            var tokensToRemove = new List<JToken>();
            foreach (var builtinRule in token.SelectTokens("$.data.rules..[?(@.type == 'BuiltIn')]"))
            {
                tokensToRemove.Add(builtinRule);
            }
            tokensToRemove.ForEach(t => t.Remove());
        }

        private void SortRules(JToken token)
        {
            SortKeyValues(token, "$.data.rules");
        }

        private void Remove(JToken item)
        {
            if (item.Parent is JArray)
            {
                var myObject = item as JObject;
                if (myObject != null && myObject["is_system_owned"].Value<bool>())
                    item.Remove();
            }
            else
                Remove(item.Parent);
        }

        private bool TokenIsOfDefinition(JToken token, string definitionName)
        {
            var result = token.SelectToken(_pathForDefinitions[definitionName]);
            return result != null && result.Value<string>() == definitionName;
        }
    }
}
