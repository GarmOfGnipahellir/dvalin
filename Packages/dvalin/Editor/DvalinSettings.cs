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
                    s_Instance = new UnityEditor.SettingsManagement.Settings("dvalin");

                return s_Instance;
            }
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
                new[] { typeof(DvalinSettings).Assembly });
            return provider;
        }
    }
}