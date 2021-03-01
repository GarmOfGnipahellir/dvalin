using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SettingsManagement;

namespace Dvalin.Editor
{
    public class DvalinSettings
    {
        static UnityEditor.SettingsManagement.Settings s_Instance;

        internal static UnityEditor.SettingsManagement.Settings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new UnityEditor.SettingsManagement.Settings("dvalin");

                    // it's nice to have these settings synced
                    s_Instance.afterSettingsSaved += () => {
                        PlayerSettings.companyName = ModInfo.author;
                        PlayerSettings.productName = ModInfo.name;
                        PlayerSettings.bundleVersion = ModInfo.version;
                    };
                }

                return s_Instance;
            }
        }

        public static UserSetting<T> UserSetting<T>(string key, T value)
        {
            return new UserSetting<T>(instance, key, value, SettingsScope.User);
        }

        public static UserSetting<T> ProjectSetting<T>(string key, T value)
        {
            return new UserSetting<T>(instance, key, value, SettingsScope.Project);
        }
    }

    static class DvalinSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateDvalinSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new UserSettingsProvider("Preferences/Dvalin",
                DvalinSettings.instance,
                new[] { typeof(DvalinSettings).Assembly }
            );
            return provider;
        }
    }
}