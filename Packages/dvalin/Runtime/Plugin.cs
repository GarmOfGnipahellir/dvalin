using System.IO;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace Dvalin
{
    [BepInPlugin(k_Guid, k_Name, k_Version)]
    [BepInProcess("valheim.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public const string k_Author = "GarmOfGnipahellir";
        public const string k_Guid = "com.garmofgnipahellir.dvalin";
        public const string k_Name = "Dvalin";
        public const string k_Version = "0.3.0";
        public const string k_Description = "Will be filled out later...";
        public const string k_WebsiteUrl = "https://github.com/GarmOfGnipahellir/dvalin";
        public static readonly string[] k_Dependencies = { "denikson-BepInExPack_Valheim" };

        private Harmony m_Harmony;
        private ContentLoader m_ContentLoader;
        private readonly List<IDestroyable> m_Destroyables = new List<IDestroyable>();
        private ContentInfo m_ContentInfo;

        public static Paths paths { get; protected set; }

        void Awake()
        {
            Logger.LogInfo(Path.GetFullPath("AssetBundles"));

            paths = new Paths();

            m_Harmony = new Harmony(k_Guid);
            m_Harmony.PatchAll();

            m_Destroyables.Add(new Logger());

            LoadOtherAssemblies();

            m_Destroyables.Add(m_ContentLoader = new ContentLoader(paths));
            m_ContentInfo = m_ContentLoader.Load();

            m_Destroyables.Add(new Localization(m_ContentInfo.Localization));

            Patches.ZNetScene.Awake.PrefixEvent += AddCustomPrefabs;
            Patches.Player.OnSpawned.PostfixEvent += ModifyPieceTables;

            Dvalin.Logger.LogInfo("Plugin awoken.");
        }

        void OnDestroy()
        {
            Dvalin.Logger.LogInfo("Destroying plugin.");

            foreach (var pieceInfo in m_ContentInfo.Pieces)
            {
                pieceInfo.Destroy();
            }

            m_Harmony.UnpatchSelf();

            foreach (var destroyable in m_Destroyables)
            {
                destroyable.Destroy();
            }
        }

        private void LoadOtherAssemblies()
        {
            var dllFiles = Directory.EnumerateFiles(paths.PluginDir, "*.dll");
            foreach (var dllFile in dllFiles)
            {
                if (dllFile == Assembly.GetExecutingAssembly().Location) continue;
                Assembly.LoadFile(dllFile);
            }
        }

        private void AddCustomPrefabs(ZNetScene nscene)
        {
            foreach (var pieceInfo in m_ContentInfo.Pieces)
            {
                nscene.m_prefabs.Add(pieceInfo.Prefab.gameObject);

                Dvalin.Logger.LogInfoFormat("Added piece {0} to {1}", pieceInfo.Prefab, nscene);
            }

            Dvalin.Logger.LogInfoFormat("Added Prefabs to {0}", nscene);
        }

        private void ModifyPieceTables(Player player)
        {
            foreach (var pieceInfo in m_ContentInfo.Pieces)
            {
                pieceInfo.AddToTable(player);
            }
        }
    }
}