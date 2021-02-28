using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dvalin.Editor
{
    public class BuildUtils
    {
        /// <summary>
        /// Absolute path to ProjectRoot/Dist.
        /// </summary>
        public static readonly string DIST_DIR = Path.Combine(Application.dataPath, "..", "Dist");

        /// <summary>
        /// Deletes <see cref="DIST_DIR" /> and runs: 
        /// <see cref="BuildRuntimeAssembly" />,
        /// <see cref="BuildAssetBundles" />
        /// </summary>
        [MenuItem("Dvalin/Build All")]
        public static void BuildAll()
        {
            Directory.Delete(DIST_DIR, true);

            BuildRuntimeAssembly();
            BuildAssetBundles();

            CreateManifest();

            CopyDistToPlugins();
        }

        /// <summary>
        /// Builds the plugin dll and puts it in <see cref="DIST_DIR" />. 
        /// </summary>
        [MenuItem("Dvalin/Build Runtime Assembly")]
        public static void BuildRuntimeAssembly()
        {
            string stageDirName = "Build";
            string projectRootDir = Path.Combine(Application.dataPath, "..");
            string stagePath = Path.Combine(projectRootDir, stageDirName);

            var options = new BuildPlayerOptions();
            options.locationPathName = Path.Combine(stagePath, "Dvalin.exe");
            options.options = BuildOptions.BuildScriptsOnly;
            options.target = EditorUserBuildSettings.activeBuildTarget;
            BuildPipeline.BuildPlayer(options);

            string distDir = CreateDistDirectory();
            File.Copy(
                Path.Combine(stagePath, "Dvalin_Data", "Managed", "Dvalin.dll"),
                Path.Combine(distDir, "Dvalin.dll")
            );
        }

        /// <summary>
        /// Builds asset bundles and puts them in <see cref="DIST_DIR" />.
        /// </summary>
        [MenuItem("Dvalin/Build Asset Bundles")]
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
        /// Creates the <see cref="DIST_DIR" /> folder if it doesn't already exist.
        /// </summary>
        /// <returns><see cref="DIST_DIR" /></returns>
        public static string CreateDistDirectory()
        {
            Directory.CreateDirectory(DIST_DIR);
            return DIST_DIR;
        }

        /// <summary>
        /// Copies everything in the <see cref="DIST_DIR" /> folder to the BepInEx/plugin folder.
        /// </summary>
        public static void CopyDistToPlugins()
        {
            // TODO implement dir copy
            foreach (var file in Directory.EnumerateFiles(DIST_DIR))
            {
                string fileName = Path.GetFileName(file);
                File.Copy(file, Path.Combine(@"D:\Programs\r2modman\data\Valheim\profiles\Dvalin Test\BepInEx\plugins\Dvalin", fileName), true);
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
            result.name = Plugin.NAME.Replace(' ', '_');
            result.version_number = result.Version = Plugin.VERSION;
            result.website_url = result.WebsiteURL = Plugin.WEBSITE_URL;
            result.description = result.Description = Plugin.DESCRIPTION;
            result.dependencies = result.Dependencies = Plugin.DEPENDENCIES;
            result.AuthorName = Plugin.AUTHOR;
            result.Name = string.Format("{0}-{1}", Plugin.AUTHOR, Plugin.NAME.Replace(' ', '_'));
            result.DisplayName = Plugin.NAME;
            return result;
        }

        [System.Serializable]
        public struct ManifestExtraData { }
    }
}