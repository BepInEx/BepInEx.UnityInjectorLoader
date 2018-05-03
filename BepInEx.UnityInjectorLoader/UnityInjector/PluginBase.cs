using System.IO;
using ExIni;
using UnityEngine;

namespace UnityInjector
{
    public class PluginBase : MonoBehaviour
    {
        private IniFile preferences;

        public string ConfigPath => Path.Combine(DataPath, $"{Name.Asciify()}.ini");
        public string DataPath => Extensions.UserDataPath;
        public string Name => GetType().Name;
        public IniFile Preferences => preferences ?? (preferences = ReloadConfig());


        protected IniFile ReloadConfig()
        {
            if (!File.Exists(ConfigPath))
                return preferences ?? new IniFile();

            IniFile ini = IniFile.FromFile(ConfigPath);

            if (preferences == null)
                preferences = ini;

            preferences.Merge(ini);

            return preferences;
        }

        protected void SaveConfig()
        {
            Preferences.Save(ConfigPath);
        }
    }
}