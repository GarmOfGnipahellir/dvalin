using UnityEngine;

namespace Dvalin
{
    /// <summary>
    /// A wrapper for Valheim's <see cref="Smelter.ItemConversion" />.
    /// </summary>
    [UnityEngine.CreateAssetMenu]
    public class SmelterItemConversionWrapper : ScriptableObject
    {
        public ItemDropWrapper from;
        public ItemDropWrapper to;
        public bool includeInRealease = false;
    }
}