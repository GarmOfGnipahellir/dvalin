using System.IO;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Dvalin.API;
using Dvalin.Patches;

namespace Dvalin.Core
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInProcess("valheim.exe")]
    public class DvalinPlugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.garm.dvalin";
        public const string PLUGIN_NAME = "Dvalin";
        public const string PLUGIN_VERSION = "0.1.1";

        private Harmony m_harmony;
        private readonly List<IDvalinDestroyable> m_destroyables = new List<IDvalinDestroyable>();
        private readonly List<DvalinPiece> m_pieces = new List<DvalinPiece>();

        public static string PluginDir { get { return Path.Combine(Paths.PluginPath, PLUGIN_NAME); } }

        void Awake()
        {
            // need access to scripts from unity
            Assembly.LoadFrom(Path.Combine(PluginDir, "DvalinUnity.dll"));

            m_harmony = new Harmony(PLUGIN_GUID);
            m_harmony.PatchAll();

            m_destroyables.Add(new DvalinLogger());
            m_destroyables.Add(new DvalinLocalization());
            
            #if DEBUG
            RegisterPiece("dvalin_piece_dvalintest");
            #endif
            RegisterPiece("dvalin_piece_maptable");

            ZNetSceneAwake.PrefixEvent += AddCustomPrefabs;
            PlayerOnSpawned.PostfixEvent += ModifyPieceTables;

            DvalinLogger.LogInfo("Plugin awoken.");
        }

        void OnDestroy()
        {
            DvalinLogger.LogInfo("Destroying plugin.");

            foreach (var dvalinPiece in m_pieces)
            {
                dvalinPiece.Destroy();
            }

            m_harmony.UnpatchSelf();

            foreach (var destroyable in m_destroyables)
            {
                destroyable.Destroy();
            }
        }

        public void RegisterPiece(string pieceName)
        {
            if (DvalinPiece.TryCreate(pieceName, out DvalinPiece dvalinPiece))
            {
                m_pieces.Add(dvalinPiece);

                DvalinLogger.LogInfoFormat("Registered: {0}", dvalinPiece);
            }
            else
            {
                DvalinLogger.LogErrorFormat("Couldn't register: {0}", pieceName);
            }
        }

        private void AddCustomPrefabs(ZNetScene zNetScene)
        {
            foreach (var dvalinPiece in m_pieces)
            {
                zNetScene.m_prefabs.Add(dvalinPiece.Prefab.gameObject);
            }
        }

        private void ModifyPieceTables(Player player)
        {
            foreach (var dvalinPiece in m_pieces)
            {
                dvalinPiece.AddToTable(player);
            }
        }
    }

    public class DvalinLogger : IDvalinDestroyable
    {
        private readonly ManualLogSource m_logSource;

        public static DvalinLogger Instance { get; private set; }

        public DvalinLogger()
        {
            m_logSource = Logger.CreateLogSource(DvalinPlugin.PLUGIN_NAME);
            Instance = this;
        }

        public void Destroy()
        {
            Logger.Sources.Remove(m_logSource);
        }

        public static void Log(LogLevel level, object data)
        {
            Instance.m_logSource.Log(level, data);
        }
        public static void LogFormat(LogLevel level, string format, params object[] args)
        {
            Log(level, string.Format(format, args));
        }

        public static void LogDebug(object data)
        {
            Log(LogLevel.Debug, data);
        }
        public static void LogDebugFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Debug, format, args);
        }

        public static void LogInfo(object data)
        {
            Log(LogLevel.Info, data);
        }
        public static void LogInfoFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Info, format, args);
        }

        public static void LogWarning(object data)
        {
            Log(LogLevel.Warning, data);
        }
        public static void LogWarningFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Warning, format, args);
        }

        public static void LogError(object data)
        {
            Log(LogLevel.Error, data);
        }
        public static void LogErrorFormat(string format, params object[] args)
        {
            LogFormat(LogLevel.Error, format, args);
        }
    }

    public interface IDvalinDestroyable
    {
        void Destroy();
    }
}
