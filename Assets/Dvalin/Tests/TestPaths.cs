using System.IO;

namespace Dvalin.Tests
{
    public class TestPaths : IPaths
    {
        public string AssetBundleDir { get { return Path.GetFullPath("AssetBundles"); } }
    }
}