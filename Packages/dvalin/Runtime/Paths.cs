using System.IO;
using System.Reflection;

namespace Dvalin
{
    public class Paths : IPaths
    {
        public string PluginDir { get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); } }
        public string AssetBundleDir { get { return PluginDir; } }
    }
}