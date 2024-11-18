using System.ComponentModel;
using System.Windows;

namespace LOKI_Client.Dictionary
{
    public class LanguageManager
    {
        private static LanguageManager _instance;
        public static LanguageManager Instance => _instance ??= new LanguageManager();

        private ResourceDictionary _currentDictionary;

        public void SetLanguage(string languageCode)
        {
            string resourcePath = $"Dictionary/{languageCode}.xaml";
            var dictionary = new ResourceDictionary { Source = new Uri(resourcePath, UriKind.Relative) };

            // Remove the previous dictionary if any
            if (_currentDictionary != null)
                Application.Current.Resources.MergedDictionaries.Remove(_currentDictionary);

            // Set and add the new dictionary
            _currentDictionary = dictionary;
            Application.Current.Resources.MergedDictionaries.Add(_currentDictionary);
        }
    }

}
