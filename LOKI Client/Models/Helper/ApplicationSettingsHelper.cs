using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOKI_Client.Models.Helper
{
    public static class ApplicationSettingsHelper
    {
        /// <summary>
        /// Save a key-value pair into Application Settings.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <param name="value">The value to save.</param>
        public static void SaveSetting(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null)
            {
                config.AppSettings.Settings.Add(key, value);
            }
            else
            {
                config.AppSettings.Settings[key].Value = value;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Retrieve a value from Application Settings.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The value of the setting, or null if the key does not exist.</returns>
        public static string? GetSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key];
        }
    }
}
