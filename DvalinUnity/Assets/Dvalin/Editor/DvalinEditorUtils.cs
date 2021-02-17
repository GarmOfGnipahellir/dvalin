using UnityEngine;
using UnityEditor;

public class DvalinEditorUtils : MonoBehaviour
{
    [MenuItem("Dvalin/Build Asset Bundles")]
    static void BuildAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
