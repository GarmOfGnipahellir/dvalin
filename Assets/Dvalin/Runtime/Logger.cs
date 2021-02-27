using BepInEx.Logging;

namespace Dvalin
{
    /// <summary>
    /// Wrapped <see cref="ManualLogSource" /> with some additional methods for logging with format.
    /// </summary>
    public class Logger : IDestroyable
    {
        private readonly ManualLogSource m_logSource;

        public static Logger Instance { get; private set; }

        public Logger()
        {
            m_logSource = BepInEx.Logging.Logger.CreateLogSource(Plugin.NAME);
            Instance = this;
        }

        public void Destroy()
        {
            BepInEx.Logging.Logger.Sources.Remove(m_logSource);
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
}