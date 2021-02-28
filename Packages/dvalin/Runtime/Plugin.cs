using System.IO;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace Dvalin
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("valheim.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string AUTHOR = "GarmOfGnipahellir";
        public const string GUID = "com.garmofgnipahellir.dvalin";
        public const string NAME = "Dvalin";
        public const string VERSION = "0.3.0";
        public const string DESCRIPTION = "Will be filled out later...";
        public const string WEBSITE_URL = "https://github.com/GarmOfGnipahellir/dvalin";
        public static readonly string[] DEPENDENCIES = { "denikson-BepInExPack_Valheim" };

        private Harmony m_harmony;
        private ContentLoader m_contentLoader;
        private readonly List<IDestroyable> m_destroyables = new List<IDestroyable>();
        private ContentInfo m_contentInfo;

        public static Paths Paths { get; protected set; }

        void Awake()
        {
            Logger.LogInfo(Path.GetFullPath("AssetBundles"));

            Paths = new Paths();

            m_harmony = new Harmony(GUID);
            m_harmony.PatchAll();

            m_destroyables.Add(new Logger());

            m_destroyables.Add(m_contentLoader = new ContentLoader(Paths));
            m_contentInfo = m_contentLoader.Load();

            m_destroyables.Add(new Localization(m_contentInfo.Localization));

            Patches.ZNetScene.Awake.PrefixEvent += AddCustomPrefabs;
            Patches.Player.OnSpawned.PostfixEvent += ModifyPieceTables;

            Dvalin.Logger.LogInfo("Plugin awoken.");
        }

        void OnDestroy()
        {
            Dvalin.Logger.LogInfo("Destroying plugin.");

            foreach (var pieceInfo in m_contentInfo.Pieces)
            {
                pieceInfo.Destroy();
            }

            m_harmony.UnpatchSelf();

            foreach (var destroyable in m_destroyables)
            {
                destroyable.Destroy();
            }
        }

        private void AddCustomPrefabs(ZNetScene nscene)
        {
            foreach (var pieceInfo in m_contentInfo.Pieces)
            {
                nscene.m_prefabs.Add(pieceInfo.Prefab.gameObject);

                Dvalin.Logger.LogInfoFormat("Added piece {0} to {1}", pieceInfo.Prefab, nscene);
            }

            Dvalin.Logger.LogInfoFormat("Added Prefabs to {0}", nscene);
        }

        private void ModifyPieceTables(Player player)
        {
            foreach (var pieceInfo in m_contentInfo.Pieces)
            {
                pieceInfo.AddToTable(player);
            }
        }
    }
}