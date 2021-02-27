using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Dvalin.Editor;

namespace Dvalin.Tests
{
    public class TestContentLoader
    {
        [Test]
        public void TestLoad()
        {
            BuildUtils.BuildAssetBundles();

            var loader = new ContentLoader(new TestPaths());
            var info = loader.Load();
            Assert.IsNotEmpty(info.Pieces);
            Assert.IsNotNull(info.Localization);
        }
    }
}
