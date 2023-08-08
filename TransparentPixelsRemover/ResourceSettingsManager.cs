using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Resources;

namespace TransparentPixelsRemover
{
    public class ResourceSettingsManager
    {
        Dictionary<string, string> settingsDictionary;
        string typeOfSettings;
        public ResourceSettingsManager(string filePath, string typeOfSettings)
        {
            this.typeOfSettings = typeOfSettings;

            settingsDictionary = new Dictionary<string, string>();

            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri(filePath, UriKind.Relative));

            if (resourceInfo is not null)
            {
                // Чтение данных из ресурса
                using (StreamReader reader = new StreamReader(resourceInfo.Stream))
                {
                    string json = reader.ReadToEnd();
                    settingsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            }
            else
            {
                MessageBox.Show($"{Application.Current.Resources["Text_ResourcesFileError"] as string} - '{filePath}'.");
            }

        }
        public List<string> getSettingsNames()
        {
            List<string> languagesNames = new List<string>();

            languagesNames.AddRange(settingsDictionary.Keys);

            return languagesNames;
        }
        public void setResource(string name)
        {
            if (settingsDictionary.ContainsKey(name))
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();

                foreach (ResourceDictionary resource in Application.Current.Resources.MergedDictionaries)
                {
                    if (resource.Contains(typeOfSettings))
                    {
                        resourceDictionary = resource;
                        break;
                    }
                }

                string langFilePath = settingsDictionary[name];                

                resourceDictionary.Source = new Uri(langFilePath, UriKind.RelativeOrAbsolute);

            }
            else
            {
                MessageBox.Show(Application.Current.Resources["Text_ChoosenResource"] as string);
            }
        }
    }
}
