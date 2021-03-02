using UnityEngine;

namespace Dvalin
{
    /// <summary>
    /// A wrapper for Valheim's <see cref="ItemDrop" />.
    /// </summary>
    public class ItemDropWrapper : ItemDrop
    {
        public bool getFromRuntime = false;
        public bool includeInRealease = false;
    }
}