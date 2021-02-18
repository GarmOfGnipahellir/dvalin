using System.IO;
using UnityEngine;
using UnityEditor;

public class DvalinEditorUtils
{
    [MenuItem("Dvalin/Build Asset Bundles")]
    static void BuildAssetBundles()
    {
        string stageDirName = "AssetBundles";
        string projectRootDir = Path.Combine(Application.dataPath, "..");
        string stagePath = Path.Combine(projectRootDir, stageDirName);
        string distDir = Path.Combine(projectRootDir, @"..\dist");

        BuildPipeline.BuildAssetBundles(stagePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        foreach (var file in Directory.EnumerateFiles(stagePath))
        {
            if (file.EndsWith(".manifest") || file.EndsWith(stageDirName)) continue;

            string fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(distDir, fileName), true);
        }
    }
}
