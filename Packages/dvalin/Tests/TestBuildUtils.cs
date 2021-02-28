using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using Dvalin.Editor;

namespace Dvalin.Tests
{
    public class TestBuildUtils
    {
        [Test]
        public void TestCreateManifest()
        {
            BuildUtils.CreateManifest();

            var manifest = new ThunderStoreManifest();

            using (var stream = File.OpenRead(
                    Path.Combine(BuildUtils.DIST_DIR, "manifest.json")))
            {
                using (var reader = new StreamReader(stream))
                {
                    EditorJsonUtility.FromJsonOverwrite(
                        reader.ReadToEnd(),
                        manifest
                    );
                }
            }
        }
    }
}