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
                CreateCableMount(Vector3.right * CableMount.k_ConnectRange * 0.5f),
                CreateCableMount(Vector3.right * CableMount.k_ConnectRange * 1.5f)
            };
        }

        private CableMount CreateCableMount(Vector3 position)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DwarvenIndustry/Prefabs/_Common/CableMount/CableMount.prefab");
            return Object.Instantiate(prefab, position, Quaternion.identity).GetComponent<CableMount>();
        }

        [Test]
        public void TestStaticFindInRadius()
        {
            var mounts = CableMount.FindInRadius(Vector3.zero, CableMount.k_ConnectRange);
            Assert.AreEqual(2, mounts.Length);
            Assert.Contains(m_SetupMounts[0], mounts);
            Assert.Contains(m_SetupMounts[1], mounts);
        }

        [Test]
        public void TestFindInRadius()
        {
            var mounts = m_SetupMounts[0].FindInRadius(CableMount.k_ConnectRange);
            Assert.AreEqual(1, mounts.Length);
            Assert.Contains(m_SetupMounts[1], mounts);
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
        public void TestDisconnect()
        {
            var mount1 = m_SetupMounts[0];
            var mount2 = m_SetupMounts[2];

            mount1.Connect(mount2);
            Assert.True(mount1.IsConnected(mount2), "mount1 not connected to mount2");
            mount1.DisconnectAll();
            Assert.False(mount1.IsConnected(mount2), "mount1 connected to mount2");
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
