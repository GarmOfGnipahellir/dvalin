using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Dvalin
{
    public class ContentLoader : IDestroyable
    {
        private IPaths m_paths;

        public static ContentInfo ContentInfo { get; protected set; }

        public ContentLoader(IPaths paths)
        {
            m_paths = paths;
        }

        public void Destroy()
        {
            // TODO destroy stuff...
        }

        public ContentInfo Load()
        {
            List<PieceInfo> pieces = new List<PieceInfo>();
            TextAsset localization = null;

            var mainBundle = AssetBundle.LoadFromFile(Path.Combine(m_paths.AssetBundleDir, "AssetBundles"));
            var manifest = mainBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");

            foreach (var subBundleName in manifest.GetAllAssetBundles())
            {
                var subBundle = AssetBundle.LoadFromFile(Path.Combine(m_paths.AssetBundleDir, subBundleName));

                foreach (var prefab in subBundle.LoadAllAssets<GameObject>())
                {
                    if (TryGetComponent<PieceWrapper>(prefab, out PieceWrapper piece))
                    {
                        if (!Debug.isDebugBuild && !piece.m_includeInRealease) continue;
                        
                        pieces.Add(new PieceInfo(piece));
                    }
                }

                foreach (var textAsset in subBundle.LoadAllAssets<TextAsset>())
                {
                    if (textAsset.name == "localization")
                    {
                        localization = textAsset;
                    }
                }

                subBundle.Unload(false);
            }

            mainBundle.Unload(false);

            return new ContentInfo(pieces, localization);
        }

        public static bool TryGetComponent<T>(GameObject gameObject, out T component)
        {
            component = gameObject.GetComponent<T>();
            return component != null;
        }
    }
}