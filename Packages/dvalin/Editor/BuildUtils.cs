using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SettingsManagement;

namespace Dvalin.Editor
{
    public class BuildUtils
    {
        /// <summary>
        /// Absolute path to ProjectRoot/Dist.
        /// </summary>
        public static readonly string k_DistDir = Path.Combine(Application.dataPath, "..", "Dist");

        private static readonly string[] k_IgnoredAssemblies = {
            "UnityEngine.TestRunner",
            "UnityEngine.UI",
            "Unity.TextMeshPro",
            "Unity.Timeline"
        };

        [UserSettingAttribute("User Settings", "Copy Dist to Plugins", "Copy build output to BepInEx plugins directory.")]
        public static UserSetting<bool> m_CopyDistToPlugins = new UserSetting<bool>(
            DvalinSettings.instance, "CopyDistToPlugins", false, SettingsScope.User
        );

        [UserSettingAttribute("User Settings", "Plugins Path", "Path to BepInEx plugins directory.")]
        public static UserSetting<string> m_BepInExPluginsDir = new UserSetting<string>(
            DvalinSettings.instance, "BepInExPluginsDir", "", SettingsScope.User
        );

        /// <summary>
        /// Deletes <see cref="k_DistDir" /> and runs: 
        /// <see cref="BuildRuntimeAssemblies" />,
        /// <see cref="BuildAssetBundles" />,
        /// <see cref="CreateManifest" />,
        /// <see cref="CopyDistToPlugins" /> (optional)
        /// </summary>
        [MenuItem("Dvalin/Build All")]
        public static void BuildAll()
        {
            if (Directory.Exists(k_DistDir))
            {
                Directory.Delete(k_DistDir, true);
            }

            BuildRuntimeAssemblies();
            BuildAssetBundles();

            CreateManifest();

            if (m_CopyDistToPlugins)
            {
                CopyDistToPlugins();
            }
        }

        /// <summary>
        /// Builds the required dlls and puts them in <see cref="k_DistDir" />. 
        /// </summary>
        // [MenuItem("Dvalin/Build Runtime Assemblies")]
        public static void BuildRuntimeAssemblies()
        {
            Assembly[] playerAssemblies = CompilationPipeline
                .GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies)
                .Where(x => !k_IgnoredAssemblies.Contains(x.name))
                .ToArray();

            string stageDirName = "Build";
            string projectRootDir = Path.Combine(Application.dataPath, "..");
            string stagePath = Path.Combine(projectRootDir, stageDirName);

            var options = new BuildPlayerOptions();
            options.locationPathName = Path.Combine(stagePath, "Dvalin.exe");
            options.options = BuildOptions.BuildScriptsOnly;
            options.target = EditorUserBuildSettings.activeBuildTarget;
            BuildPipeline.BuildPlayer(options);

            string distDir = CreateDistDirectory();
            foreach (var assembly in playerAssemblies)
            {
                File.Copy(
                    Path.Combine(stagePath, "Dvalin_Data", "Managed", assembly.name + ".dll"),
                    Path.Combine(distDir, assembly.name + ".dll"),
                    true
                );
            }
        }

        /// <summary>
        /// Builds asset bundles and puts them in <see cref="k_DistDir" />.
        /// </summary>
        // [MenuItem("Dvalin/Build Asset Bundles")]
        public static void BuildAssetBundles()
        {
            string stageDirName = "AssetBundles";
            string projectRootDir = Path.Combine(Application.dataPath, "..");
            string stagePath = Path.Combine(projectRootDir, stageDirName);

            BuildPipeline.BuildAssetBundles(stagePath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            string distDir = CreateDistDirectory();
            foreach (var file in Directory.EnumerateFiles(stagePath))
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(distDir, fileName), true);
            }
        }

        public static void CreateManifest()
        {
            string distDir = CreateDistDirectory();
            var manifest = ThunderStoreManifest.FromPlugin();

            var json = EditorJsonUtility.ToJson(manifest, true);
            using (var stream = File.OpenWrite(Path.Combine(distDir, "manifest.json")))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(json);
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="k_DistDir" /> folder if it doesn't already exist.
        /// </summary>
        /// <returns><see cref="k_DistDir" /></returns>
        public static string CreateDistDirectory()
        {
            Directory.CreateDirectory(k_DistDir);
            return k_DistDir;
        }

        /// <summary>
        /// Copies everything in the <see cref="k_DistDir" /> folder to the BepInEx/plugin folder.
        /// </summary>
        public static void CopyDistToPlugins()
        {
            string pluginDir = Path.Combine(m_BepInExPluginsDir, "Dvalin");
            if (Directory.Exists(pluginDir))
            {
                Directory.Delete(pluginDir, true);
            }
            Directory.CreateDirectory(pluginDir);

            foreach (var file in Directory.EnumerateFiles(k_DistDir))
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(pluginDir, fileName), true);
            }
        }
    }

    [System.Serializable]
    public class ThunderStoreManifest
    {
        public string name = "Unknown";
        public string version_number = "0.0.0";
        public string website_url = "";
        public string description = "";
        public string[] dependencies = new string[0];

        public int ManifestVersion = 2;
        public string AuthorName = "Unknown";
        public string Name = "Unknown";
        public string DisplayName = "Unknown";
        public string Version = "0.0.0";
        public string Licence = "";
        public string WebsiteURL = "";
        public string Description = "";
        public string GameVersion = "0.0.0";
        public string[] Dependencies = { };
        public string[] OptionalDependencies = { };
        public string[] Incompatibilities = { };
        public string NetworkMode = "both";
        public string PackageType = "mod";
        public string InstallMode = "managed";
        public string[] Loaders = { "bepinex" };
        public ManifestExtraData ExtraData = new ManifestExtraData();

        public static ThunderStoreManifest FromPlugin()
        {
            var result = new ThunderStoreManifest();
            result.name = Plugin.k_Name.Replace(' ', '_');
            result.version_number = result.Version = Plugin.k_Version;
            result.website_url = result.WebsiteURL = Plugin.k_WebsiteUrl;
            result.description = result.Description = Plugin.k_Description;
            result.dependencies = result.Dependencies = Plugin.k_Dependencies;
            result.AuthorName = Plugin.k_Author;
            result.Name = string.Format("{0}-{1}", Plugin.k_Author, Plugin.k_Name.Replace(' ', '_'));
            result.DisplayName = Plugin.k_Name;
            return result;
        }

        [System.Serializable]
        public struct ManifestExtraData { }
    }
}