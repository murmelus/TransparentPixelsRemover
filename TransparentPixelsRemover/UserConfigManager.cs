using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace TransparentPixelsRemover
{
    public class UserConfigManager
    {
        Dictionary<string, string> configDictionary;
        string configName;

        public UserConfigManager(string configName) {
            this.configName = configName;
            configDictionary = new Dictionary<string, string>();
            TryLoadConfig();
        }

        public bool TrySetValue(string key, string value)
        {
            try
            {
                configDictionary[key] = value;
                return TrySaveConfig();
            }
            catch
            {
                return false;
            }
        }

        public bool TryLoadConfig()
        {
            try
            {
                if (File.Exists(configName))
                {
                    string json = File.ReadAllText(configName);
                    configDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TrySaveConfig()
        {
            try
            {
                string json = JsonSerializer.Serialize(configDictionary, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configName, json);
                return true;
            }
            catch
            {
                MessageBox.Show(Application.Current.Resources["Text_UnableToSaveUserConfig"] as string);
                return false;
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            try
            {
                if (configDictionary.TryGetValue(key, out value))
                {
                    return true;
                }

                value = null;
                return false;
            }
            catch
            {
                value = null;
                return false;
            }
        }

    }
}
