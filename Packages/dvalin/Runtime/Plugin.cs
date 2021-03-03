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
            Patches.ObjectDB.Awake.PrefixEvent += AddCustomObjects;
            Patches.Player.OnSpawned.PostfixEvent += ModifyPieceTables;
            Patches.Smelter.Awake.PostfixEvent += ModifySmelterItemConversions;

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

            foreach (var itemInfo in m_ContentInfo.Items)
            {
                if (itemInfo == null || itemInfo.Prefab.getFromRuntime) continue;

                nscene.m_prefabs.Add(itemInfo.Prefab.gameObject);

                Dvalin.Logger.LogInfoFormat("Added item {0} to {1}", itemInfo.Prefab, nscene);
            }

            Dvalin.Logger.LogInfoFormat("Added Prefabs to {0}", nscene);
        }

        private void AddCustomObjects(ObjectDB objectDb)
        {
            foreach (var itemInfo in m_ContentInfo.Items)
            {
                if (itemInfo.Prefab.getFromRuntime) continue;

                objectDb.m_items.Add(itemInfo.Prefab.gameObject);

                Dvalin.Logger.LogInfoFormat("Added item {0} to {1}", itemInfo.Prefab, objectDb);
            }

            foreach (var recipe in m_ContentInfo.Recipes)
            {
                for (int i = 0; i < recipe.m_resources.Length; i++)
                {
                    var item = recipe.m_resources[i].m_resItem as ItemDropWrapper;
                    if (item == null || !item.getFromRuntime) continue;

                    var prefab = objectDb.GetItemPrefab(item.gameObject.name);
                    if (prefab == null) continue;

                    recipe.m_resources[i].m_resItem = prefab.GetComponent<ItemDrop>();
                }

                objectDb.m_recipes.Add(recipe);

                Dvalin.Logger.LogInfoFormat("Added recipe {0} to {1}", recipe, objectDb);
            }

            foreach (var pieceInfo in m_ContentInfo.Pieces)
            {
                for (int i = 0; i < pieceInfo.Prefab.m_resources.Length; i++)
                {
                    var item = pieceInfo.Prefab.m_resources[i].m_resItem as ItemDropWrapper;
                    if (item == null || !item.getFromRuntime) continue;

                    var prefab = objectDb.GetItemPrefab(item.gameObject.name);
                    if (prefab == null) continue;

                    pieceInfo.Prefab.m_resources[i].m_resItem = prefab.GetComponent<ItemDrop>();
                }

                Dvalin.Logger.LogInfoFormat("Fixed rintime types for {0}", pieceInfo.Prefab);
            }

            Dvalin.Logger.LogInfoFormat("Added Prefabs to {0}", objectDb);
        }

        private void ModifyPieceTables(Player player)
        {
            foreach (var pieceInfo in m_ContentInfo.Pieces)
            {
                pieceInfo.AddToTable(player);
            }
        }

        private void ModifySmelterItemConversions(Smelter smelter)
        {
            foreach (var smelterItemConversion in m_ContentInfo.SmelterItemConversions)
            {
                var itemConversion = new Smelter.ItemConversion();
                itemConversion.m_from = WrapperToRuntime(smelterItemConversion.from);
                itemConversion.m_to = WrapperToRuntime(smelterItemConversion.to);
                smelter.m_conversion.Add(itemConversion);

                Dvalin.Logger.LogInfoFormat("Added {0} -> {1} to {2}", itemConversion.m_from, itemConversion.m_to, smelter);
            }

            Dvalin.Logger.LogInfoFormat("Added item conversions to {0}", smelter);
        }

        private ItemDrop WrapperToRuntime(ItemDropWrapper wrapper)
        {
            if (!wrapper.getFromRuntime) return wrapper;

            var prefab = ObjectDB.instance.GetItemPrefab(wrapper.gameObject.name);
            if (prefab == null) return wrapper;

            return prefab.GetComponent<ItemDrop>();
        }
    }
}