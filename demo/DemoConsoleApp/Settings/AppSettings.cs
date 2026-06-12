using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

public class AppSettings
{
    private static IConfigurationRoot _configuration;
    private static string _configFilePath;
    private static Dictionary<string, Dictionary<string, (string value, DateTime savedDateTime)>> _changedSections = new();
    private static Dictionary<string, HashSet<string>> _deletedSections = new();

    public class SettingEntry
    {
        public string Value { get; set; } = string.Empty;
        public DateTime SavedDateTime { get; set; }
    }

    public string ApplicationName { get; set; } = string.Empty;
    public int MaxItemsPerPage { get; set; }

    /// <summary>
    /// Static constructor that automatically initializes AppSettings with configuration from the application directory
    /// </summary>
    static AppSettings()
    {
        _configuration = Config.GetConfig();
        _configFilePath = Config.GetConfigFilePath();
        if (!File.Exists(_configFilePath))
        {
            throw new InvalidOperationException("Configuration file path is not set or file does not exist.");
        }
    }

    /// <summary>
    /// Retrieves a configuration section as a list of key-value pairs
    /// </summary>
    /// <param name="sectionName">The name of the section (e.g., "SavedItems")</param>
    /// <param name="subSectionName">Optional subsection name (e.g., "Methods")</param>
    /// <returns>List of key-value pairs from the section</returns>
    public static List<KeyValuePair<string, string>> GetSectionSetting(string sectionName, string? subSectionName = null)
    {

        var section = _configuration.GetSection(sectionName);
        
        if (!string.IsNullOrEmpty(subSectionName))
        {
            section = section.GetSection(subSectionName);
        }

        var result = new List<KeyValuePair<string, string>>();
        
        foreach (var child in section.GetChildren())
        {
            var value = GetSectionChildValue(child);
            result.Add(new KeyValuePair<string, string>(child.Key, value));
        }

        return result;
    }

    private static string GetSectionChildValue(IConfigurationSection section)
    {
        if (!string.IsNullOrEmpty(section.Value))
        {
            return ExtractValueFromEntry(section.Value);
        }

        var innerValueSection = section.GetSection("value");
        if (innerValueSection.Exists() && !string.IsNullOrEmpty(innerValueSection.Value))
        {
            return ExtractValueFromEntry(innerValueSection.Value);
        }

        return string.Empty;
    }

    /// <summary>
    /// Retrieves a configuration section as a list of SettingEntry objects (includes SavedDateTime), sorted by most recent first
    /// </summary>
    /// <param name="sectionName">The name of the section</param>
    /// <param name="subSectionName">Optional subsection name</param>
    /// <returns>List of SettingEntry objects with values and timestamps, sorted by SavedDateTime descending</returns>
    public static List<KeyValuePair<string, SettingEntry>> GetSectionSettingWithTimestamp(string sectionName, string? subSectionName = null)
    {
        var result = new List<KeyValuePair<string, SettingEntry>>();
        
        try
        {
            var jsonText = File.ReadAllText(_configFilePath);
            var jsonNode = JsonNode.Parse(jsonText);
            
            if (jsonNode == null)
            {
                throw new InvalidOperationException("Failed to parse appsettings.json file.");
            }

            // Navigate to the section
            var currentNode = jsonNode[sectionName];
            
            if (!string.IsNullOrEmpty(subSectionName))
            {
                currentNode = currentNode?[subSectionName];
            }

            if (currentNode is JsonObject sectionObj)
            {
                foreach (var kvp in sectionObj)
                {
                    if (kvp.Value is JsonObject entryObj)
                    {
                        var entry = ParseJsonObjectToSettingEntry(entryObj);
                        result.Add(new KeyValuePair<string, SettingEntry>(kvp.Key, entry));
                    }
                }
            }

            // Sort by SavedDateTime descending (most recent first)
            result = result.OrderByDescending(x => x.Value.SavedDateTime).ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error reading settings from file: {ex.Message}", ex);
        }

        return result;
    }

