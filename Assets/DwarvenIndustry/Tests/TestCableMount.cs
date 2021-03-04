using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests
{
    public class TestCableMount
    {
        CableMount[] m_SetupMounts;

        [SetUp]
        public void SetUp()
        {
            m_SetupMounts = new[] {
                CreateCableMount(Vector3.zero),
                CreateCableMount(Vector3.right * 5),
                CreateCableMount(Vector3.right * 15)
            };
        }

        private CableMount CreateCableMount(Vector3 position)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/_Common/CableMount/CableMount.prefab");
            return Object.Instantiate(prefab, position, Quaternion.identity).GetComponent<CableMount>();
        }

        [UnityTest]
        public IEnumerator TestAutoConnect()
        {
            yield return null;

            Assert.True(m_SetupMounts[0].IsConnected(m_SetupMounts[1]), "mount0 not connected to mount1");
            Assert.True(m_SetupMounts[1].IsConnected(m_SetupMounts[2]), "mount1 not connected to mount2");

            Assert.True(m_SetupMounts[2].IsConnected(m_SetupMounts[1]), "mount2 not connected to mount1");
            Assert.True(m_SetupMounts[1].IsConnected(m_SetupMounts[0]), "mount1 not connected to mount0");
        }

        [Test]
        public void TestManualConnect()
        {
            var mount1 = m_SetupMounts[0];
            var mount2 = m_SetupMounts[2];

            Assert.False(mount1.IsConnected(mount2), "mount1 already connected to mount2");
            Assert.False(mount2.IsConnected(mount1), "mount2 already connected to mount1");

            mount1.Connect(mount2);

            Assert.True(mount1.IsConnected(mount2), "mount1 not connected to mount2");
            Assert.True(mount2.IsConnected(mount1), "mount2 not connected to mount1");
        }

        [Test]
        public void TestStaticFindInRadius()
        {
            var mounts = CableMount.FindInRadius(Vector3.zero, 10);
            Assert.AreEqual(2, mounts.Length);
            Assert.Contains(m_SetupMounts[0], mounts);
            Assert.Contains(m_SetupMounts[1], mounts);
        }

        [Test]
        public void TestFindInRadius()
        {
            var mounts = m_SetupMounts[0].FindInRadius(10);
            Assert.AreEqual(1, mounts.Length);
            Assert.Contains(m_SetupMounts[1], mounts);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var mount in m_SetupMounts)
            {
                Object.DestroyImmediate(mount.gameObject);
            }
        }
    }
}