    /// <summary>
    /// Retrieves a configuration section as a dictionary
    /// </summary>
    /// <param name="sectionName">The name of the section</param>
    /// <param name="subSectionName">Optional subsection name</param>
    /// <returns>Dictionary of key-value pairs from the section</returns>
    public static Dictionary<string, string> GetSectionSettingAsDict(string sectionName, string? subSectionName = null)
    {
        return GetSectionSetting(sectionName, subSectionName).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Retrieves a configuration section as a dictionary with SettingEntry objects (includes SavedDateTime)
    /// </summary>
    /// <param name="sectionName">The name of the section</param>
    /// <param name="subSectionName">Optional subsection name</param>
    /// <returns>Dictionary with SettingEntry objects</returns>
    public static Dictionary<string, SettingEntry> GetSectionSettingAsDictWithTimestamp(string sectionName, string? subSectionName = null)
    {
        return GetSectionSettingWithTimestamp(sectionName, subSectionName).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Adds or updates a setting in a configuration section with a save timestamp
    /// </summary>
    /// <param name="sectionName">The name of the section</param>
    /// <param name="key">The key to add or update</param>
    /// <param name="value">The value to set</param>
    /// <param name="subSectionName">Optional subsection name</param>
    public static void AddOrUpdateSectionSetting(string sectionName, string key, string value, string? subSectionName = null)
    {
        var sectionKey = subSectionName != null ? $"{sectionName}:{subSectionName}" : sectionName;
        
        if (!_changedSections.ContainsKey(sectionKey))
        {
            _changedSections[sectionKey] = new Dictionary<string, (string, DateTime)>();
        }

        _changedSections[sectionKey][key] = (value, DateTime.UtcNow);
    }

    /// <summary>
    /// Deletes a setting from a configuration section
    /// </summary>
    /// <param name="sectionName">The name of the section</param>
    /// <param name="key">The key to delete</param>
    /// <param name="subSectionName">Optional subsection name</param>
    public static void DeleteSectionSetting(string sectionName, string key, string? subSectionName = null)
    {
        var sectionKey = subSectionName != null ? $"{sectionName}:{subSectionName}" : sectionName;
        
        // Record deletion so SaveChanges can remove it from the file
        if (!_deletedSections.ContainsKey(sectionKey))
        {
            _deletedSections[sectionKey] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
        _deletedSections[sectionKey].Add(key);

        // If there was a pending change for this key, remove it
        if (_changedSections.ContainsKey(sectionKey))
        {
            _changedSections[sectionKey].Remove(key);
        }
    }

    /// <summary>
    /// Saves all changes made via AddOrUpdateSectionSetting and DeleteSectionSetting to the appsettings.json file
    /// </summary>
    public static void SaveChanges()
    {
        var jsonText = File.ReadAllText(_configFilePath);
        var jsonNode = JsonNode.Parse(jsonText);

        if (jsonNode == null) throw new InvalidOperationException("Failed to parse appsettings.json file.");

        // Apply deletions first
        foreach (var sectionDeletes in _deletedSections)
        {
            var pathParts = sectionDeletes.Key.Split(':');
            JsonNode? currentNode = jsonNode;

            // Navigate to the section; if missing, nothing to delete
            foreach (var part in pathParts)
            {
                if (currentNode?[part] == null)
                {
                    currentNode = null;
                    break;
                }
                currentNode = currentNode[part];
            }

            if (currentNode is JsonObject sectionObj)
            {
                foreach (var key in sectionDeletes.Value)
                {
                    sectionObj.Remove(key);
                }
            }
        }

        // Apply all additions/updates
        foreach (var sectionChanges in _changedSections)
        {
            var pathParts = sectionChanges.Key.Split(':');
            JsonNode? currentNode = jsonNode;

            // Navigate to or create the section
            foreach (var part in pathParts)
            {
                if (currentNode?[part] == null)
                {
                    currentNode![part] = new JsonObject();
                }
                currentNode = currentNode[part];
            }

            // Update keys in the section with both value and timestamp
            var sectionObject = currentNode as JsonObject;
            if (sectionObject != null)
            {
                foreach (var kvp in sectionChanges.Value)
                {
                    var entryObject = new JsonObject
                    {
                        ["value"] = kvp.Value.value,
                        ["savedDateTime"] = kvp.Value.savedDateTime.ToString("O")
                    };
                    sectionObject[kvp.Key] = entryObject;
                }
            }
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = jsonNode.ToJsonString(options);
        File.WriteAllText(_configFilePath, jsonString);

        _changedSections.Clear();
        _deletedSections.Clear();
    }

    /// <summary>
    /// Helper method to parse a JsonObject into a SettingEntry (preserves savedDateTime from JSON)
    /// </summary>
    private static SettingEntry ParseJsonObjectToSettingEntry(JsonObject obj)
    {
        var value = obj["value"]?.GetValue<string>() ?? string.Empty;
        var savedDateTimeStr = obj["savedDateTime"]?.GetValue<string>();
        
        var savedDateTime = !string.IsNullOrEmpty(savedDateTimeStr) && DateTime.TryParse(savedDateTimeStr, out var dt) 
            ? dt 
            : DateTime.UtcNow;
        
        return new SettingEntry { Value = value, SavedDateTime = savedDateTime };
    }

    /// <summary>
    /// Helper method to extract just the value from a setting entry (handles both old string format and new object format)
    /// </summary>
    private static string ExtractValueFromEntry(string? jsonValue)
    {
        if (string.IsNullOrEmpty(jsonValue))
            return string.Empty;

        try
        {
            var node = JsonNode.Parse(jsonValue);
            if (node is JsonObject obj && obj.ContainsKey("value"))
            {
                return obj["value"]?.GetValue<string>() ?? string.Empty;
            }
        }
        catch
        {
            // If it's not JSON, treat as plain string value
        }

        return jsonValue;
    }

    /// <summary>
    /// Helper method to parse a setting entry into a SettingEntry object
    /// </summary>
    private static SettingEntry ParseSettingEntry(string? jsonValue)
    {
        if (string.IsNullOrEmpty(jsonValue))
            return new SettingEntry { Value = string.Empty, SavedDateTime = DateTime.UtcNow };

        try
        {
            var node = JsonNode.Parse(jsonValue);
            if (node is JsonObject obj && obj.ContainsKey("value"))
            {
                var value = obj["value"]?.GetValue<string>() ?? string.Empty;
                var savedDateTimeStr = obj["savedDateTime"]?.GetValue<string>();
                var savedDateTime = !string.IsNullOrEmpty(savedDateTimeStr) && DateTime.TryParse(savedDateTimeStr, out var dt) 
                    ? dt 
                    : DateTime.UtcNow;
                
                return new SettingEntry { Value = value, SavedDateTime = savedDateTime };
            }
        }
        catch
        {
            // If it's not JSON, treat as plain string value
        }

        return new SettingEntry { Value = jsonValue, SavedDateTime = DateTime.UtcNow };
    }
}